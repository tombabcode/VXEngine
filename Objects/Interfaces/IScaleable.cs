namespace VXEngine.Objects.Interfaces;

public interface IScaleable {

    GameObjectV2 SetScaleX(float scaleX);
    GameObjectV2 SetScaleY(float scaleY);
    GameObjectV2 SetScale(float scale);
    float GetScaleX( );
    float GetScaleY( );

}
