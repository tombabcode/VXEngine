namespace VXEngine.Utility;

/// <summary>
/// Utility for easing
/// </summary>
public static class Easing {

    /// <summary>
    /// Linear easing
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float Linear(float value) => value;

    /// <summary>
    /// Quadratic easing IN
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float QuadIn(float value) => value * value;

    /// <summary>
    /// Quadratic easing OUT
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float QuadOut(float value) => 1 - (1 - value) * (1 - value);

    /// <summary>
    /// Quadratic easing IN and OUT
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float QuadInOut(float value) => (float)(value < 0.5 ? 2 * value * value : 1 - Math.Pow(-2 * value + 2, 2) / 2);

    /// <summary>
    /// Cubic easing IN
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float CubicIn(float value) => value * value * value;

    /// <summary>
    /// Cubic easing OUT
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float CubicOut(float value) => 1 - (float)Math.Pow(1 - value, 3);

    /// <summary>
    /// Cubic easing IN and OUT
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float CubicInOut(float value) => (float)(value < 0.5 ? 4 * value * value * value : 1 - Math.Pow(-2 * value + 2, 3) / 2);

}
