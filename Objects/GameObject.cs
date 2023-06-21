using Microsoft.Xna.Framework;
using VXEngine.Controllers;
using VXEngine.Objects.Interfaces;
using VXEngine.Types;
using static VXEngine.Controllers.BasicInputController;

namespace VXEngine.Objects;

public abstract class GameObject : INestable, IPositionable, ISizeable, IOpaqueable, IColorable, IScaleable, IRotateable, ISortable, IAlignable, IDisposable, IInteractable {

    public Guid GUID { get; private set; }

    protected BasicConfigController _config { get; private set; } = null;
    protected BasicInputController _input { get; private set; } = null;

    // Checks in each frame if any update is needed to the outputs
    public bool IsOpacityUpdateRequired { get; set; } = true;
    public bool IsColorUpdateRequired { get; set; } = true;
    public bool IsPositionUpdateRequired { get; set; } = true;
    public bool IsSizeUpdateRequired { get; set; } = true;

    // Output render
    protected float _renderX = 0;
    protected float _renderY = 0;
    protected float _renderWidth = 0;
    protected float _renderHeight = 0;
    protected float _renderOpacity = 1;
    protected Color? _renderColor = null;
    protected float _renderScaleX = 1;
    protected float _renderScaleY = 1;
    protected float _renderAngle = 0;
    protected float _renderAngleOffsetX = 0;
    protected float _renderAngleOffsetY = 0;

    // Nesting (INestable)
    protected GameObject _parent = null;
    protected List<GameObject> _children = null;

    // Position (IPositionable)
    protected float _offsetX = 0;
    protected float _offsetY = 0;
    protected UnitType _offsetXUnit = UnitType.Pixel;
    protected UnitType _offsetYUnit = UnitType.Pixel;

    // Size (ISizeable)
    protected float _width = 0;
    protected float _height = 0;
    protected UnitType _widthUnit = UnitType.Pixel;
    protected UnitType _heightUnit = UnitType.Pixel;

    // Opacity (IOpaqueable)
    protected float _opacity = 1;

    // Color (IColorable)
    protected Color? _color = null;

    // Scale (IScaleable)
    protected float _scaleX = 1;
    protected float _scaleY = 1;

    // Angle (IRotateable)
    protected float _angle = 0;
    protected float _rotationOriginX = 0;
    protected float _rotationOriginY = 0;

    // Depth (ISortable)
    protected float _depth = 0;

    // Align (IAlignable)
    protected float _alignX = 0;
    protected float _alignY = 0;

    // Interaction (IInteractable)
    protected bool _isInteractive = false;
    protected bool _wasPreviouslyHovered = false;

    // Events
    public List<Action> OnPositionChange { get; private set; } = new List<Action>( );
    public List<Action> OnSizeChange { get; private set; } = new List<Action>( );
    public List<Action> OnOpacityChange { get; private set; } = new List<Action>( );
    public List<Action> OnColorChange { get; private set; } = new List<Action>( );

    public List<Action> OnHoverEnter { get; private set; } = new List<Action>( );
    public List<Action> OnHoverExit { get; private set; } = new List<Action>( );
    public List<Action> OnHover { get; private set; } = new List<Action>( );

    // Constructor
    public GameObject(BasicConfigController config, BasicInputController input) {
        GUID = Guid.NewGuid( );

        _children = new List<GameObject>( );
        _config = config;
        _input = input;
    }

    public float DisplayX => _renderX;
    public float DisplayY => _renderY;
    public float DisplayWidth => _renderWidth;
    public float DisplayHeight => _renderHeight;
    public float DisplayOpacity => _renderOpacity;
    public float DisplayAngle => _renderAngle;
    public float DisplayAngleInDegrees => MathHelper.ToDegrees(_renderAngle);
    public Color? DisplayColor => _renderColor;

    // Parent
    public virtual GameObject GetParent( ) => _parent;
    public virtual GameObject SetParent(GameObject parent) {
        // Skip if parent is already the same
        if (_parent == parent) return this;

        // Current parrent is null and new parent is object (Set new parent)
        if (_parent == null && parent != null) {
            _parent = parent;
            _parent.AddChild(this);

            // Current parent is object and new parent is different object (Swap parents)
        } else if (_parent != null && parent != null) {
            _parent.RemoveChild(this);
            _parent = parent;
            _parent.AddChild(this);

            // Current parent is object and new parent is null (Remove parent)
        } else if (_parent != null && parent == null) {
            _parent.RemoveChild(this);
            _parent = null;
        }

        IsColorUpdateRequired = true;
        IsOpacityUpdateRequired = true;
        IsPositionUpdateRequired = true;
        IsSizeUpdateRequired = true;
        return this;
    }

