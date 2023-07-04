namespace VXEngine.Utility;

/// <summary>
/// Smoothen the values
/// </summary>
public static class Motion {

    /// <summary>
    /// Smooths the motion
    /// </summary>
    /// <param name="source">Source position</param>
    /// <param name="target">Target position</param>
    /// <param name="delta">Delta time in ms</param>
    /// <param name="inertia">How soft the movement should be. Default is 20</param>
    /// <param name="margin">How detailed should be the result</param>
    /// <returns></returns>
    public static float Smooth(float source, float target, float delta, float inertia = 20, float margin = .01f) {
        if (delta > 1000) delta = 1000;

        bool direction = true;
        if (source > target)
            direction = false;

        float move = (!direction ? 1 : -1) * delta / (inertia * 16.66f) * Math.Abs(source - target);
        source = source - move;

        if ((direction && source > target) || (!direction && source < target) || Math.Abs(target - source) <= Math.Abs(margin))
            source = target;

        return source;
    }

}
