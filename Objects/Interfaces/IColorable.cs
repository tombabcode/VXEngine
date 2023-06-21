using Microsoft.Xna.Framework;

namespace VXEngine.Objects.Interfaces;
public interface IColorable {

    GameObject SetColor(Color? color);
    Color? GetColor( );

}
