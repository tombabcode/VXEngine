using Microsoft.Xna.Framework;
using VXEngine.Types;
using VXEngine.Utility;

namespace VXEngine.Animation {
    public class AnimationInstance {

        // Helpers
        protected float _timePassed;

        /// <summary>
        /// How many repeats left
        /// </summary>
        protected int _repeatsLeft;

        /// <summary>
        /// Current state
        /// </summary>
        public AnimationState State { get; protected set; }

        /// <summary>
        /// Source value
        /// </summary>
        public float Source { get; set; }

        /// <summary>
        /// Target value
        /// </summary>
        public float Target { get; set; }

        /// <summary>
        /// Total animation duration in ms
        /// </summary>
        public float Duration { get; set; }

        public bool Yoyo { get; set; } = false;

        /// <summary>
        /// Initial animation delay
        /// </summary>
        public float Delay { get; protected set; }

        /// <summary>
        /// Delay before repeat
        /// </summary>
        public float RepeatDelay { get; protected set; }

        /// <summary>
        /// Current value, calculated from <see cref="Source"/>, <see cref="Target"/> and <see cref="Progress"/>
        /// </summary>
        public float Value { get; protected set; }

        /// <summary>
        /// Current animation's progress. In range of 0 to 1
        /// </summary>
        public float Progress { get; protected set; }

        /// <summary>
        /// Loop flag
        /// </summary>
        public bool IsLooped => RepeatCount == -1;

        /// <summary>
        /// Dispose flag
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// How many times animation should be repeated before stop
        /// </summary>
        public int RepeatCount { get; protected set; }

        /// <summary>
        /// Custom easing function
        /// </summary>
        public Func<float, float> EaseFunction { get; set; }

        /// <summary>
        /// On animation start event
        /// </summary>
        public Action<AnimationInstance> OnPlay { get; set; } = null;

        /// <summary>
        /// On animation pause event
        /// </summary>
        public Action<AnimationInstance> OnPause { get; set; } = null;

        /// <summary>
        /// On animation resume event
        /// </summary>
        public Action<AnimationInstance> OnResume { get; set; } = null;

        /// <summary>
        /// On animation stop event
        /// </summary>
        public Action<AnimationInstance> OnStop { get; set; } = null;

        /// <summary>
        /// On animation absolute stop event
        /// </summary>
        public Action<AnimationInstance> OnFinish { get; set; } = null;

        /// <summary>
        /// On animation repeat event
        /// </summary>
        public Action<AnimationInstance> OnRepeat { get; set; } = null;

        /// <summary>
        /// On animation reverse event
        /// </summary>
        public Action<AnimationInstance> OnReverse { get; set; } = null;

        /// <summary>
        /// On animation update event
        /// </summary>
        public Action<AnimationInstance> OnUpdate { get; set; } = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public AnimationInstance(float source, float target, float duration, float delay = 0, int repeat = 0, float repeatDelay = 0, bool yoyo = false, Func<float, float> ease = null) {
            Source = source;
            Target = target;
            Duration = duration < 0 ? 0 : duration;
            Delay = delay < 0 ? 0 : delay;
            RepeatDelay = repeatDelay < 0 ? 0 : repeatDelay;
            RepeatCount = repeat;
            _repeatsLeft = repeat;
            Yoyo = yoyo;
            EaseFunction = ease ?? Easing.Linear;

            Progress = 0;
            Value = Source;
        }

        /// <summary>
        /// Play animation once
        /// </summary>
        public void PlayOnce( ) {
            RepeatCount = 0;
            _repeatsLeft = 0;
            _timePassed = -Delay;
            Progress = 0;
            Value = Source;

            State = AnimationState.Playing;
            OnPlay?.Invoke(this);
        }

        /// <summary>
        /// Play animation in loop
        /// </summary>
        public void PlayLooped( ) {
            RepeatCount = -1;
            _repeatsLeft = -1;
            _timePassed = -Delay;
            Progress = 0;
            Value = Source;

            State = AnimationState.Playing;
            OnPlay?.Invoke(this);
        }

