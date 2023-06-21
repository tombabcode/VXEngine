namespace VXEngine.Objects.Interfaces;

public interface IRotateable {

    GameObject SetAngleInRadians(float angle);
    GameObject SetAngleInDegrees(float angle);
    GameObject SetRotationOriginX(float originX);
    GameObject SetRotationOriginY(float originY);
    GameObject SetRotationOrigin(float originX, float originY);
    GameObject SetRotationOrigin(float origin);
    float GetAngleInRadians( );
    float GetAngleInDegrees( );
    float GetRotationOriginX( );
    float GetRotationOriginY( );

    void AddAngleInRadians(float angle);
    void AddAngleInDegrees(float angle);

}
