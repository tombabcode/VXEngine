using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;
using VXEngine.Types;

namespace VXEngine.Objects.Primitives;

public class Box : GameObject {

    protected BasicContentController _content { get; private set; } = null;

    public bool IsTextureRebuildRequired { get; set; } = true;

    private Texture2D _textureFill = null;

    public Color ColorFill { get; private set; } = Color.Transparent;

    // Constructor
    public Box(BasicContentController content, BasicConfigController config, BasicInputController input) : this(content, config, input, null, 0, 0, 100, 100, null) { }
    public Box(BasicContentController content, BasicConfigController config, BasicInputController input, GameObject parent) : this(content, config, input, parent, 0, 0, 0, 0, null) { }
    public Box(BasicContentController content, BasicConfigController config, BasicInputController input, float width, float height) : this(content, config, input, null, 0, 0, width, height, null) { }
    public Box(BasicContentController content, BasicConfigController config, BasicInputController input, float width, float height, Color? colorFill) : this(content, config, input, null, 0, 0, width, height, colorFill) { }
    public Box(BasicContentController content, BasicConfigController config, BasicInputController input, GameObject parent, float width, float height) : this(content, config, input, parent, 0, 0, width, height, null) { }
    public Box(BasicContentController content, BasicConfigController config, BasicInputController input, GameObject parent, float width, float height, Color? colorFill) : this(content, config, input, parent, 0, 0, width, height, colorFill) { }
    public Box(BasicContentController content, BasicConfigController config, BasicInputController input, float x, float y, float width, float height) : this(content, config, input, null, x, y, width, height, null) { }
    public Box(BasicContentController content, BasicConfigController config, BasicInputController input, float x, float y, float width, float height, Color? colorFill) : this(content, config, input, null, x, y, width, height, colorFill) { }
    public Box(BasicContentController content, BasicConfigController config, BasicInputController input, GameObject parent, float x, float y, float width, float height) : this(content, config, input, parent, x, y, width, height, null) { }
    public Box(BasicContentController content, BasicConfigController config, BasicInputController input, GameObject parent, float x, float y, float width, float height, Color? colorFill) : base(config, input) {
        _content = content;

        SetParent(parent);

        // Set properties
        ColorFill = colorFill ?? Color.White;

        // Set position
        SetDimension(x, y, width, height);

        // Set events
        OnSizeChange.Add(( ) => {
            IsTextureRebuildRequired = true;
            _children.ForEach(child => {
                if (child is Box) ((Box)child).IsTextureRebuildRequired = true;
            });
        });
    }

    // Overrides
    public override GameObject SetParent(GameObject parent) {
        base.SetParent(parent);
        IsTextureRebuildRequired = true;
        return this;
    }
    public override GameObject SetWidth(float width, UnitType unit = UnitType.Pixel) {
        if (width < 0) width = 0;
        if (_width == width && _widthUnit == unit) return this;
        _widthUnit = unit;
        _width = width;
        IsSizeUpdateRequired = true;
        IsPositionUpdateRequired = true;
        IsTextureRebuildRequired = true;
        return this;
    }
    public override GameObject SetHeight(float height, UnitType unit = UnitType.Pixel) {
        if (height < 0) height = 0;
        if (_height == height && _heightUnit == unit) return this;
        _heightUnit = unit;
        _height = height;
        IsSizeUpdateRequired = true;
        IsPositionUpdateRequired = true;
        IsTextureRebuildRequired = true;
        return this;
    }
    public override GameObject SetSize(float width, float height, UnitType unit = UnitType.Pixel) {
        if (width < 0) width = 0;
        if (height < 0) height = 0;
        if (_width == width && _height == height && _widthUnit == unit && _heightUnit == unit) return this;
        _widthUnit = unit;
        _heightUnit = unit;
        _width = width;
        _height = height;
        IsSizeUpdateRequired = true;
        IsPositionUpdateRequired = true;
        IsTextureRebuildRequired = true;
        return this;
    }
    public override GameObject SetDimension(float x, float y, float width, float height) {
        if (width < 0) width = 0;
        if (height < 0) height = 0;
        if (_offsetX == x && _offsetY == y && _offsetXUnit == UnitType.Pixel && _offsetYUnit == UnitType.Pixel &&
            _width == width && _height == height && _widthUnit == UnitType.Pixel && _heightUnit == UnitType.Pixel)
            return this;
        _offsetXUnit = UnitType.Pixel;
        _offsetYUnit = UnitType.Pixel;
        _widthUnit = UnitType.Pixel;
        _heightUnit = UnitType.Pixel;
        _offsetX = x;
        _offsetY = y;
        _width = width;
        _height = height;
        IsPositionUpdateRequired = true;
        IsSizeUpdateRequired = true;
        IsTextureRebuildRequired = true;
        return this;
    }
    public override GameObject SetScaleX(float scale) {
        if (_scaleX == scale) return this;
        if (scale < 0) scale = 0;
        _scaleX = scale;
        IsSizeUpdateRequired = true;
        IsPositionUpdateRequired = true;
        IsTextureRebuildRequired = true;
        return this;
    }
    public override GameObject SetScaleY(float scale) {
        if (_scaleY == scale) return this;
        if (scale < 0) scale = 0;
        _scaleY = scale;
        IsSizeUpdateRequired = true;
        IsPositionUpdateRequired = true;
        IsTextureRebuildRequired = true;
        return this;
    }
    public override GameObject SetScale(float scale) {
        if (_scaleX == scale && _scaleY == scale) return this;
        if (scale < 0) scale = 0;
        _scaleX = scale;
        _scaleY = scale;
        IsSizeUpdateRequired = true;
        IsPositionUpdateRequired = true;
        IsTextureRebuildRequired = true;
        return this;
    }
    public override GameObject SetColor(Color? color) {
        if (_color == color) return this;
        _color = color;
        ColorFill = color ?? Color.Transparent;
        IsColorUpdateRequired = true;
        return this;
    }

