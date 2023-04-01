using Microsoft.Xna.Framework;
using VXEngine.Animation;
using VXEngine.Utility;

namespace VXEngine.Controllers {
    public class BasicAnimationController {

        protected List<AnimationInstance> _animations;

        public BasicAnimationController( ) {
            _animations = new List<AnimationInstance>( );
        }

        public virtual AnimationInstance Register(float source, float target, float duration, float delay = 0, float repeatDelay = 0, int repeat = 0, Func<float, float> ease = null) {
            AnimationInstance instance = new AnimationInstance(source, target, duration, delay, repeatDelay, repeat, ease);
            _animations.Add(instance);
            return instance;
        }

        public virtual AnimationInstance Once(float source, float target, float duration, float delay = 0, Func<float, float> ease = null) {
            AnimationInstance instance = new AnimationInstance(source, target, duration, delay, 0, 0, ease);
            instance.DisposeOnFinish = true;
            _animations.Add(instance);
            instance.PlayOnce( );
            return instance;
        }

        public virtual void Delay(float delay, Action<AnimationInstance> onComplete) {
            AnimationInstance instance = new AnimationInstance(0, 1, 0, delay, 0, 0, Easing.Linear);
            instance.DisposeOnFinish = true;
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
                instance.Update(time);
                if (instance.IsDisposed) {
                    _animations.RemoveAt(i);
                    i--;
                    continue;
                }
            }
        }

    }
}