        /// <summary>
        /// Play animation and repeat it given times
        /// </summary>
        public void PlayRepeat(int repeatCount) {
            if (repeatCount < 0) {
                PlayLooped( );
            } else if (repeatCount == 0) {
                PlayOnce( );
            } else {
                RepeatCount = repeatCount;
                _repeatsLeft = repeatCount;
                _timePassed = -Delay;
                Progress = 0;
                Value = Source;

                State = AnimationState.Playing;
                OnPlay?.Invoke(this);
            }
        }

        /// <summary>
        /// Pause animation
        /// </summary>
        public void Pause( ) {
            if (State == AnimationState.Paused)
                return;

            State = AnimationState.Paused;
            OnPause?.Invoke(this);
        }

        /// <summary>
        /// Resume animation
        /// </summary>
        public void Resume( ) {
            if (State != AnimationState.Paused)
                return;

            State = AnimationState.Playing;
            OnResume?.Invoke(this);
        }

        /// <summary>
        /// Stops animation
        /// </summary>
        public void Stop( ) {
            if (State == AnimationState.Stopped)
                return;

            Value = Target;
            Progress = 1;
            _timePassed = Duration;
            _repeatsLeft = 0;
            State = AnimationState.Stopped;
            OnStop?.Invoke(this);
        }

        /// <summary>
        /// Reverse animation
        /// </summary>
        public void Reverse(bool swapEasing = false) {
            float tempSource = Source;
            Source = Target;
            Target = tempSource;
            Progress = 1 - Progress;
            _timePassed = Duration - _timePassed;
            OnReverse?.Invoke(this);
            State = AnimationState.Playing;
            OnPlay?.Invoke(this);

            if (swapEasing) {
                if (EaseFunction == Easing.QuadIn) EaseFunction = Easing.QuadOut;
                else if (EaseFunction == Easing.QuadOut) EaseFunction = Easing.QuadIn;
                else if (EaseFunction == Easing.CubicIn) EaseFunction = Easing.CubicOut;
                else if (EaseFunction == Easing.CubicOut) EaseFunction = Easing.CubicIn;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose( ) {
            IsDisposed = true;
        }

        /// <summary>
        /// Update animation's logic
        /// </summary>
        public void Update(GameTime time) {
            // Skip if animation is not played
            if (State != AnimationState.Playing)
                return;

            // Add time
            _timePassed += (float)time.ElapsedGameTime.TotalMilliseconds;

            // Skip if delayed
            if (_timePassed <= 0)
                return;

            // Calculate progress
            Progress = Duration == 0 ? 1 : _timePassed / Duration;

            // Delayed
            if (Progress <= 0) {
                Progress = 0;
                Value = Source;
                OnUpdate?.Invoke(this);

            // Finished
            } else if (Progress >= 1) {
                // Repeat or loop
                if (_repeatsLeft > 0 || _repeatsLeft == -1) {
                    if (_repeatsLeft > 0)
                        _repeatsLeft -= 1;

                    if (Yoyo)
                        Reverse(true);
                    else {
                        _timePassed -= (Duration + RepeatDelay);
                        Progress = Duration == 0 ? 1 : _timePassed / Duration;
                        Value = (Target - Source) * EaseFunction.Invoke(Progress) + Source;
                    }

                    OnUpdate?.Invoke(this);
                    OnRepeat?.Invoke(this);

                // Absolute finish
                } else {
                    Progress = 1;
                    Value = Target;
                    State = AnimationState.Stopped;
                    OnUpdate?.Invoke(this);
                    OnFinish?.Invoke(this);

                    Dispose( );
                }

            // During
            } else {
                Value = (Target - Source) * EaseFunction.Invoke(Progress) + Source;
                OnUpdate?.Invoke(this);
            }
        }

    }
}
