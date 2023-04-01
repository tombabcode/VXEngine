using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using VXEngine.Controllers;

namespace VXEngine.Audio {
    /// <summary>
    /// Sound effect instance
    /// </summary>
    public class SoundInstance {

        /// <summary>
        /// Reference config
        /// </summary>
        protected BasicConfigController _config;

        /// <summary>
        /// Checks if sound is started. Used for counting repeats
        /// </summary>
        protected bool _wasStarted = false;

        /// <summary>
        /// How many repeats left
        /// </summary>
        protected int _repeatsLeft;

        /// <summary>
        /// Sound instance
        /// </summary>
        public SoundEffectInstance Instance { get; protected set; }

        /// <summary>
        /// This object's volume level
        /// </summary>
        public float Volume { get; protected set; } = 1;

        /// <summary>
        /// Total volume output
        /// </summary>
        public float VolumeOutput => Volume * _config.VolumeMaster * _config.VolumeSound;

        /// <summary>
        /// Should instance be disposed by the controller after finish
        /// </summary>
        public bool DisposeOnFinish { get; set; }

        /// <summary>
        /// Loop flag
        /// </summary>
        public bool IsLooped => RepeatCount == -1;

        /// <summary>
        /// Dispose flag
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// How many times sound should be repeated before stop
        /// </summary>
        public int RepeatCount { get; protected set; } = 0;

        /// <summary>
        /// On play event
        /// </summary>
        public Action<SoundInstance> OnPlay { get; set; }

        /// <summary>
        /// On resume event
        /// </summary>
        public Action<SoundInstance> OnResume { get; set; }

        /// <summary>
        /// On pause event
        /// </summary>
        public Action<SoundInstance> OnPause { get; set; }

        /// <summary>
        /// On stop event
        /// </summary>
        public Action<SoundInstance> OnStop { get; set; }

        /// <summary>
        /// On repeat event
        /// </summary>
        public Action<SoundInstance> OnRepeat { get; set; }

        /// <summary>
        /// When all repeats was done
        /// </summary>
        public Action<SoundInstance> OnFinish { get; set; }

        /// <summary>
        /// On volume change event
        /// </summary>
        public Action<SoundInstance> OnVolumeChange { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SoundInstance(BasicConfigController config, SoundEffect audio) {
            _config = config;
            Instance = audio.CreateInstance( );
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SoundInstance(BasicConfigController config, SoundEffectInstance audio) {
            _config = config;
            Instance = audio;
        }

        /// <summary>
        /// Play sound only once
        /// </summary>
        public virtual void PlayOnce( ) {
            RepeatCount = 0;
            _repeatsLeft = 0;
            _wasStarted = true;

            Instance.Volume = VolumeOutput;
            Instance.IsLooped = false;
            Instance.Play( );
            OnPlay?.Invoke(this);
        }

        /// <summary>
        /// Play sound in loop
        /// </summary>
        public virtual void PlayLooped( ) {
            RepeatCount = -1;
            _repeatsLeft = -1;
            _wasStarted = true;

            Instance.Volume = VolumeOutput;
            Instance.IsLooped = true;
            Instance.Play( );
            OnPlay?.Invoke(this);
        }

        /// <summary>
        /// Play sound and repeat it given times
        /// </summary>
        public virtual void PlayRepeat(int repeatCount) {
            if (repeatCount < 0)
                PlayLooped( );
            else if (repeatCount == 0)
                PlayOnce( );
            else {
                RepeatCount = repeatCount;
                _repeatsLeft = repeatCount;
                _wasStarted = true;

                Instance.Volume = VolumeOutput;
                Instance.IsLooped = false;
                Instance.Play( );
                OnPlay?.Invoke(this);
            }
        }

        /// <summary>
        /// Pause sound
        /// </summary>
        public virtual void Pause( ) {
            if (Instance.State == SoundState.Paused)
                return;

            Instance.Pause( );
            OnPause?.Invoke(this);
        }

        /// <summary>
        /// Resume sound
        /// </summary>
        public virtual void Resume( ) {
            if (Instance.State != SoundState.Paused)
                return;

            Instance.Resume( );
            OnResume?.Invoke(this);
        }

        /// <summary>
        /// Stop sound
        /// </summary>
        public virtual void Stop( ) {
            if (Instance.State == SoundState.Stopped)
                return;

            Instance.Stop( );
            OnStop?.Invoke(this);
        }

        /// <summary>
        /// Loop sound
        /// </summary>
        public virtual void SetLoop( ) {
            RepeatCount = -1;
            _repeatsLeft = -1;
        }

        /// <summary>
        /// Set how many times sound should repeat
        /// </summary>
        public virtual void SetRepeat(int repeatCount) {
            if (repeatCount < 0)
                SetLoop( );
            else {
                RepeatCount = repeatCount;
                _repeatsLeft = repeatCount;
            }
        }

        /// <summary>
        /// Set volume
        /// </summary>
        public virtual void SetVolume(float volume) {
            if (volume < 0) volume = 0;
            if (volume > 1) volume = 1;

            if (Volume == volume)
                return;

            Volume = volume;

            if (Instance != null) {
                Instance.Volume = VolumeOutput;
                OnVolumeChange?.Invoke(this);
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public virtual void Dispose( ) {
            _wasStarted = false;
            _config = null;
            if (Instance != null) {
                if (Instance.State != SoundState.Stopped)
                    Instance.Stop( );
                Instance.Dispose( );
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        public virtual void Update(GameTime time) {
            if (!_wasStarted)
                return;

            // Repeat if needed
            if (Instance.State == SoundState.Stopped && _repeatsLeft > 0) {
                _repeatsLeft--;
                Instance.Play( );
                OnRepeat?.Invoke(this);
            }

            // Absolute stop
            if (Instance.State == SoundState.Stopped && _repeatsLeft == 0) {
                _wasStarted = false;
                OnFinish?.Invoke(this);
                if (DisposeOnFinish) {
                    Dispose( );
                    IsDisposed = true;
                }
            }
        }

    }
}
