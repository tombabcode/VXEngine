using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using VXEngine.Controllers;
using VXEngine.Textures;
using VXEngine.Types;

namespace VXEngine.Objects.Primitives;

public class Sprite : GameObject {

    protected BasicContentController _content { get; private set; } = null;

    // Texture clip
    protected float _sourceX = 0;
    protected float _sourceY = 0;
    protected float _sourceWidth = -1;
    protected float _sourceHeight = -1;
    protected UnitType _sourceXUnit = UnitType.Pixel;
    protected UnitType _sourceYUnit = UnitType.Pixel;
    protected UnitType _sourceWidthUnit = UnitType.Pixel;
    protected UnitType _sourceHeightUnit = UnitType.Pixel;
    protected Rectangle? _source = null;

    public TextureInstance Texture { get; private set; } = null;

    public Sprite(BasicContentController content, BasicConfigController config, BasicInputController input, GameObject parent, TextureInstance texture, float x, float y, float width, float height) : base(config, input) {
        _content = content;

        Texture = texture;

        SetParent(parent);

        if (width <= 0 || height <= 0) {
            if (Texture != null) {
                SetDimension(x, y, Texture.Data.FrameWidth, Texture.Data.FrameHeight);
            } else {
                SetDimension(x, y, 0, 0);
            }
        } else {
            SetDimension(x, y, width, height);
        }
    }
    public Sprite(BasicContentController content, BasicConfigController config, BasicInputController input, GameObject parent, TextureBase texture, float x, float y, float width, float height)
        : this(content, config, input, parent, texture?.GetInstance( ), x, y, width, height) { }
    public Sprite(BasicContentController content, BasicConfigController config, BasicInputController input, GameObject parent, TextureInstance texture, float width, float height)
        : this(content, config, input, parent, texture, 0, 0, width, height) { }
    public Sprite(BasicContentController content, BasicConfigController config, BasicInputController input, GameObject parent, TextureBase texture, float width, float height)
        : this(content, config, input, parent, texture?.GetInstance( ), 0, 0, width, height) { }
    public Sprite(BasicContentController content, BasicConfigController config, BasicInputController input, GameObject parent, TextureInstance texture)
        : this(content, config, input, parent, texture, 0, 0, -1, -1) { }
    public Sprite(BasicContentController content, BasicConfigController config, BasicInputController input, TextureBase texture, float x, float y, float width, float height)
        : this(content, config, input, null, texture?.GetInstance( ), x, y, width, height) { }
    public Sprite(BasicContentController content, BasicConfigController config, BasicInputController input, TextureInstance texture, float width, float height)
        : this(content, config, input, null, texture, 0, 0, width, height) { }
    public Sprite(BasicContentController content, BasicConfigController config, BasicInputController input, TextureBase texture, float width, float height)
        : this(content, config, input, null, texture?.GetInstance( ), 0, 0, width, height) { }
    public Sprite(BasicContentController content, BasicConfigController config, BasicInputController input, TextureInstance texture)
        : this(content, config, input, null, texture, 0, 0, -1, -1) { }

    // Texture clip
    public virtual void SetClip(float x, float y, float width, float height) {
        if (Texture == null || width <= 0 || height <= 0) {
            _source = null;
            return;
        }

        if (width > Texture.Data.FrameWidth) width = Texture.Data.FrameWidth;
        if (height > Texture.Data.FrameHeight) height = Texture.Data.FrameHeight;
        if (x < 0) x = 0;
        if (y < 0) y = 0;
        if (x > Texture.Data.FrameWidth - width) x = Texture.Data.FrameWidth - width;
        if (y > Texture.Data.FrameHeight - height) y = Texture.Data.FrameHeight - height;

        _source = new Rectangle((int)x, (int)y, (int)width, (int)height);
    }
    public virtual void SetClip(Rectangle? source) {
        if (source == null) {
            _source = null;
            return;
        }

        SetClip(source.Value.X, source.Value.Y, source.Value.Width, source.Value.Height);
    }
    public virtual void SetClipAsPercentage(float x, float y, float width, float height) {
        if (Texture == null || width <= 0 || height <= 0) {
            _source = null;
            return;
        }

        if (width > 1) width = 1;
        if (height > 1) height = 1;
        if (x < 0) x = 0;
        if (y < 0) y = 0;
        if (x > 1 - width) x = 1 - width;
        if (y > 1 - height) y = 1 - height;

        _source = new Rectangle((int)(x * Texture.Data.FrameWidth), (int)(y * Texture.Data.FrameHeight), (int)(width * Texture.Data.FrameWidth), (int)(height * Texture.Data.FrameHeight));
    }
    public virtual void SetClipAsPercentage(Rectangle? source) {
        if (source == null) {
            _source = null;
            return;
        }

        SetClipAsPercentage(source.Value.X, source.Value.Y, source.Value.Width, source.Value.Height);
    }

    // Render
    public override void Render(GameTime time, bool renderOnlyThisObject = false) {
        if (DisplayOpacity <= 0) return;
        if (DisplayWidth <= 0 || DisplayHeight <= 0 || Texture == null) {
            base.Render(time, renderOnlyThisObject);
            return;
        }

        if (Texture != null)
            _content.Canvas.Draw(
                Texture.Data.Image,
                new Vector2(DisplayX + _rotationOriginX * GetWidth( ), DisplayY + _rotationOriginY * GetHeight( )),
                _source,
                (DisplayColor ?? Color.White) * DisplayOpacity,
                MathHelper.ToRadians(DisplayAngle),
                new Vector2(_rotationOriginX * GetWidth( ), _rotationOriginY * GetHeight( )),
                new Vector2(
                    DisplayWidth / (_source != null ? _source.Value.Width : Texture.Data.FrameWidth),
                    DisplayHeight / (_source != null ? _source.Value.Height : Texture.Data.FrameHeight)
                ),
                SpriteEffects.None,
                _depth
            );

        base.Render(time, renderOnlyThisObject);
    }

}
