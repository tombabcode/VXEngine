namespace VXEngine.Utility;

public static class Motion {

    /// <summary>
    /// Smooths the motion
    /// </summary>
    /// <param name="source">Source position</param>
    /// <param name="target">Target position</param>
    /// <param name="delta">Delta time in ms</param>
    /// <param name="inertia">How soft the movement should be. Default is 20</param>
    /// <returns>New source, smoothen value</returns>
    public static float Smooth(float source, float target, float delta, float inertia = 20) {
        if (delta > 1000) delta = 1000;

        bool direction = true;
        if (source > target)
            direction = false;

        float move = (!direction ? 1 : -1) * delta / (inertia * 16.66f) * Math.Abs(source - target);
        source = source - move;

        if ((direction && source > target) || (!direction && source < target))
            source = target;

        return source;
    }

}
