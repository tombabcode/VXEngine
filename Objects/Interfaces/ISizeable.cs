using VXEngine.Types;

namespace VXEngine.Objects.Interfaces;

public interface ISizeable {

    float GetWidth( );
    float GetHeight( );

    GameObject SetWidth(float width, UnitType unit = UnitType.Pixel);
    GameObject SetHeight(float height, UnitType unit = UnitType.Pixel);
    GameObject SetSize(float size, UnitType unit = UnitType.Pixel);
    GameObject SetSize(float width, float height, UnitType unit = UnitType.Pixel);

    void AddWidth(float value, UnitType unit = UnitType.Pixel);
    void AddHeight(float value, UnitType unit = UnitType.Pixel);
    void AddSize(float width, float height, UnitType unit = UnitType.Pixel);
    void AddSize(float size, UnitType unit = UnitType.Pixel);

}
