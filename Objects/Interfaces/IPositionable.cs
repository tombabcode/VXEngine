using VXEngine.Controllers;

namespace VXEngine.Objects.Interfaces;

public interface IPositionable {

    float GetX( );
    float GetY( );

    GameObject SetX(float x);
    GameObject SetY(float y);
    GameObject SetPosition(float x, float y);

    GameObject SetXAsPercent(float x);
    GameObject SetYAsPercent(float y);
    GameObject SetPositionAsPercent(float x, float y);

    GameObject SetPosition(BasicInputController.MouseData mouse);

    void AddX(float x);
    void AddY(float y);
    void AddPosition(float x, float y);
    void AddPosition(float position);

    void AddXAsPercent(float x);
    void AddYAsPercent(float y);
    void AddPositionAsPercent(float x, float y);
    void AddPositionAsPercent(float position);

}
