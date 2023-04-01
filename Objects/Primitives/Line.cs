using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;
using VXEngine.Utility;

namespace VXEngine.Objects.Primitives;

/// <summary>
/// Line 
/// </summary>
public sealed class Line : GameObject {

    /// <summary>
    /// Content reference
    /// </summary>
    private BasicContentController _content;

    /// <summary>
    /// End point of line, x axis
    /// </summary>
    public float TargetX { get; private set; } = 0;

    /// <summary>
    /// End point of line, y axis
    /// </summary>
    public float TargetY { get; private set; } = 0;

    /// <summary>
    /// Constructor
    /// </summary>
    public Line(BasicContentController content, BasicInputController input, float sourceX, float sourceY, float targetX, float targetY, float thickness = 1, Color? color = null) : base(input) {
        _content = content;

        SetPoints(sourceX, sourceY, targetX, targetY);
        SetHeight(thickness);
        SetColor(color ?? Color.White);

        _rotationOriginX = 0;
        _rotationOriginY = .5f;
    }

    /// <summary>
    /// Recalculate sizes, angles and positions
    /// </summary>
    private void Calculate( ) {
        _width = Calculations.Distance(DisplayX, DisplayY, TargetX, TargetY);
        _angle = Calculations.Angle(DisplayX, DisplayY, TargetX, TargetY);
    }

    public override void SetRotationOrigin(float rotationOrigin) => throw new NotSupportedException( );
    public override void SetRotationOrigin(float rotationOriginX, float rotationOriginY) => throw new NotSupportedException( );
    public override void SetRotationOriginX(float rotationOriginX) => throw new NotSupportedException( );
    public override void SetRotationOriginY(float rotationOriginX) => throw new NotSupportedException( );

    /// <summary>
    /// Sets both points of line
    /// </summary>
    public void SetPoints(float sourceX, float sourceY, float targetX, float targetY) {
        _x = sourceX;
        _y = sourceY;
        TargetX = targetX;
        TargetY = targetY;
        Calculate( );
    }

    /// <summary>
    /// Sets target's position, y axis
    /// </summary>
    public void SetTargetX(float x) {
        TargetX = x;
        Calculate( );
    }

    /// <summary>
    /// Sets target's position, y axis
    /// </summary>
    public void SetTargetY(float y) {
        TargetY = y;
        Calculate( );
    }

    /// <summary>
    /// Sets target's position, both x and y axis
    /// </summary>
    public void SetTargetPosition(float xy) {
        TargetX = xy;
        TargetY = xy;
        Calculate( );
    }

    /// <summary>
    /// Sets target's position, both x and y axis
    /// </summary>
    public void SetTargetPosition(float x, float y) {
        TargetX = x;
        TargetY = y;
        Calculate( );
    }

    /// <inheritdoc/>
    public override void SetX(float x) {
        base.SetX(x);
        Calculate( );
    }

    /// <inheritdoc/>
    public override void SetY(float y) {
        base.SetY(y);
        Calculate( );
    }

    /// <inheritdoc/>
    public override void SetPosition(float x, float y) {
        base.SetPosition(x, y);
        Calculate( );
    }

    /// <inheritdoc/>
    public override void SetWidth(float width) {
        base.SetWidth(width);
        Vector2 target = Calculations.PositionOnCircle(_width, GetAngle( ));
        TargetX = target.X;
        TargetY = target.Y;
        Calculate( );
    }

    /// <inheritdoc/>
    public override void SetSize(float width, float height) {
        base.SetSize(width, height);
        Vector2 target = Calculations.PositionOnCircle(_width, GetAngle( ));
        TargetX = target.X;
        TargetY = target.Y;
        Calculate( );
    }

    /// <inheritdoc/>
    public override void SetDimensions(float x, float y, float width, float height) {
        base.SetPosition(x, y);
        base.SetWidth(width);
        base.SetHeight(height);
        Vector2 target = Calculations.PositionOnCircle(_width, GetAngle( ));
        TargetX = target.X;
        TargetY = target.Y;
        Calculate( );
    }

    /// <inheritdoc/>
    public override void Render(GameTime time) {
        if (OutputAlpha <= 0 || !GetVisible( ) || _width <= 0 || _height <= 0) return;

        _content.Canvas.Draw(
            _content.TexturePixel.Image,
            new Vector2(DisplayX, DisplayY),
            null,
            OutputColor,
            GetAngle( ),
            new Vector2(RotationOriginX / GetWidth( ), RotationOriginY / GetHeight( )),
            new Vector2(DisplayWidth, DisplayHeight),
            SpriteEffects.None,
            0
        );
    }

}
