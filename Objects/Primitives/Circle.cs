using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;

namespace VXEngine.Objects.Primitives;

public class Circle : GameObject {

    /// <summary>
    /// Content reference
    /// </summary>
    private BasicContentController _content;

    /// <summary>
    /// Texture
    /// </summary>
    private Texture2D _texture = null;

    /// <summary>
    /// Diameter of the circle
    /// </summary>
    public float Diameter { get; set; } = 0;

    /// <summary>
    /// Radius of the circle
    /// </summary>
    public float Radius => Diameter == 0 ? 0 : Diameter / 2;

    /// <summary>
    /// Color of the circle's outline
    /// </summary>
    public Color ColorOutline { get; private set; } = Color.White;

    /// <summary>
    /// Color of the circle's inside
    /// </summary>
    public Color ColorFill { get; private set; } = Color.Transparent;

    public Circle(BasicContentController content, BasicInputController input, float x, float y, float diameter, float thickness = 1, Color? colorOutline = null, Color? colorFill = null) : base(input) {
        _content = content;

        ColorOutline = colorOutline ?? Color.White;
        ColorFill = colorFill ?? Color.Transparent;
        SetPosition(x, y);
        _width = (int)diameter;
        _height = (int)diameter;
        Calculate( );
    }

    private void Calculate( ) {
        // Remove old texture data
        if (_texture != null)
            _texture.Dispose( );

        // Create texture
        _texture = new Texture2D(_content.Device, (int)_width, (int)_height);
        Color[] data = new Color[_texture.Width * _texture.Height];
        for (int i = 0; i < data.Length; i++)
            data[i] = Color.Transparent;

        // Calculate
        int x = _texture.Width / 2;
        int y = 0;
        float p = 3 - 2 * Radius;
        data[y * _texture.Width + x] = ColorOutline;

        while (x != _texture.Height - y) {
            x++;

            if (p < 0) {
                p = p + 4 * x + 6;
                data[y * _texture.Width + x] = ColorOutline;
            } else {
                y++;
                p = p + 4 * (x - y) + 10;
                data[y * _texture.Width + x] = ColorOutline;
            }
        }

        // Set color data
        _texture.SetData(data);
    }

    /// <inheritdoc/>
    public override void Render(GameTime time) {
        if (OutputAlpha <= 0 || !GetVisible( ) || _width <= 0 || _height <= 0) return;

        _content.Canvas.Draw(
            _texture,
            new Vector2(DisplayX + GetRotationOriginX( ) * DisplayWidth, DisplayY + GetRotationOriginY( ) * DisplayHeight),
            new Rectangle(0, 0, _texture.Width, _texture.Height),
            Color.White,
            GetAngle( ),
            new Vector2(RotationOriginX, RotationOriginY),
            new Vector2(GetScaleX( ), GetScaleY( )),
            SpriteEffects.None,
            0
        );
    }

}
