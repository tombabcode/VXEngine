using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using VXEngine.Controllers;
using VXEngine.Textures;

namespace VXEngine.Objects.Primitives;

public class Sprite : GameObject {

    protected BasicContentController _content { get; private set; } = null;

    public TextureInstance Texture { get; private set; } = null;

    public Sprite(BasicContentController content, BasicConfigController config, BasicInputController input, TextureInstance texture) : this(content, config, input, 0, 0, texture) { }
    public Sprite(BasicContentController content, BasicConfigController config, BasicInputController input, float x, float y, TextureInstance texture) : base(config, input) {
        _content = content;

        Texture = texture;

        SetDimension(x, y, Texture.Data.FrameWidth, Texture.Data.FrameHeight);
    }

    // Render
    public override void Render(GameTime time, bool renderOnlyThisObject = false) {
        if (RenderOpacity <= 0) return;
        if (RenderWidth <= 0 || RenderHeight <= 0 || Texture == null) {
            base.Render(time, renderOnlyThisObject);
            return;
        }

        _content.Canvas.Draw(
            Texture.Data.Image,
            new Vector2(RenderX + RenderAngleOriginX, RenderY + RenderAngleOriginY),
            null,
            (RenderTint ?? Color.White) * RenderOpacity,
            RenderAngle,
            new Vector2(RenderAngleOriginX / RenderScaleX, RenderAngleOriginY / RenderScaleY),
            new Vector2(RenderScaleX * (GetWidth( ) / Texture.Data.FrameWidth), RenderScaleY * (GetHeight( ) / Texture.Data.FrameHeight)),
            SpriteEffects.None,
            RenderDepth
        );

        base.Render(time, renderOnlyThisObject);
    }

}
