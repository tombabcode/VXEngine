using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;

namespace VXEngine.Objects.PrimitivesOld;

/// <summary>
/// Primitive box
/// </summary>
public sealed class Box : GameObjectOld {

    /// <summary>
    /// Content reference
    /// </summary>
    private BasicContentController _content;

    /// <summary>
    /// Texture for circle's outline
    /// </summary>
    private Texture2D _textureOutline = null;

    /// <summary>
    /// Texture for circle's fill
    /// </summary>
    private Texture2D _textureFill = null;

    /// <summary>
    /// Thickness of the outline
    /// </summary>
    public int Thickness { get; private set; } = 0;

    /// <summary>
    /// Color of the box's outline
    /// </summary>
    public Color ColorOutline { get; private set; } = Color.Transparent;

    /// <summary>
    /// Color of the box's inside
    /// </summary>
    public Color ColorFill { get; private set; } = Color.Transparent;

    /// <summary>
    /// Constructor
    /// </summary>
    public Box(BasicContentController content, BasicInputController input, float x, float y, float width, float height, int thickness = 1, Color? colorOutline = null, Color? colorFill = null) : base(input) {
        _content = content;

        // Set properties
        ColorOutline = colorOutline ?? Color.Transparent;
        ColorFill = colorFill ?? Color.Transparent;
        Thickness = width > height
            ? thickness < 0 ? 0 : thickness > (int)height ? (int)height : thickness
            : thickness < 0 ? 0 : thickness > (int)width ? (int)width : thickness;

        // Set position
        SetDimensions(x, y, width, height);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public Box(BasicContentController content, BasicInputController input, float width, float height, int thickness = 1, Color? colorOutline = null, Color? colorFill = null) : this(content, input, 0, 0, width, height, thickness, colorOutline, colorFill) { }

    /// <summary>
    /// Recalculate sizes and recreates texture
    /// </summary>
    private void Calculate( ) {
        // Remove old texture data
        if (_textureFill != null) _textureFill.Dispose( );
        if (_textureOutline != null) _textureOutline.Dispose( );

        if (_width <= 0 || _height <= 0) {
            _textureOutline = null;
            _textureFill = null;
            _width = 0;
            _height = 0;
            return;
        }

        // Create texture
        _textureOutline = new Texture2D(_content.Device, (int)_width, (int)_height);
        _textureFill = new Texture2D(_content.Device, (int)_width, (int)_height);
        Color[] outlineData = new Color[_textureOutline.Width * _textureOutline.Height];
        Color[] fillData = new Color[_textureFill.Width * _textureFill.Height];

        // Fill color data
        for (int i = 0; i < fillData.Length; i++)
            fillData[i] = Color.White;

        // Fill outline
        if (Thickness <= 0)
            for (int i = 0; i < outlineData.Length; i++)
                outlineData[i] = Color.White;
        else
            for (int y = 0; y < _textureFill.Height; y++)
                for (int x = 0; x < _textureFill.Width; x++)
                    outlineData[y * _textureFill.Width + x] = x < Thickness || y < Thickness || x >= _textureFill.Width - Thickness || y >= _textureFill.Height - Thickness ? Color.White : Color.Transparent;

        // Set color data
        _textureOutline.SetData(outlineData);
        _textureFill.SetData(fillData);

        // Update sizes (round, ex. width of 50.123 will be 50.0)
        _width = _textureFill.Width;
        _height = _textureFill.Height;
    }

    /// <summary>
    /// Sets thickness of the outline
    /// </summary>
    public void SetThickness(int thickness) {
        if (Thickness == thickness)
            return;

        // Set thickness
        Thickness = _width > _height
            ? thickness < 0 ? 0 : thickness > (int)_height ? (int)_height : thickness
            : thickness < 0 ? 0 : thickness > (int)_width ? (int)_width : thickness;

        // Rebuild textures
        Calculate( );
    }

    /// <inheritdoc/>
    public override void SetWidth(float width) {
        if (_width == width)
            return;
        base.SetWidth(width);
        Calculate( );
    }

    /// <inheritdoc/>
    public override void SetHeight(float height) {
        if (_height == height)
            return;
        base.SetHeight(height);
        Calculate( );
    }

    /// <inheritdoc/>
    public override void SetSize(float width, float height) {
        if (_width == width && _height == height)
            return;
        base.SetSize(width, height);
        Calculate( );
    }

    /// <inheritdoc/>
    public override void SetDimensions(float x, float y, float width, float height) {
        if (_width == width && _height == height) {
            SetPosition(x, y);
            return;
        }

        base.SetDimensions(x, y, width, height);
        Calculate( );
    }

    /// <summary>
    /// Sets fill color of the circle
    /// </summary>
    public void SetColorFill(Color color) => SetColorFill(color.R, color.G, color.B, color.A);

    /// <summary>
    /// Sets fill color of the circle
    /// </summary>
    public void SetColorFill(byte r, byte g, byte b, byte a = 255) {
        ColorFill = new Color(r, g, b, a);
    }

    /// <summary>
    /// Sets outline color of the circle
    /// </summary>
    public void SetColorOutline(Color color) => SetColorOutline(color.R, color.G, color.B, color.A);

    /// <summary>
    /// Sets outline color of the circle
    /// </summary>
    public void SetColorOutline(byte r, byte g, byte b, byte a = 255) {
        ColorOutline = new Color(r, g, b, a);
    }

    /// <inheritdoc/>
    public override void SetColor(byte r, byte g, byte b, byte a = 255) {
        ColorFill = new Color(r, g, b, a);
        ColorOutline = new Color(r, g, b, a);
    }

    /// <inheritdoc/>
    public override void Render(GameTime time) {
        if (OutputAlpha <= 0 || !GetVisible( ) || _width <= 0 || _height <= 0) return;

        // Display fill
        if (ColorFill != Color.Transparent && _textureFill != null)
            _content.Canvas.Draw(
                _textureFill,
                new Vector2(DisplayX + GetRotationOriginX( ) * DisplayWidth, DisplayY + GetRotationOriginY( ) * DisplayHeight),
                new Rectangle(0, 0, _textureFill.Width, _textureFill.Height),
                ColorFill,
                GetAngle( ),
                new Vector2(RotationOriginX, RotationOriginY),
                new Vector2(GetScaleX( ), GetScaleY( )),
                SpriteEffects.None,
                0
            );

        // Display outline
        if (ColorOutline != Color.Transparent && Thickness > 0 && _textureOutline != null)
            _content.Canvas.Draw(
                _textureOutline,
                new Vector2(DisplayX + GetRotationOriginX( ) * DisplayWidth, DisplayY + GetRotationOriginY( ) * DisplayHeight),
                new Rectangle(0, 0, _textureOutline.Width, _textureOutline.Height),
                ColorOutline,
                GetAngle( ),
                new Vector2(RotationOriginX, RotationOriginY),
                new Vector2(GetScaleX( ), GetScaleY( )),
                SpriteEffects.None,
                0
            );
    }
}