    // Color
    public Box SetColorFill(Color color) {
        ColorFill = new Color(color.R, color.G, color.B, color.A);
        return this;
    }
    public Box SetColorFill(byte r, byte g, byte b, byte a = 255) {
        ColorFill = new Color(r, g, b, a);
        return this;
    }
    public Box SetColorFill(float r, float g, float b, float a = 1) {
        ColorFill = new Color(r, g, b, a);
        return this;
    }

    // Rebuild texture
    public void Rebuild( ) {
        // Remove old texture data
        if (_textureFill != null) _textureFill.Dispose( );

        // TODO If parent is rebuilt - do I need to rebuild this?

        // Prevent null size
        if (DisplayWidth <= 0 || DisplayHeight <= 0) {
            _textureFill = null;
            return;
        }

        // Create fill texture
        _textureFill = new Texture2D(_content.Device, (int)DisplayWidth, (int)DisplayHeight);
        Color[] fillData = new Color[_textureFill.Width * _textureFill.Height];
        // TODO Can be changed to x/y loop and remove pixels that are overlapping with outline - it'll overlap if opacity is < 1
        for (int i = 0; i < fillData.Length; i++)
            fillData[i] = Color.White;

        // Set data
        _textureFill.SetData(fillData);

        // Reset flag
        IsTextureRebuildRequired = false;
    }

    // Update
    public override void Update(GameTime time, bool updateOnlyThisObject = false) {
        // Updates
        if (IsColorUpdateRequired) UpdateColor(time);
        if (IsOpacityUpdateRequired) UpdateOpacity(time);
        if (IsSizeUpdateRequired) UpdateSize(time);
        if (IsPositionUpdateRequired) UpdatePosition(time);
        if (IsTextureRebuildRequired) Rebuild( );

        // Update children
        if (!updateOnlyThisObject)
            _children.ForEach(child => child.Update(time));

        // Update interaction
        UpdateInteraction(time);
    }

    // Render
    public override void Render(GameTime time, bool renderOnlyThisObject = false) {
        if (DisplayOpacity <= 0) return;
        if (DisplayWidth <= 0 || DisplayHeight <= 0) {
            base.Render(time, renderOnlyThisObject); 
            return;
        }

        // Fill texture
        if (_textureFill != null)
            _content.Canvas.Draw(
                _textureFill,
                new Vector2(DisplayX + _rotationOriginX * GetWidth( ), DisplayY + _rotationOriginY * GetHeight( )),
                null,
                ColorFill * DisplayOpacity,
                DisplayAngle,
                new Vector2(_rotationOriginX * GetWidth( ), _rotationOriginY * GetHeight( )),
                Vector2.One,
                SpriteEffects.None,
                _depth
            );

        base.Render(time, renderOnlyThisObject);
    }

}
