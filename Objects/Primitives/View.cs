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
    protected float _viewRenderAngle = 0;
    protected float _viewRenderAngleOffsetX = 0;
    protected float _viewRenderAngleOffsetY = 0;
    protected float _targetOpacity = 1;

    public View(BasicContentController content, BasicConfigController config, BasicInputController input, GameObject parent, float x, float y, float width, float height) : base(config, input) {
        _content = content;

        SetParent(parent);

        if (width <= 0 || height <= 0) {
            SetSize(0, 0);
        } else {
            SetSize((int)width, (int)height);
        }

        SetPosition(x, y);
    }

    // Update size
    public override void UpdateSize(GameTime time) {
        base.UpdateSize(time);

        if (DisplayWidth <= 0 || DisplayHeight <= 0) {
            _target = null;
        } else {
            if (_target != null)
                _target.Dispose( );

            _target = new RenderTarget2D(_content.Device, (int)DisplayWidth, (int)DisplayHeight);
        }
    }

    // Update position
    public override void UpdatePosition(GameTime time) {
        _viewRenderAngle = (_parent != null ? _parent.GetAngleInRadians( ) : 0) + _angle;
        _viewRenderX = (_parent != null ? _parent.DisplayX : 0) + GetX( ) * (_parent == null ? 1 : _renderScaleX) - _alignX * GetWidth( );
        _viewRenderY = (_parent != null ? _parent.DisplayY : 0) + GetY( ) * (_parent == null ? 1 : _renderScaleY) - _alignY * GetHeight( );

        // Update offset created by rotation
        if (_parent != null) {
            float thisX = DisplayX + _rotationOriginX * GetWidth( );
            float thisY = DisplayY + _rotationOriginY * GetHeight( );
            float parentX = _parent.DisplayX + _parent.GetRotationOriginX( ) * _parent.GetWidth( );
            float parentY = _parent.DisplayY + _parent.GetRotationOriginY( ) * _parent.GetHeight( );
            float distanceFromParentRotationPointX = thisX - parentX;
            float distanceFromParentRotationPointY = thisY - parentY;
            float distance = (float)Math.Sqrt(Math.Pow(distanceFromParentRotationPointX, 2) + Math.Pow(distanceFromParentRotationPointY, 2));
            float angle = (float)Math.Atan2(distanceFromParentRotationPointY, distanceFromParentRotationPointX) + (_parent != null ? _parent.GetAngleInRadians( ) : 0);

            _viewRenderAngleOffsetX = (float)(Math.Cos(angle) * distance);
            _viewRenderAngleOffsetY = (float)(Math.Sin(angle) * distance);

            _viewRenderX += -distanceFromParentRotationPointX + _viewRenderAngleOffsetX;
            _viewRenderY += -distanceFromParentRotationPointY + _viewRenderAngleOffsetY;
        } else {
            _viewRenderAngleOffsetX = 0;
            _viewRenderAngleOffsetY = 0;
        }

        // Trigger event
        foreach (Action action in OnPositionChange)
            action?.Invoke( );

        // Propagate to children
        _children.ForEach(child => child.IsPositionUpdateRequired = true);

        // Reset flag
        IsPositionUpdateRequired = false;
    }

    // Update interaction
    public override void UpdateInteraction(GameTime time) {
        // Skip if it's not interactive
        if (!_isInteractive) return;

        // Mouse is over this element now
        if (_input.IsOverInGame(_viewRenderX, _viewRenderY, DisplayWidth, DisplayHeight)) {
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

    // Render children inside of View
    public virtual void Prepare(GameTime time) {
        if (DisplayOpacity <= 0 || DisplayWidth <= 0 || DisplayHeight <= 0 || _children.Count == 0) return;

        _content.Device.SetRenderTarget(_target);
        _content.Device.Clear(Color.Transparent);
        _content.Canvas.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, _config.IsPixelart ? SamplerState.PointClamp : SamplerState.AnisotropicClamp, null, null, null, Matrix.CreateTranslation(new Vector3(0, 0, 0)));

        _children.ForEach(child => child.Render(time));

        _content.Canvas.End( );
    }

    // Display View with its rendered children onto screen
    public override void Render(GameTime time, bool renderOnlyThisObject = false) {
        if (DisplayOpacity <= 0 || DisplayWidth <= 0 || DisplayHeight <= 0 || _children.Count == 0) return;

        _content.Canvas.Draw(
            _target,
            new Vector2(_viewRenderX + _rotationOriginX * GetWidth( ), _viewRenderY + _rotationOriginY * GetHeight( )),
            null,
            Color.White * DisplayOpacity,
            _viewRenderAngle,
            new Vector2(_rotationOriginX * GetWidth( ), _rotationOriginY * GetHeight( )),
            Vector2.One,
            SpriteEffects.None,
            _depth
        );
    }

}
