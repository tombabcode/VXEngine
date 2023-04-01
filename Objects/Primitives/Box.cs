using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;

namespace VXEngine.Objects.Primitives;

/// <summary>
/// Primitive box
/// </summary>
public sealed class Box : GameObject {

    /// <summary>
    /// Content reference
    /// </summary>
    private BasicContentController _content;

    /// <summary>
    /// Texture
    /// </summary>
    private Texture2D _texture = null;

    /// <summary>
    /// Constructor
    /// </summary>
    public Box(BasicContentController content, BasicInputController input, float x, float y, float width, float height, Color? fillColor = null) : base(input) {
        _content = content;
        SetColor(fillColor ?? Color.White);
        SetDimensions(x, y, width, height);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public Box(BasicContentController content, BasicInputController input, float width, float height, Color? fillColor = null) : this(content, input, 0, 0, width, height, fillColor) { }

    /// <summary>
    /// Recalculate sizes and recreates texture
    /// </summary>
    private void Calculate( ) {
        // Skip if there is no change
        if (_texture != null && (int)_width == _texture.Width && (int)_height == _texture.Height)
            return;

        // Remove old texture data
        if (_texture != null)
            _texture.Dispose( );

        // Skip if it's too small
        if ((int)_width <= 0 || (int)_height <= 0) {
            _width = 0;
            _height = 0;
            return;
        }

        // Create texture
        _texture = new Texture2D(_content.Device, (int)_width, (int)_height);
        Color[] data = new Color[_texture.Width * _texture.Height];

        // Fill color data
        for (int i = 0; i < data.Length; i++)
            data[i] = Color.White;

        // Set color data
        _texture.SetData(data);
        _width = _texture.Width;
        _height = _texture.Height;
    }

    /// <inheritdoc/>
    public override void SetWidth(float width) {
        base.SetWidth(width);
        Calculate( );
    }

    /// <inheritdoc/>
    public override void SetHeight(float height) {
        base.SetHeight(height);
        Calculate( );
    }

    /// <inheritdoc/>
    public override void SetSize(float width, float height) {
        base.SetSize(width, height);
        Calculate( );
    }

    /// <inheritdoc/>
    public override void SetDimensions(float x, float y, float width, float height) {
        base.SetDimensions(x, y, width, height);
        Calculate( );
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
