using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;
using VXEngine.Types;

namespace VXEngine.Objects.Primitives;

public class Circle : GameObject {

    protected BasicContentController _content { get; private set; } = null;

    public bool IsTextureRebuildRequired { get; set; } = true;

    private Texture2D _textureFill = null;

    public int Radius { get; private set; } = 0;

    public Color ColorFill { get; private set; } = Color.White;

    public Circle(BasicContentController content, BasicConfigController config, BasicInputController input) : this(content, config, input, 0, 0, 0, null) { }
    public Circle(BasicContentController content, BasicConfigController config, BasicInputController input, Color? color) : this(content, config, input, 0, 0, 0, color) { }
    public Circle(BasicContentController content, BasicConfigController config, BasicInputController input, int radius) : this(content, config, input, 0, 0, radius, null) { }
    public Circle(BasicContentController content, BasicConfigController config, BasicInputController input, int radius, Color? color) : this(content, config, input, 0, 0, radius, color) { }
    public Circle(BasicContentController content, BasicConfigController config, BasicInputController input, float x, float y, int radius) : this(content, config, input, x, y, radius, null) { }
    public Circle(BasicContentController content, BasicConfigController config, BasicInputController input, float x, float y, int radius, Color? color) : base(config, input) {
        _content = content;
        SetRadius(radius);
        SetSize(radius * 2);
        SetPosition(x, y);
        ColorFill = color ?? Color.White;

        OnParentChange.Add(MarkToRebuild);
        OnSizeChange.Add(MarkToRebuild);
    }

    private void MarkToRebuild( ) {
        IsTextureRebuildRequired = true;
        Children.ForEach(child => {
            if (child is Box) ((Box)child).IsTextureRebuildRequired = true;
        });
    }

    // Overrides
    public override float GetWidth( ) => Radius * 2;
    public override float GetHeight( ) => Radius * 2;
    protected override GameObject SetWidth(float width, UnitType unit) {
        if (width < 0) width = 0;
        Radius = (int)((unit == UnitType.Pixel ? width : (Parent != null ? Parent.GetWidth( ) * width : _config.ViewWidth * width)) / 2);
        if (Radius < 0) Radius = 0;
        IsTextureRebuildRequired = true;
        IsSizeUpdateRequired = true;
        IsPositionUpdateRequired = true;
        return this;
    }
    protected override GameObject SetHeight(float height, UnitType unit) {
        if (height < 0) height = 0;
        Radius = (int)((unit == UnitType.Pixel ? height : (Parent != null ? Parent.GetHeight( ) * height : _config.ViewHeight * height)) / 2);
        if (Radius < 0) Radius = 0;
        IsTextureRebuildRequired = true;
        IsSizeUpdateRequired = true;
        IsPositionUpdateRequired = true;
        return this;
    }

    // Color
    public GameObject SetColorFill(Color color) {
        ColorFill = new Color(color.R, color.G, color.B, color.A);
        IsTextureRebuildRequired = true;
        return this;
    }
    public GameObject SetColorFill(byte r, byte g, byte b, byte a = 255) {
        ColorFill = new Color(r, g, b, a);
        IsTextureRebuildRequired = true;
        return this;
    }
    public GameObject SetColorFill(float r, float g, float b, float a = 1) {
        ColorFill = new Color(r, g, b, a);
        IsTextureRebuildRequired = true;
        return this;
    }

    // Radius
    public GameObject SetRadius(int radius) {
        Radius = radius < 0 ? 0 : radius;
        SetSize(Radius * 2);
        IsTextureRebuildRequired = true;
        return this;
    }

    public void Rebuild( ) {
        // Skip if there is no need to rebuild
        if (_textureFill != null && _textureFill.Width == (int)GetWidth( ) && _textureFill.Height == (int)GetHeight( ))
            return;

        // Remove old texture data
        if (_textureFill != null) _textureFill.Dispose( );

        // Prevent null size
        if ((int)GetWidth( ) <= 0 || (int)GetHeight( ) <= 0) {
            _textureFill = null;
            return;
        }

        // Create fill texture
        _textureFill = new Texture2D(_content.Device, (int)GetWidth( ), (int)GetHeight( ));
        Color[] fillData = Enumerable.Repeat(Color.Transparent, _textureFill.Width * _textureFill.Height).ToArray( );

        for (int x = -Radius; x < Radius; x++) {
            int height = (int)Math.Sqrt(Radius * Radius - x * x);

            for (int y = -height; y < height; y++) {
                int posX = x + _textureFill.Width / 2;
                int posY = y + _textureFill.Height / 2;
                if (posX >= 0 && posY >= 0 && posX < _textureFill.Width && posY < _textureFill.Height)
                    fillData[posY * _textureFill.Height + posX] = ColorFill;
            }
        }

        // Set data
        _textureFill.SetData(fillData);

        // Reset flag
        IsTextureRebuildRequired = false;
    }

    public override bool IsPointInside(float x, float y) {
        if (Radius <= 0)
            return false;

        float distance = (float)Math.Sqrt(Math.Pow(x - (RenderX + RenderWidth / 2), 2) + Math.Pow(y - (RenderY + RenderHeight / 2), 2));
        return distance <= Radius * RenderScaleX;
    }

    // Update
    public override void Update(GameTime time, bool updateOnlyThisObject = false) {
        if (IsSizeUpdateRequired) UpdateSize( );
        if (IsPositionUpdateRequired) UpdatePosition( );
        if (IsOpacityUpdateRequired) UpdateOpacity( );
        if (IsDepthUpdateRequired) UpdateDepth( );
        if (IsTintUpdateRequired) UpdateColor( );

        if (IsTextureRebuildRequired) Rebuild( );

        // Update children
        if (!updateOnlyThisObject)
            Children.ForEach(child => child.Update(time));

        // Update interaction
        UpdateInteraction( );
    }

    // Render
    public override void Render(GameTime time, bool renderOnlyThisObject = false) {
        if (RenderOpacity <= 0) return;
        if (RenderWidth <= 0 || RenderHeight <= 0 || _textureFill == null) {
            base.Render(time, renderOnlyThisObject);
            return;
        }

        _content.Canvas.Draw(
            _textureFill,
            new Vector2(RenderX + RenderAngleOriginX, RenderY + RenderAngleOriginY),
            null,
            (RenderTint ?? Color.White) * RenderOpacity,
            RenderAngle,
            new Vector2(RenderAngleOriginX / RenderScaleX, RenderAngleOriginY / RenderScaleY),
            new Vector2(RenderScaleX, RenderScaleY),
            SpriteEffects.None,
            RenderDepth
        );

        base.Render(time, renderOnlyThisObject);
    }

}
