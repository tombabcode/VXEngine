using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;

namespace VXEngine.Objects.Primitives;

/// <summary>
/// Primitive box with outline only (border)
/// </summary>
public sealed class Border : GameObject {

    /// <summary>
    /// Content reference
    /// </summary>
    private BasicContentController _content;

    /// <summary>
    /// Texture
    /// </summary>
    private Texture2D _texture = null;

    /// <summary>
    /// Thickness of the border
    /// </summary>
    public int Thickness { get; private set; } = -1;

    /// <summary>
    /// Constructor
    /// </summary>
    public Border(BasicContentController content, BasicInputController input, float x, float y, float width, float height, int size = 1, Color? color = null) : base(input) {
        _content = content;

        _width = width;
        _height = height;
        SetThickness(size < 1 ? 1 : size);
        SetPosition(x, y);
        SetColor(color ?? Color.White);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public Border(BasicContentController content, BasicInputController input, float width, float height, int size = 1, Color? color = null) : this(content, input, 0, 0, width, height, size, color) { }

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
        for (int y = 0; y < _texture.Height; y++)
            for (int x = 0; x < _texture.Width; x++)
                data[y * _texture.Width + x] = x < Thickness || y < Thickness || x >= _texture.Width - Thickness || y >= _texture.Height - Thickness ? Color.White : Color.Transparent;

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

    /// <summary>
    /// Sets thickness of the border walls
    /// </summary>
    public void SetThickness(int thickness) {
        if (Thickness == thickness)
            return;

        if (thickness <= 0) thickness = 1;
        if (thickness >= (int)(_width / 2)) thickness = (int)(_width / 2);
        if (thickness >= (int)(_height / 2)) thickness = (int)(_height / 2);

        Thickness = thickness;
        Calculate( );
    }

    /// <inheritdoc/>
    public override void Render(GameTime time) {
        if (OutputAlpha <= 0 || !GetVisible( ) || _texture == null) return;

        _content.Canvas.Draw(
            _texture,
            new Vector2(DisplayX + GetRotationOriginX( ) * DisplayWidth, DisplayY + GetRotationOriginY( ) * DisplayHeight),
            new Rectangle(0, 0, _texture.Width, _texture.Height),
            OutputColor,
            GetAngle( ),
            new Vector2(RotationOriginX, RotationOriginY),
            new Vector2(GetScaleX( ), GetScaleY( )),
            SpriteEffects.None,
            0
        );
    }
}
