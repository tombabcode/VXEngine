using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using VXEngine.Audio;

namespace VXEngine.Controllers {
    public class BasicAudioController {

        /// <summary>
        /// Music instances
        /// </summary>
        protected readonly List<MusicInstance> _music;

        /// <summary>
        /// Sound instances
        /// </summary>
        protected readonly List<SoundInstance> _sounds;

        /// <summary>
        /// Constructor
        /// </summary>
        public BasicAudioController( ) {
            _music = new List<MusicInstance>( );
            _sounds = new List<SoundInstance>( );
        }

        /// <summary>
        /// Registers new music
        /// </summary>
        public virtual MusicInstance RegisterMusic(BasicConfigController config, SoundEffect music) {
            MusicInstance instance = new MusicInstance(config, music);
            _music.Add(instance);
            return instance;
        }

        /// <summary>
        /// Registers new sound effect
        /// </summary>
        public virtual SoundInstance RegisterSound(BasicConfigController config, SoundEffect sound) {
            SoundInstance instance = new SoundInstance(config, sound);
            _sounds.Add(instance);
            return instance;
        }

        /// <summary>
        /// Play music once
        /// </summary>
        public virtual MusicInstance OnceMusic(BasicConfigController config, SoundEffect music) {
            MusicInstance instance = new MusicInstance(config, music);
            _music.Add(instance);
            instance.DisposeOnFinish = true;
            instance.PlayOnce( );
            return instance;
        }

        /// <summary>
        /// Play music once
        /// </summary>
        public virtual SoundInstance OnceSound(BasicConfigController config, SoundEffect sound) {
            SoundInstance instance = new SoundInstance(config, sound);
            _sounds.Add(instance);
            instance.DisposeOnFinish = true;
            instance.PlayOnce( );
            return instance;
        }

        /// <summary>
        /// Remove given music instance
        /// </summary>
        /// <param name="instance"></param>
        public virtual void Dispose(MusicInstance instance) {
            MusicInstance music = _music.Find(obj => obj == instance);
            if (music != null) {
                music.Dispose( );
                _music.Remove(music);
            }
        }

        /// <summary>
        /// Remove given sound instance
        /// </summary>
        /// <param name="instance"></param>
        public virtual void Dispose(SoundInstance instance) {
            SoundInstance sound = _sounds.Find(obj => obj == instance);
            if (sound != null) {
                sound.Dispose( );
                _sounds.Remove(sound);
            }
        }

        /// <summary>
        /// Stops all music
        /// </summary>
        public virtual void StopAllMusic( ) {
            foreach (MusicInstance music in _music)
                music.Dispose( );
            _music.Clear( );
        }

        /// <summary>
        /// Stops all sounds
        /// </summary>
        public virtual void StopAllSound( ) {
            foreach (SoundInstance sound in _sounds)
                sound.Dispose( );
            _sounds.Clear( );
        }

        /// <summary>
        /// Stops all audio immediately
        /// </summary>
        public virtual void StopAllAudio( ) {
            StopAllMusic( );
            StopAllSound( );
        }

        /// <summary>
        /// Update
        /// </summary>
        public virtual void Update(GameTime time) {
            // Update music
            for (int i = 0; i < _music.Count; i++) {
                MusicInstance instance = _music[i];
                instance.Update(time);
                if (instance.IsDisposed) {
                    _music.RemoveAt(i);
                    i--;
                    continue;
                }
            }

            // Update sound
            for (int i = 0; i < _sounds.Count; i++) {
                SoundInstance instance = _sounds[i];
                instance.Update(time);
                if (instance.IsDisposed) {
                    _sounds.RemoveAt(i);
                    i--;
                    continue;
                }
            }
        }
    }
}
