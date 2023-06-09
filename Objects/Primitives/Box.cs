﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;

namespace VXEngine.Objects.Primitives;

public class Box : GameObject {

    protected BasicContentController _content { get; private set; } = null;

    public bool IsTextureRebuildRequired { get; set; } = true;

    private Texture2D _textureFill = null;

    public Color ColorFill { get; private set; } = Color.White;

    // Constructor
    public Box(BasicContentController content, BasicConfigController config, BasicInputController input) : this(content, config, input, 0, 0, 0, 0, null) { }
    public Box(BasicContentController content, BasicConfigController config, BasicInputController input, Color? color) : this(content, config, input, 0, 0, 0, 0, color) { }
    public Box(BasicContentController content, BasicConfigController config, BasicInputController input, float size, Color? color) : this(content, config, input, 0, 0, size, size, color) { }
    public Box(BasicContentController content, BasicConfigController config, BasicInputController input, float width, float height) : this(content, config, input, 0, 0, width, height, null) { }
    public Box(BasicContentController content, BasicConfigController config, BasicInputController input, float width, float height, Color? color) : this(content, config, input, 0, 0, width, height, color) { }
    public Box(BasicContentController content, BasicConfigController config, BasicInputController input, float x, float y, float width, float height, Color? color) : base(config, input) {
        _content = content;
        ColorFill = color ?? Color.White;

        OnParentChange.Add(MarkToRebuild);
        OnSizeChange.Add(MarkToRebuild);

        SetDimension(x, y, width, height);
    }

    private void MarkToRebuild( ) {
        IsTextureRebuildRequired = true;
        Children.ForEach(child => {
            if (child is Box) ((Box)child).IsTextureRebuildRequired = true;
        });
    }

    public override bool IsPointInside(float x, float y) {
        if (RenderWidth <= 0 || RenderHeight <= 0)
            return false;

        // If rectangle is not rotated - checking is simple
        if (RenderAngle == 0) {
            return x >= RenderX && x <= RenderX + RenderWidth && y >= RenderY && y <= RenderY + RenderHeight;

        // Check if point is inside rotated rectangle
        // https://stackoverflow.com/a/17146376/3389035 (Calculate area of triangles)
        // https://gamedev.stackexchange.com/a/86784 (Calculate corners positions)
        } else {
            float cos = (float)Math.Cos(RenderAngle);
            float sin = (float)Math.Sin(RenderAngle);

            float Ax = RenderX;
            float Ay = RenderY;
            float Bx = Ax + cos * RenderWidth;
            float By = Ay + sin * RenderWidth;
            float Cx = Ax + cos * RenderWidth - sin * RenderHeight;
            float Cy = Ay + sin * RenderWidth + cos * RenderHeight;
            float Dx = Ax + -sin * RenderHeight;
            float Dy = Ay + cos * RenderHeight;

            // A B C
            // A P D
            float areaAPD = Math.Abs((x * Ay - Ax * y) + (Dx * y - x * Dy) + (Ax * Dy - Dx * Ay)) / 2;

            // A B C
            // D P C
            float areaDPC = Math.Abs((x * Dy - Dx * y) + (Cx * y - x * Cy) + (Dx * Cy - Cx * Dy)) / 2;

            // A B C
            // C P B
            float areaCPB = Math.Abs((x * Cy - Cx * y) + (Bx * y - x * By) + (Cx * By - Bx * Cy)) / 2;

            // A B C
            // P B A
            float areaPBA = Math.Abs((Bx * y - x * By) + (Ax * By - Bx * Ay) + (x * Ay - Ax * y)) / 2;

            if ((int)(areaAPD + areaDPC + areaCPB + areaPBA) > (int)(RenderWidth * RenderHeight)) {
                return false;
            } else {
                return true;
            }
        }
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

    // Rebuild texture
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
        Color[] fillData = new Color[_textureFill.Width * _textureFill.Height];

        for (int i = 0; i < fillData.Length; i++)
            fillData[i] = ColorFill;

        // Set data
        _textureFill.SetData(fillData);

        // Reset flag
        IsTextureRebuildRequired = false;
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
