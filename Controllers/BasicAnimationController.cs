using Microsoft.Xna.Framework;
using VXEngine.Animation;
using VXEngine.Utility;

namespace VXEngine.Controllers {
    public class BasicAnimationController {

        protected List<AnimationInstance> _animations;

        public BasicAnimationController( ) {
            _animations = new List<AnimationInstance>( );
        }

        public virtual AnimationInstance PlayOnce(float source, float target, float duration, float delay = 0, bool yoyo = false, Func<float, float> ease = null, Action<AnimationInstance> onPlay = null, Action<AnimationInstance> onPause = null, Action<AnimationInstance> onResume = null, Action<AnimationInstance> onStop = null, Action<AnimationInstance> onFinish = null, Action<AnimationInstance> onRepeat = null, Action<AnimationInstance> onReverse = null, Action<AnimationInstance> onUpdate = null) => Register(source, target, duration, delay, 0, 0, yoyo, ease, true, onPlay, onPause, onResume, onStop, onFinish, onRepeat, onReverse, onUpdate);
        public virtual AnimationInstance PlayLooped(float source, float target, float duration, float delay = 0, float repeatDelay = 0, bool yoyo = false, Func<float, float> ease = null, Action<AnimationInstance> onPlay = null, Action<AnimationInstance> onPause = null, Action<AnimationInstance> onResume = null, Action<AnimationInstance> onStop = null, Action<AnimationInstance> onFinish = null, Action<AnimationInstance> onRepeat = null, Action<AnimationInstance> onReverse = null, Action<AnimationInstance> onUpdate = null) => Register(source, target, duration, delay, -1, repeatDelay, yoyo, ease, true, onPlay, onPause, onResume, onStop, onFinish, onRepeat, onReverse, onUpdate);
        public virtual AnimationInstance PlayRepeated(float source, float target, float duration, int repeat = 0, float repeatDelay = 0, float delay = 0, bool yoyo = false, Func<float, float> ease = null, Action<AnimationInstance> onPlay = null, Action<AnimationInstance> onPause = null, Action<AnimationInstance> onResume = null, Action<AnimationInstance> onStop = null, Action<AnimationInstance> onFinish = null, Action<AnimationInstance> onRepeat = null, Action<AnimationInstance> onReverse = null, Action<AnimationInstance> onUpdate = null) => Register(source, target, duration, delay, repeat, repeatDelay, yoyo, ease, true, onPlay, onPause, onResume, onStop, onFinish, onRepeat, onReverse, onUpdate);

        public virtual AnimationInstance Register(float source, float target, float duration, float delay = 0, int repeat = 0, float repeatDelay = 0, bool yoyo = false, Func<float, float> ease = null, bool autoplay = false, Action<AnimationInstance> onPlay = null, Action<AnimationInstance> onPause = null, Action<AnimationInstance> onResume = null, Action<AnimationInstance> onStop = null, Action<AnimationInstance> onFinish = null, Action<AnimationInstance> onRepeat = null, Action<AnimationInstance> onReverse = null, Action<AnimationInstance> onUpdate = null) {
            AnimationInstance instance = new AnimationInstance(source, target, duration, delay, repeat, repeatDelay, yoyo, ease);
            instance.OnPlay = onPlay;
            instance.OnPause = onPause;
            instance.OnResume = onResume;
            instance.OnStop = onStop;
            instance.OnFinish = onFinish;
            instance.OnRepeat = onRepeat;
            instance.OnReverse = onReverse;
            instance.OnUpdate = onUpdate;

            _animations.Add(instance);

            if (autoplay)
                instance.PlayRepeat(repeat);

            return instance;
        }

        public virtual void Delay(float delay, Action<AnimationInstance> onComplete) {
            AnimationInstance instance = new AnimationInstance(0, 1, 0, delay, 0, 0, false, Easing.Linear);
            instance.OnFinish = onComplete;
            _animations.Add(instance);
            instance.PlayOnce( );
        }

        public virtual void Dispose(AnimationInstance instance) {
            AnimationInstance animation = _animations.Find(obj => obj == instance);
            if (animation != null) {
                animation.Dispose( );
                _animations.Remove(animation);
            }
        }

        public virtual void StopAllAnimations( ) {
            foreach (AnimationInstance animation in _animations)
                animation.Dispose( );
            _animations.Clear( );
        }

        public virtual void Update(GameTime time) {
            for (int i = 0; i < _animations.Count; i++) {
                AnimationInstance instance = _animations[i];
                if (instance.IsDisposed) {
                    _animations.RemoveAt(i);
                    i--;
                    continue;
                }

                instance.Update(time);
            }
        }

    }
}
