namespace VXEngine.Objects.Interfaces;

public interface IRotateable {

    GameObjectV2 SetAngle(float angle);
    GameObjectV2 SetRotationOriginX(float originX);
    GameObjectV2 SetRotationOriginY(float originY);
    GameObjectV2 SetRotationOrigin(float originX, float originY);
    GameObjectV2 SetRotationOrigin(float origin);
    float GetAngle( );
    float GetRotationOriginX( );
    float GetRotationOriginY( );

    void AddAngle(float angle);

}
