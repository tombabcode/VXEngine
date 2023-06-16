using VXEngine.Types;
using static VXEngine.Controllers.BasicInputController;

namespace VXEngine.Objects.Interfaces;

public interface IPositionable {

    float GetX( );
    float GetY( );

    GameObjectV2 SetX(float x, UnitType unit = UnitType.Pixel);
    GameObjectV2 SetY(float y, UnitType unit = UnitType.Pixel);
    GameObjectV2 SetPosition(float x, float y, UnitType unit = UnitType.Pixel);
    GameObjectV2 SetPosition(MouseData mouse);

    void AddX(float x, UnitType unit = UnitType.Pixel);
    void AddY(float y, UnitType unit = UnitType.Pixel);
    void AddPosition(float x, float y, UnitType unit = UnitType.Pixel);
    void AddPosition(float position, UnitType unit = UnitType.Pixel);

}
