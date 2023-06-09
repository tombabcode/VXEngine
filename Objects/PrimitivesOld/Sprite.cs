﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;
using VXEngine.Textures;

namespace VXEngine.Objects.PrimitivesOld;

/// <summary>
/// Textured object
/// </summary>
public sealed class Sprite : GameObjectOld {

    /// <summary>
    /// Content reference
    /// </summary>
    private BasicContentController _content;

    /// <summary>
    /// Texture instance
    /// </summary>
    private TextureInstance _texture;

    /// <summary>
    /// Constructor
    /// </summary>
    public Sprite(BasicContentController content, BasicInputController input, TextureInstance texture, float x = 0, float y = 0) : base(input) {
        _content = content;
        _texture = texture;
        Rectangle source = _texture.GetSource( );
        SetDimensions(x, y, source.Width, source.Height);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public Sprite(BasicContentController content, BasicInputController input, TextureBase data, float x = 0, float y = 0) : this(content, input, data.GetInstance( ), x, y) { }

    /// <summary>
    /// Display sprite
    /// </summary>
    public override void Render(GameTime time) {
        if (OutputAlpha <= 0 || !GetVisible( ) || _texture == null || DisplayWidth <= 0 || DisplayHeight <= 0)
            return;

        _content.Canvas.Draw(
            _texture.Data.Image,
            new Vector2(DisplayX + GetRotationOriginX( ) * DisplayWidth, DisplayY + GetRotationOriginY( ) * DisplayHeight),
            _texture.GetSource( ),
            OutputColor,
            GetAngle( ),
            new Vector2(RotationOriginX, RotationOriginY),
            new Vector2(GetScaleX( ), GetScaleY( )),
            SpriteEffects.None,
            0
        );
    }

}
