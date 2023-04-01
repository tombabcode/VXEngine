using Microsoft.Xna.Framework;
using VXEngine.Objects;

namespace VXEngine.Utility;

public static class Calculations {

    /// <summary>
    /// Calculate distance between two points
    /// </summary>
    public static float Distance(float sourceX, float sourceY, float targetX, float targetY) => sourceX != 0 || sourceY != 0 || targetX != 0 || targetY != 0
        ? (float)Math.Sqrt(Math.Pow(sourceX - targetX, 2) + Math.Pow(sourceY - targetY, 2))
        : 0;

    /// <summary>
    /// Calculate distance between two points
    /// </summary>
    public static float Distance(Vector2 source, Vector2 target) => Distance(source.X, source.Y, target.X, target.Y);

    /// <summary>
    /// Calculate distance between two objects
    /// </summary>
    public static float Distance(GameObject objA, GameObject objB) => Distance(objA.DisplayX, objA.DisplayY, objB.DisplayX, objB.DisplayY);

    /// <summary>
    /// Calculate angle between two points. In radians
    /// </summary>
    public static float Angle(float sourceX, float sourceY, float targetX, float targetY) => (float)Math.Atan2(targetY - sourceY, targetX - sourceX);

    /// <summary>
    /// Calculate angle between two points. In radians
    /// </summary>
    public static float Angle(Vector2 source, Vector2 target) => Angle(source.X, source.Y, target.X, target.Y);

    /// <summary>
    /// Calculate angle between two objects. In radians
    /// </summary>
    public static float Angle(GameObject objA, GameObject objB) => Angle(objA.DisplayX, objA.DisplayY, objB.DisplayX, objB.DisplayY);

    /// <summary>
    /// Calculate position on a circle, based on given radius and angle
    /// </summary>
    public static Vector2 PositionOnCircle(float radius, float angleInRadians = 0) => new Vector2(
        (float)(radius * Math.Cos(angleInRadians)),
        (float)(radius * Math.Sin(angleInRadians))
    );

    /// <summary>
    /// Calculate position on a circle, based on given radius and angle, including center of the circle
    /// </summary>
    public static Vector2 PositionOnCircle(float centerX, float centerY, float radius, float angleInRadians = 0) => new Vector2(
        (float)(radius * Math.Cos(angleInRadians)) + centerX,
        (float)(radius * Math.Sin(angleInRadians)) + centerY
    );

    /// <summary>
    /// Transforms radians to degrees
    /// </summary>
    public static float RadiansToDegrees(float radians) => (float)(radians * (180f / Math.PI));

    /// <summary>
    /// Transforms radians to degrees
    /// </summary>
    public static float DegreesToRadians(float degrees) => (float)(degrees * (Math.PI / 180));


}
