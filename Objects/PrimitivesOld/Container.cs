using Microsoft.Xna.Framework;
using VXEngine.Controllers;

namespace VXEngine.Objects.PrimitivesOld; 

/// <summary>
/// Non-visible, container that holds other elements
/// </summary>
public class Container : GameObjectOld {

    /// <summary>
    /// Constructor
    /// </summary>
    public Container(BasicInputController input) : base(input) { }

    public Rectangle GetBoundingBox( ) {
        if (_children == null || _children.Count == 0)
            return new Rectangle((int)_x, (int)_y, (int)_width, (int)_height);

        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (GameObjectOld child in _children) {
            if (child.GetX( ) < minX) { minX = (int)child.GetX( ); }
            if (child.GetY( ) < minY) { minY = (int)child.GetY( ); }
            if (child.GetX( ) + child.DisplayWidth > maxX) { maxX = (int)child.GetX( ); }
            if (child.GetY( ) + child.DisplayHeight > maxY) { maxY = (int)child.GetY( ); }
        }

        return new Rectangle(minX, minY, maxX - minX, maxY - minY);
    }

}
