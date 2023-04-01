using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using VXEngine.Controllers;

namespace VXEngine.Audio {
    /// <summary>
    /// Sound effect instance
    /// </summary>
    public class MusicInstance {

        /// <summary>
        /// Reference config
        /// </summary>
        protected BasicConfigController _config;

        /// <summary>
        /// Checks if music is started. Used for counting repeats
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
        public float VolumeOutput => Volume * _config.VolumeMaster * _config.VolumeMusic;

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
        /// How many times music should be repeated before stop
        /// </summary>
        public int RepeatCount { get; protected set; } = 0;

        /// <summary>
        /// On play event
        /// </summary>
        public Action<MusicInstance> OnPlay { get; set; }

        /// <summary>
        /// On resume event
        /// </summary>
        public Action<MusicInstance> OnResume { get; set; }

        /// <summary>
        /// On pause event
        /// </summary>
        public Action<MusicInstance> OnPause { get; set; }

        /// <summary>
        /// On stop event
        /// </summary>
        public Action<MusicInstance> OnStop { get; set; }

        /// <summary>
        /// On repeat event
        /// </summary>
        public Action<MusicInstance> OnRepeat { get; set; }

        /// <summary>
        /// When all repeats was done
        /// </summary>
        public Action<MusicInstance> OnFinish { get; set; }

        /// <summary>
        /// On volume change event
        /// </summary>
        public Action<MusicInstance> OnVolumeChange { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MusicInstance(BasicConfigController config, SoundEffect audio) {
            _config = config;
            Instance = audio.CreateInstance( );
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MusicInstance(BasicConfigController config, SoundEffectInstance audio) {
            _config = config;
            Instance = audio;
        }

        /// <summary>
        /// Play music only once
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
        /// Play music in loop
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
        /// Play music and repeat it given times
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
        /// Pause music
        /// </summary>
        public virtual void Pause( ) {
            if (Instance.State == SoundState.Paused)
                return;

            Instance.Pause( );
            OnPause?.Invoke(this);
        }

        /// <summary>
        /// Resume music
        /// </summary>
        public virtual void Resume( ) {
            if (Instance.State != SoundState.Paused)
                return;

            Instance.Resume( );
            OnResume?.Invoke(this);
        }

        /// <summary>
        /// Stop music
        /// </summary>
        public virtual void Stop( ) {
            if (Instance.State == SoundState.Stopped)
                return;

            Instance.Stop( );
            OnStop?.Invoke(this);
        }

        /// <summary>
        /// Loop music
        /// </summary>
        public virtual void SetLoop( ) {
            RepeatCount = -1;
            _repeatsLeft = -1;
        }

        /// <summary>
        /// Set how many times music should repeat
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
