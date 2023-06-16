using Microsoft.Xna.Framework;

namespace VXEngine.Objects.Interfaces;
public interface IColorable {

    GameObjectV2 SetColor(Color? color);
    Color? GetColor( );

}
