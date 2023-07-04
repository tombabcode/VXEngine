using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;
using VXEngine.Types;

namespace VXEngine.Objects.Primitives;

public class View : GameObject {

    protected BasicContentController _content { get; private set; } = null;

    protected RenderTarget2D _target { get; private set; } = null;

    // We want to move only view, not children, so View has separated offsets
    protected float _viewRenderX = 0;
    protected float _viewRenderY = 0;
    protected float _viewRenderAngleOriginX = 0;
    protected float _viewRenderAngleOriginY = 0;
    protected float _viewRenderAngle = 0;
    protected float _viewOpacity = 1;

    public View(BasicContentController content, BasicConfigController config, BasicInputController input, float width, float height) : this(content, config, input, 0, 0, width, height) { }
    public View(BasicContentController content, BasicConfigController config, BasicInputController input, float x, float y, float width, float height) : base(config, input) {
        _content = content;

        if (width <= 0 || height <= 0) {
            SetSize(0, 0);
        } else {
            SetSize((int)width, (int)height);
        }

        SetPosition(x, y);
    }

    // Update size
    public override void UpdateSize( ) {
        base.UpdateSize( );

        if ((int)RenderWidth <= 0 || (int)RenderHeight <= 0) {
            _target = null;
        } else {
            if (_target != null)
                _target.Dispose( );

            _target = new RenderTarget2D(_content.Device, (int)RenderWidth, (int)RenderHeight);
        }
    }

    // Update position
    public override void UpdatePosition( ) {
        _viewRenderAngle = (Parent != null ? Parent.RenderAngle : 0) + _sourceAngle;

        _viewRenderAngleOriginX = _sourceAngleOriginX * RenderWidth;
        _viewRenderAngleOriginY = _sourceAngleOriginY * RenderHeight;

        _viewRenderX = (Parent != null ? Parent.RenderX : 0) + GetX( ) * RenderScaleX - _sourceAlignOriginX * RenderWidth;
        _viewRenderY = (Parent != null ? Parent.RenderY : 0) + GetY( ) * RenderScaleY - _sourceAlignOriginY * RenderHeight;

        if (Parent != null) {
            float parentRotationPointX = Parent.RenderX + Parent.RenderAngleOriginX;
            float parentRotationPointY = Parent.RenderY + Parent.RenderAngleOriginY;

            float objectRotationPointX = _viewRenderX + _viewRenderAngleOriginX;
            float objectRotationPointY = _viewRenderY + _viewRenderAngleOriginY;

            float radius = (float)Math.Sqrt(Math.Pow(parentRotationPointX - objectRotationPointX, 2) + Math.Pow(parentRotationPointY - objectRotationPointY, 2));
            float angle = (float)Math.Atan2(objectRotationPointY - parentRotationPointY, objectRotationPointX - parentRotationPointX);

            float offsetX = (float)Math.Cos(Parent.RenderAngle + angle) * radius;
            float offsetY = (float)Math.Sin(Parent.RenderAngle + angle) * radius;

            _viewRenderX += (parentRotationPointX - objectRotationPointX) + offsetX;
            _viewRenderY += (parentRotationPointY - objectRotationPointY) + offsetY;
        }

        IsPositionUpdateRequired = false;
    }

    // Update interaction
    public override void UpdateInteraction( ) {
        // Skip if it's not interactive
        if (!_isInteractive) return;

        // Mouse is over this element now
        if (_input.IsOverInGame(_viewRenderX, _viewRenderY, RenderWidth, RenderHeight)) {
            // It's hovered for the first time
            if (!_wasPreviouslyHovered) {
                OnHoverEnter?.ForEach(e => e?.Invoke( ));
            }

            // Hover event
            OnHover?.ForEach(e => e?.Invoke( ));

            // Mark as hovered
            _wasPreviouslyHovered = true;

            // Mouse is not over this element now
        } else {
            // Was hovered, now it's not - trigger on hover exit
            if (_wasPreviouslyHovered) {
                OnHoverExit?.ForEach(e => e?.Invoke( ));
                _wasPreviouslyHovered = false;
            }
        }
    }

    // Update opacity
    public override void UpdateOpacity( ) {
        _viewOpacity = _sourceOpacity;
    }

    // Render children inside of View
    public virtual void Prepare(GameTime time) {
        if (RenderOpacity <= 0 || RenderWidth <= 0 || RenderHeight <= 0 || Children.Count == 0) return;

        _content.Device.SetRenderTarget(_target);
        _content.Device.Clear(Color.Transparent);
        _content.Canvas.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, _config.IsPixelart ? SamplerState.PointClamp : SamplerState.AnisotropicClamp, null, null, null, null);

        Children.ForEach(child => child.Render(time));

        _content.Canvas.End( );
    }

    // Display View with its rendered children onto screen
    public override void Render(GameTime time, bool renderOnlyThisObject = false) {
        if (RenderOpacity <= 0 || RenderWidth <= 0 || RenderHeight <= 0 || Children.Count == 0) return;

        _content.Canvas.Draw(
            _target,
            new Vector2(_viewRenderX + _viewRenderAngleOriginX, _viewRenderY + _viewRenderAngleOriginY),
            null,
            (RenderTint ?? Color.White) * _viewOpacity,
            _viewRenderAngle,
            new Vector2(_viewRenderAngleOriginX, _viewRenderAngleOriginY),
            Vector2.One,
            SpriteEffects.None,
            RenderDepth
        );
    }

}
