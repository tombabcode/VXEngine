using VXEngine.Types;
using static VXEngine.Controllers.BasicInputController;

namespace VXEngine.Objects.Interfaces;

public interface IPositionable {

    float GetX( );
    float GetY( );

    GameObject SetX(float x, UnitType unit = UnitType.Pixel);
    GameObject SetY(float y, UnitType unit = UnitType.Pixel);
    GameObject SetPosition(float x, float y, UnitType unit = UnitType.Pixel);
    GameObject SetPosition(MouseData mouse);

    void AddX(float x, UnitType unit = UnitType.Pixel);
    void AddY(float y, UnitType unit = UnitType.Pixel);
    void AddPosition(float x, float y, UnitType unit = UnitType.Pixel);
    void AddPosition(float position, UnitType unit = UnitType.Pixel);

}
