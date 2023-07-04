namespace VXEngine.Objects.Interfaces;

/// <summary>
/// Allows object to rotate around origin point
/// </summary>
public interface IRotateable {

    /// <summary>
    /// Sets object's rotation in radians
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    GameObject SetAngleInRadians(float angle);

    /// <summary>
    /// Sets object's rotation in degrees
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    GameObject SetAngleInDegrees(float angle);

    /// <summary>
    /// Sets object's rotation origin point's X position as the normalized value of the object, where 0 means left side and 1 right side of the object
    /// </summary>
    /// <param name="originX"></param>
    /// <returns></returns>
    GameObject SetRotationOriginX(float originX);

    /// <summary>
    /// Sets object's rotation origin point's Y position as normalized value of the object, where 0 means top side and 1 bottom side of the object
    /// </summary>
    /// <param name="originY"></param>
    /// <returns></returns>
    GameObject SetRotationOriginY(float originY);

    /// <summary>
    /// Sets object's rotation origin points as normalized valeus of the object
    /// </summary>
    /// <param name="originX"></param>
    /// <param name="originY"></param>
    /// <returns></returns>
    GameObject SetRotationOrigin(float originX, float originY);

    /// <summary>
    /// Sets object's rotation origin point for both axis (X and Y) as normalized values of the object
    /// </summary>
    /// <param name="origin"></param>
    /// <returns></returns>
    GameObject SetRotationOrigin(float origin);

    float GetAngleInRadians( );
    float GetAngleInDegrees( );
    float GetRotationOriginX( );
    float GetRotationOriginY( );

    void AddAngleInRadians(float angle);
    void AddAngleInDegrees(float angle);

}
