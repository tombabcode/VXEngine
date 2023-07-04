using Microsoft.Xna.Framework;
using VXEngine.Controllers;

namespace VXEngine.Objects.Primitives;

public class Container : GameObject {

    public Container(BasicConfigController config, BasicInputController input) : base(config, input) { }

    public override bool IsPointInside(float x, float y) {
        if (RenderWidth <= 0 || RenderHeight <= 0)
            return false;

        // If rectangle is not rotated - checking is simple
        if (RenderAngle == 0) {
            return x >= RenderX && x <= RenderX + RenderWidth && y >= RenderY && y <= RenderY + RenderHeight;

            // Check if point is inside rotated rectangle
            // https://stackoverflow.com/a/17146376/3389035 (Calculate area of triangles)
            // https://gamedev.stackexchange.com/a/86784 (Calculate corners positions)
        } else {
            float cos = (float)Math.Cos(RenderAngle);
            float sin = (float)Math.Sin(RenderAngle);

            float Ax = RenderX;
            float Ay = RenderY;
            float Bx = Ax + cos * RenderWidth;
            float By = Ay + sin * RenderWidth;
            float Cx = Ax + cos * RenderWidth - sin * RenderHeight;
            float Cy = Ay + sin * RenderWidth + cos * RenderHeight;
            float Dx = Ax + -sin * RenderHeight;
            float Dy = Ay + cos * RenderHeight;

            // A B C
            // A P D
            float areaAPD = Math.Abs((x * Ay - Ax * y) + (Dx * y - x * Dy) + (Ax * Dy - Dx * Ay)) / 2;

            // A B C
            // D P C
            float areaDPC = Math.Abs((x * Dy - Dx * y) + (Cx * y - x * Cy) + (Dx * Cy - Cx * Dy)) / 2;

            // A B C
            // C P B
            float areaCPB = Math.Abs((x * Cy - Cx * y) + (Bx * y - x * By) + (Cx * By - Bx * Cy)) / 2;

            // A B C
            // P B A
            float areaPBA = Math.Abs((Bx * y - x * By) + (Ax * By - Bx * Ay) + (x * Ay - Ax * y)) / 2;

            if ((int)(areaAPD + areaDPC + areaCPB + areaPBA) > (int)(RenderWidth * RenderHeight)) {
                return false;
            } else {
                return true;
            }
        }
    }

    // Render
    public override void Render(GameTime time, bool renderOnlyThisObject = false) {
        if (RenderOpacity <= 0) return;
        base.Render(time, renderOnlyThisObject);
    }

}