    // Children
    public virtual bool ContainsChild(GameObject child) => _children.Contains(child);
    public virtual void AddChild(GameObject child) {
        if (child == null) return;

        // Given object is not a child
        if (!ContainsChild(child)) {
            child.SetParent(this);
            _children.Add(child);

            // Given object is a child
        } else {
            // Check if given object is a parent
            if (child.GetParent( ) != this)
                child.SetParent(this);
        }
    }
    public virtual void AddChildren(List<GameObject> children) => children?.ForEach(child => AddChild(child));
    public virtual List<GameObject> GetChildren( ) => _children;
    public virtual void RemoveChild(GameObject child) {
        if (!ContainsChild(child)) return;

        child.SetParent(null);
        _children.Remove(child);
    }
    public virtual void RemoveAllChildren( ) => _children.ForEach(child => {
        child.SetParent(null);
        _children.Remove(child);
    });

    // Position
    public virtual void AddX(float x, UnitType unit = UnitType.Pixel) {
        // If unit is the same - simple add value
        if (_offsetXUnit == unit)
            SetX(_offsetX + x, unit);

        // If unit differs - adjust value
        else {
            // Transform percent into pixels
            if (unit == UnitType.Pixel) {
                float currentPositionXInPixels = GetX( );
                SetX(currentPositionXInPixels + x, UnitType.Pixel);

                // Transform pixels into width
            } else {
                float currentPositionXInPixels = GetX( );
                float asPercentage = 0;

                // Calculate percentage to parent
                if (_parent != null) {
                    float parentWidth = _parent.GetWidth( );
                    if (parentWidth > 0)
                        asPercentage = currentPositionXInPixels / parentWidth;

                    // Calculate percentage to view
                } else {
                    float viewWidth = _config.ViewWidth;
                    if (viewWidth > 0)
                        asPercentage = currentPositionXInPixels / viewWidth;
                }

                SetX(asPercentage + x, UnitType.Percent);
            }
        }
    }
    public virtual void AddY(float y, UnitType unit = UnitType.Pixel) {
        // If unit is the same - simple add value
        if (_offsetYUnit == unit)
            SetY(_offsetY + y, unit);

        // If unit differs - adjust value
        else {
            // Transform percent into pixels
            if (unit == UnitType.Pixel) {
                float currentPositionYInPixels = GetY( );
                SetY(currentPositionYInPixels + y, UnitType.Pixel);

                // Transform pixels into width
            } else {
                float currentPositionYInPixels = GetY( );
                float asPercentage = 0;

                // Calculate percentage to parent
                if (_parent != null) {
                    float parentHeight = _parent.GetHeight( );
                    if (parentHeight > 0)
                        asPercentage = currentPositionYInPixels / parentHeight;

                    // Calculate percentage to view
                } else {
                    float viewHeight = _config.ViewHeight;
                    if (viewHeight > 0)
                        asPercentage = currentPositionYInPixels / viewHeight;
                }

                SetY(asPercentage + y, UnitType.Percent);
            }
        }
    }
    public virtual void AddPosition(float x, float y, UnitType unit = UnitType.Pixel) {
        AddX(x, unit);
        AddY(y, unit);
    }
    public virtual void AddPosition(float position, UnitType unit = UnitType.Pixel) => AddPosition(position, position, unit);
    public virtual float GetX( ) => _offsetXUnit == UnitType.Pixel ? _offsetX : (_parent != null ? _parent.GetWidth( ) * _offsetX : _config.ViewWidth * _offsetX);
    public virtual float GetY( ) => _offsetYUnit == UnitType.Pixel ? _offsetY : (_parent != null ? _parent.GetHeight( ) * _offsetY : _config.ViewHeight * _offsetY);
    public virtual GameObject SetX(float x, UnitType unit = UnitType.Pixel) {
        if (_offsetX == x && _offsetXUnit == unit) return this;
        _offsetXUnit = unit;
        _offsetX = x;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetY(float y, UnitType unit = UnitType.Pixel) {
        if (_offsetY == y && _offsetYUnit == unit) return this;
        _offsetYUnit = unit;
        _offsetY = y;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetPosition(float x, float y, UnitType unit = UnitType.Pixel) {
        if (_offsetX == x && _offsetY == y && _offsetXUnit == unit && _offsetYUnit == unit) return this;
        _offsetXUnit = unit;
        _offsetYUnit = unit;
        _offsetX = x;
        _offsetY = y;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetPosition(MouseData mouse) => SetPosition(mouse.X, mouse.Y, UnitType.Pixel);

    // Size
    public virtual void AddWidth(float value, UnitType unit = UnitType.Pixel) {
        // If unit is the same - simple add value
        if (_widthUnit == unit)
            SetWidth(_width + value, unit);

        // If unit differs - adjust value
        else {
            // Transform percent into pixels
            if (unit == UnitType.Pixel) {
                float currentWidthInPixels = GetWidth( );
                SetWidth(currentWidthInPixels + value, UnitType.Pixel);

                // Transform pixels into width
            } else {
                float currentWidthInPixels = GetWidth( );
                float asPercentage = 0;

                // Calculate percentage to parent
                if (_parent != null) {
                    float parentWidth = _parent.GetWidth( );
                    if (parentWidth > 0)
                        asPercentage = currentWidthInPixels / parentWidth;

                    // Calculate percentage to view
                } else {
                    float viewWidth = _config.ViewWidth;
                    if (viewWidth > 0)
                        asPercentage = currentWidthInPixels / viewWidth;
                }

                SetWidth(asPercentage + value, UnitType.Percent);
            }
        }
    }
    public virtual void AddHeight(float value, UnitType unit = UnitType.Pixel) {
        // If unit is the same - simple add value
        if (_heightUnit == unit)
            SetHeight(_height + value, unit);

        // If unit differs - adjust value
        else {
            // Transform percent into pixels
            if (unit == UnitType.Pixel) {
                float currentHeightInPixels = GetHeight( );
                SetHeight(currentHeightInPixels + value, UnitType.Pixel);

                //Transform pixels into width
            } else {
                float currentHeightInPixels = GetHeight( );
                float asPercentage = 0;

                // Calculate percentage to parent
                if (_parent != null) {
                    float parentHeight = _parent.GetHeight( );
                    if (parentHeight > 0)
                        asPercentage = currentHeightInPixels / parentHeight;

                    // Calculate percentage to view
                } else {
                    float viewHeight = _config.ViewHeight;
                    if (viewHeight > 0)
                        asPercentage = currentHeightInPixels / viewHeight;
                }

                SetHeight(asPercentage + value, UnitType.Percent);
            }
        }
    }
    public virtual void AddSize(float width, float height, UnitType unit = UnitType.Pixel) {
        AddWidth(width, unit);
        AddHeight(height, unit);
    }
    public virtual void AddSize(float size, UnitType unit = UnitType.Pixel) => AddSize(size, size, unit);
    public virtual float GetWidth( ) => _widthUnit == UnitType.Pixel ? _width : (_parent != null ? _parent.GetWidth( ) * _width : _config.ViewWidth * _width);
    public virtual float GetHeight( ) => _heightUnit == UnitType.Pixel ? _height : (_parent != null ? _parent.GetHeight( ) * _height : _config.ViewHeight * _height);
    public virtual GameObject SetWidth(float width, UnitType unit = UnitType.Pixel) {
        if (width < 0) width = 0;
        if (_width == width && _widthUnit == unit) return this;
        _widthUnit = unit;
        _width = width;
        IsSizeUpdateRequired = true;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetHeight(float height, UnitType unit = UnitType.Pixel) {
        if (height < 0) height = 0;
        if (_height == height && _heightUnit == unit) return this;
        _heightUnit = unit;
        _height = height;
        IsSizeUpdateRequired = true;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetSize(float width, float height, UnitType unit = UnitType.Pixel) {
        if (width < 0) width = 0;
        if (height < 0) height = 0;
        if (_width == width && _height == height && _widthUnit == unit && _heightUnit == unit) return this;
        _widthUnit = unit;
        _heightUnit = unit;
        _width = width;
        _height = height;
        IsSizeUpdateRequired = true;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetSize(float size, UnitType unit = UnitType.Pixel) => SetSize(size, size, unit);

    // Dimension (Position & Size)
    public virtual GameObject SetDimension(float x, float y, float width, float height) {
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
        return this;
    }

    // Opacity
    public virtual GameObject SetOpacity(float opacity) {
        if (opacity < 0) opacity = 0;
        if (opacity > 1) opacity = 1;
        if (_opacity == opacity) return this;

        _opacity = opacity;
        IsOpacityUpdateRequired = true;
        return this;
    }
    public virtual float GetOpacity( ) => _opacity;

    // Color
    public virtual GameObject SetColor(Color? color) {
        if (_color == color) return this;
        _color = color;
        IsColorUpdateRequired = true;
        return this;
    }
    public virtual Color? GetColor( ) => _color;

    // Scale
    public virtual GameObject SetScaleX(float scale) {
        if (_scaleX == scale) return this;
        if (scale < 0) scale = 0;
        _scaleX = scale;
        IsPositionUpdateRequired = true;
        IsSizeUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetScaleY(float scale) {
        if (_scaleY == scale) return this;
        if (scale < 0) scale = 0;
        _scaleY = scale;
        IsPositionUpdateRequired = true;
        IsSizeUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetScale(float scale) {
        if (_scaleX == scale && _scaleY == scale) return this;
        if (scale < 0) scale = 0;
        _scaleX = scale;
        _scaleY = scale;
        IsPositionUpdateRequired = true;
        IsSizeUpdateRequired = true;
        return this;
    }
    public virtual float GetScaleX( ) => _scaleX;
    public virtual float GetScaleY( ) => _scaleY;

    // Angle
    public virtual void AddAngleInRadians(float angle) => SetAngleInRadians(_angle + angle);
    public virtual void AddAngleInDegrees(float angle) => SetAngleInRadians(_angle + MathHelper.ToRadians(angle));
    public virtual GameObject SetAngleInRadians(float angle) {
        if (_angle == angle) return this;
        _angle = angle;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetAngleInDegrees(float angle) {
        if (_angle == MathHelper.ToRadians(angle)) return this;
        _angle = MathHelper.ToRadians(angle);
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetRotationOriginX(float originX) {
        if (_rotationOriginX == originX) return this;
        _rotationOriginX = originX;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetRotationOriginY(float originY) {
        if (_rotationOriginY == originY) return this;
        _rotationOriginY = originY;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetRotationOrigin(float originX, float originY) {
        if (_rotationOriginX == originX && _rotationOriginY == originY) return this;
        _rotationOriginX = originX;
        _rotationOriginY = originY;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetRotationOrigin(float origin) => SetRotationOrigin(origin, origin);
    public virtual float GetAngleInRadians( ) => _angle;
    public virtual float GetAngleInDegrees( ) => MathHelper.ToDegrees(_angle);
    public virtual float GetRotationOriginX( ) => _rotationOriginX;
    public virtual float GetRotationOriginY( ) => _rotationOriginY;

    // Depth
    public virtual GameObject SetDepth(float depth) {
        if (depth < 0) depth = 0;
        if (depth > 1) depth = 1;
        _depth = depth;

        return this;
    }
    public virtual float GetDepth( ) => _depth;

    // Align
    public virtual GameObject SetAlignX(float align) {
        if (_alignX == align) return this;
        _alignX = align;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetAlignY(float align) {
        if (_alignY == align) return this;
        _alignY = align;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetAlign(float alignX, float alignY) {
        if (_alignX == alignX && _alignY == alignY) return this;
        _alignX = alignX;
        _alignY = alignY;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetAlign(float align) => SetAlign(align, align);
    public virtual float GetAlignX( ) => _alignX;
    public virtual float GetAlignY( ) => _alignY;

    // Interaction
    public virtual void EnableInteraction( ) => _isInteractive = true;
    public virtual void DisableInteraction( ) => _isInteractive = false;
    public virtual GameObject SetInteraction(bool isEnabled) {
        _isInteractive = isEnabled;
        return this;
    }

    // Dispose
    public virtual void Dispose( ) {
        // Remove parent and inform it about loosing its child
        SetParent(null);

        // Remove children
        foreach (GameObject child in _children)
            child.Dispose( );

        // Remove references
        OnPositionChange.Clear( );
        OnSizeChange.Clear( );
        OnOpacityChange.Clear( );
        OnColorChange.Clear( );
        OnHover.Clear( );
        _children.Clear( );

        _children = null;
        _config = null;
        OnPositionChange = null;
        OnSizeChange = null;
        OnOpacityChange = null;
        OnColorChange = null;
        OnHover = null;
    }

    // Update color
    public virtual void UpdateColor(GameTime time) {
        if (_parent == null || _parent._renderColor == null || _color == null) {
            _renderColor = _color;
        } else {
            float r = (_parent._renderColor.Value.R + _color.Value.R) / 510f;
            float g = (_parent._renderColor.Value.G + _color.Value.G) / 510f;
            float b = (_parent._renderColor.Value.B + _color.Value.B) / 510f;
            float a = (_parent._renderColor.Value.A + _color.Value.A) / 510f;
            _renderColor = new Color(r, g, b, a);
        }

        // Trigger event
        foreach (Action action in OnColorChange)
            action?.Invoke( );
        
        // Propagate to children
        _children.ForEach(child => child.IsColorUpdateRequired = true);

        // Reset flag
        IsColorUpdateRequired = false;
    }

    // Update opacity
    public virtual void UpdateOpacity(GameTime time) {
        _renderOpacity = (_parent != null ? _parent._renderOpacity : 1) * _opacity;

        // Trigger event
        foreach (Action action in OnOpacityChange)
            action?.Invoke( );

        // Propagate to children
        _children.ForEach(child => child.IsOpacityUpdateRequired = true);

        // Reset flag
        IsOpacityUpdateRequired = false;
    }

    // Update size
    public virtual void UpdateSize(GameTime time) {
        _renderScaleX = (_parent != null ? _parent._renderScaleX : 1) * _scaleX;
        _renderScaleY = (_parent != null ? _parent._renderScaleY : 1) * _scaleY;
        _renderWidth = GetWidth( ) * _renderScaleX;
        _renderHeight = GetHeight( ) * _renderScaleY;

        // Trigger event
        foreach (Action action in OnSizeChange)
            action?.Invoke( );

        // Propagate to children
        _children.ForEach(child => child.IsSizeUpdateRequired = true);

        // Reset flag
        IsSizeUpdateRequired = false;
    }

    // Update position
    public virtual void UpdatePosition(GameTime time) {
        _renderAngle = (_parent != null ? _parent._renderAngle : 0) + _angle;
        _renderX = (_parent != null ? _parent.DisplayX : 0) + GetX( ) * (_parent == null ? 1 : _renderScaleX) - _alignX * GetWidth( );
        _renderY = (_parent != null ? _parent.DisplayY : 0) + GetY( ) * (_parent == null ? 1 : _renderScaleY) - _alignY * GetHeight( );

        // Update offset created by rotation
        if (_parent != null) {
            float thisX = DisplayX + _rotationOriginX * GetWidth( );
            float thisY = DisplayY + _rotationOriginY * GetHeight( );
            float parentX = _parent.DisplayX + _parent._rotationOriginX * _parent.GetWidth( );
            float parentY = _parent.DisplayY + _parent._rotationOriginY * _parent.GetHeight( );
            float distanceFromParentRotationPointX = thisX - parentX;
            float distanceFromParentRotationPointY = thisY - parentY;
            float distance = (float)Math.Sqrt(Math.Pow(distanceFromParentRotationPointX, 2) + Math.Pow(distanceFromParentRotationPointY, 2));
            float angle = (float)Math.Atan2(distanceFromParentRotationPointY, distanceFromParentRotationPointX) + (_parent != null ? _parent._renderAngle : 0);

            _renderAngleOffsetX = (float)(Math.Cos(angle) * distance);
            _renderAngleOffsetY = (float)(Math.Sin(angle) * distance);

            _renderX += -distanceFromParentRotationPointX + _renderAngleOffsetX;
            _renderY += -distanceFromParentRotationPointY + _renderAngleOffsetY;
        } else {
            _renderAngleOffsetX = 0;
            _renderAngleOffsetY = 0;
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
    public virtual void UpdateInteraction(GameTime time) {
        // Skip if it's not interactive
        if (!_isInteractive) return;

        // Mouse is over this element now
        if (_input.IsOver(this)) {
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

    // Update
    public virtual void Update(GameTime time, bool updateOnlyThisObject = false) {
        // Updates
        if (IsColorUpdateRequired) UpdateColor(time);
        if (IsOpacityUpdateRequired) UpdateOpacity(time);
        if (IsSizeUpdateRequired) UpdateSize(time);
        if (IsPositionUpdateRequired) UpdatePosition(time);

        // Update children
        if (!updateOnlyThisObject)
            _children.ForEach(child => child.Update(time));

        // Update interaction
        UpdateInteraction(time);
    }

    // Render
    public virtual void Render(GameTime time, bool renderOnlyThisObject = false) {
        if (!renderOnlyThisObject)
            _children.ForEach(child => child.Render(time, false));
    }

}