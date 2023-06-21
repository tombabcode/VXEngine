using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;

namespace VXEngine.Objects.PrimitivesOld;

/// <summary>
/// Primitive circle
/// Algorith: Midpoint Circle Algorithm (https://en.wikipedia.org/wiki/Midpoint_circle_algorithm)
/// </summary>
public class Circle : GameObjectOld {

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
    /// Diameter of the circle
    /// </summary>
    public int Diameter { get; private set; } = 0;

    /// <summary>
    /// Radius of the circle
    /// </summary>
    public float Radius => Diameter == 0 ? 0 : Diameter / 2;

    /// <summary>
    /// Thickness of the outline
    /// </summary>
    public int Thickness { get; private set; } = 0;

    /// <summary>
    /// Color of the circle's outline
    /// </summary>
    public Color ColorOutline { get; private set; } = Color.Transparent;

    /// <summary>
    /// Color of the circle's inside
    /// </summary>
    public Color ColorFill { get; private set; } = Color.Transparent;

    public Circle(BasicContentController content, BasicInputController input, float x, float y, int diameter, int thickness = 1, Color? colorOutline = null, Color? colorFill = null) : base(input) {
        _content = content;

        // Set properties
        Diameter = diameter < 1 ? 1 : diameter;
        Thickness = thickness < 0 ? 0 : thickness > (int)Radius ? (int)Radius : thickness;
        ColorOutline = colorOutline ?? Color.Transparent;
        ColorFill = colorFill ?? Color.Transparent;

        // Set position
        SetPosition(x, y);

        // Set size
        _width = Diameter;
        _height = Diameter;

        // Create textures
        Calculate( );
    }

    private void Calculate( ) {
        // Remove old texture data
        if (_textureFill != null) _textureFill.Dispose( );
        if (_textureOutline != null) _textureOutline.Dispose( );

        // Create textures
        _textureOutline = new Texture2D(_content.Device, Diameter, Diameter);
        _textureFill = new Texture2D(_content.Device, Diameter, Diameter);
        Color[] outlineData = new Color[Diameter * Diameter];
        Color[] fillData = new Color[Diameter * Diameter];
        for (int i = 0; i < outlineData.Length; i++) {
            fillData[i] = Color.Transparent;
            outlineData[i] = Color.Transparent;
        }

        // Description of the "Q" - those are parts of the circle
        //        Q8      Q1
        //          .......
        //       .     |     .
        // Q7  .    \  |  /    .  Q2
        //    .       \|/       .
        //    . -------|------- .
        //    .       /|\       .
        // Q6  .    /  |  \    .  Q3
        //       .     |     .
        //          .......
        //        Q5      Q4

        // Calculate
        int radius = (int)(Radius - 0.5f);
        int x = 0;
        int y = radius;
        int p = 1 - radius;

        // Plot (Transforms calculations into real pixel position in the color array)
        Action<bool> plot = (initial) => {
            if (initial) {
                if (Thickness == 0)
                    return;

                for (int i = 0; i < Thickness; i++) {
                    outlineData[(radius - y + i) * Diameter + radius + x] = Color.White; // Q1
                    outlineData[(radius + x) * Diameter + radius + y - i] = Color.White; // Q3
                    outlineData[(radius + y - i) * Diameter + radius - x] = Color.White; // Q5
                    outlineData[(radius - x) * Diameter + radius - y + i] = Color.White; // Q7
                }
            } else {
                if (Thickness != 0)
                    for (int i = 0; i < Thickness; i++) {
                        outlineData[(radius - y + i) * Diameter + radius + x] = Color.White; // Q1
                        outlineData[(radius - x) * Diameter + radius + y - i] = Color.White; // Q2
                        outlineData[(radius + x) * Diameter + radius + y - i] = Color.White; // Q3
                        outlineData[(radius + y - i) * Diameter + radius + x] = Color.White; // Q4
                        outlineData[(radius + y - i) * Diameter + radius - x] = Color.White; // Q5
                        outlineData[(radius + x) * Diameter + radius - y + i] = Color.White; // Q6
                        outlineData[(radius - x) * Diameter + radius - y + i] = Color.White; // Q7
                        outlineData[(radius - y + i) * Diameter + radius - x] = Color.White; // Q8
                    }

                for (int i = -x; i < x; i++) {
                    fillData[(radius - y) * Diameter + radius + i] = Color.White; // TOP
                    fillData[(radius + y) * Diameter + radius + i] = Color.White; // BOTTOM
                    fillData[(radius + i) * Diameter + radius + y] = Color.White; // RIGHT
                    fillData[(radius + i) * Diameter + radius - y] = Color.White; // LEFT
                }
            }
        };

        // Draw initial points
        plot(true);

        // Mid-point circle algorithm
        while (x < y) {
            // Calculate
            x = x + 1;
            y = y - (p >= 0 ? 1 : 0);
            p = p + 2 * (x - (p >= 0 ? y : 0)) + (p >= 0 ? 5 : 3);

            // Render
            plot(false);
        }

        // Fill inside of the circle
        int squareHalfSide = (int)Math.Floor((Diameter / Math.Sqrt(2)) / 2) - 1;
        for (int i = -squareHalfSide + 1; i < squareHalfSide; i++)
            for (int j = -squareHalfSide + 1; j < squareHalfSide; j++)
                fillData[(radius - j) * Diameter + (radius + i)] = Color.White;

        // Set color data
        _textureOutline.SetData(outlineData);
        _textureFill.SetData(fillData);
    }

    /// <inheritdoc/>
    public override void SetWidth(float width) {
        if ((int)_width == width || width < 1)
            return;
        SetDiameter((int)width);
    }

    /// <inheritdoc/>
    public override void SetHeight(float height) {
        if ((int)_height == height || height < 1)
            return;
        SetDiameter((int)height);
    }

    /// <inheritdoc/>
    public override void SetSize(float width, float height) {
        if (((int)_width == width || width < 1) && ((int)_height == height || height < 1))
            return;
        SetDiameter((int)width);
    }

    /// <inheritdoc/>
    public override void SetSize(float size) {
        if ((int)_width == size || size < 1)
            return;
        SetDiameter((int)size);
    }

    /// <summary>
    /// Sets diameter of the circle
    /// </summary>
    public void SetDiameter(int diameter) {
        if (Diameter == diameter)
            return;
        Diameter = diameter < 1 ? 1 : diameter;
        Calculate( );
    }

    /// <summary>
    /// Sets thickness of the outline
    /// </summary>
    public void SetThickness(int thickness) {
        if (Thickness == thickness)
            return;
        Thickness = thickness < 1 ? 1 : thickness > (int)Radius ? (int)Radius : thickness;
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
        if (ColorFill != Color.Transparent)
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
        if (ColorOutline != Color.Transparent && Thickness > 0)
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
