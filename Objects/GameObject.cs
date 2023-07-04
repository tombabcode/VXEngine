using Microsoft.Xna.Framework;
using VXEngine.Controllers;
using VXEngine.Objects.Interfaces;
using VXEngine.Types;

namespace VXEngine.Objects;

/// <summary>
/// Basic game object
/// </summary>
public abstract class GameObject : INestable, IPositionable, ISizeable, IOpaqueable, IColorable, IScaleable, IRotateable, ISortable, IAlignable, IDisposable, IInteractable {

    /// <summary>
    /// GUID of the object
    /// </summary>
    public Guid GUID { get; private set; }

    // References
    protected BasicConfigController _config { get; private set; } = null;
    protected BasicInputController _input { get; private set; } = null;

    // Checks in each frame if any update is needed to the outputs
    public bool IsPositionUpdateRequired { get; set; } = true;
    public bool IsSizeUpdateRequired { get; set; } = true;
    public bool IsOpacityUpdateRequired { get; set; } = true;
    public bool IsTintUpdateRequired { get; set; } = true;
    public bool IsDepthUpdateRequired { get; set; } = true;

    // Raw object data (Set by user - before calcualtions, values might be in different units like percentages)
    protected float _sourceX = 0;
    protected float _sourceY = 0;
    protected float _sourceWidth = 0;
    protected float _sourceHeight = 0;
    protected float _sourceScaleX = 1;
    protected float _sourceScaleY = 1;
    protected float _sourceAngle = 0;
    protected float _sourceAngleOriginX = 0;
    protected float _sourceAngleOriginY = 0;
    protected float _sourceAlignOriginX = 0;
    protected float _sourceAlignOriginY = 0;
    protected float _sourceOpacity = 1;
    protected float _sourceDepth = 0;
    protected Color? _sourceTint = null;
    protected UnitType _sourceUnitX = UnitType.Pixel;
    protected UnitType _sourceUnitY = UnitType.Pixel;
    protected UnitType _sourceUnitWidth = UnitType.Pixel;
    protected UnitType _sourceUnitHeight = UnitType.Pixel;

    // Output render data (Real object position, size, angle, etc. - after all calculations, always in defined units like pixels, radians, etc.))
    public GameObject Parent { get; protected set; } = null;
    public List<GameObject> Children { get; protected set; } = new List<GameObject>( );
    public float RenderX { get; protected set; } = 0;
    public float RenderY { get; protected set; } = 0;
    public float RenderWidth { get; protected set; } = 0;
    public float RenderHeight { get; protected set; } = 0;
    public float RenderScaleX { get; protected set; } = 1;
    public float RenderScaleY { get; protected set; } = 1;
    public float RenderAngle { get; protected set; } = 0;
    public float RenderAngleOriginX { get; protected set; } = 0;
    public float RenderAngleOriginY { get; protected set; } = 0;
    public float RenderOpacity { get; protected set; } = 1;
    public float RenderDepth { get; protected set; } = 0;
    public Color? RenderTint { get; protected set; } = null;

    // Interaction (IInteractable)
    protected bool _isInteractive = false;
    protected bool _wasPreviouslyHovered = false;

    // Events
    public List<Action> OnParentChange { get; private set; } = new List<Action>( );
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

        _config = config;
        _input = input;
    }

    // Nesting
    public virtual GameObject SetParent(GameObject parent) {
        // Skip if parent is already the same
        if (Parent == parent) return this;

        // Current parrent is null and new parent is object (Set new parent)
        if (Parent == null && parent != null) {
            Parent = parent;
            Parent.AddChild(this);

            // Current parent is object and new parent is different object (Swap parents)
        } else if (Parent != null && parent != null) {
            Parent.RemoveChild(this);
            Parent = parent;
            Parent.AddChild(this);

            // Current parent is object and new parent is null (Remove parent)
        } else if (Parent != null && parent == null) {
            GameObject par = Parent;
            Parent = null;
            par.RemoveChild(this);
        }

        IsTintUpdateRequired = true;
        IsOpacityUpdateRequired = true;
        IsPositionUpdateRequired = true;
        IsSizeUpdateRequired = true;
        OnParentChange.ForEach(e => e?.Invoke( ));
        return this;
    }
    public virtual GameObject GetParent( ) => Parent;

    // Children
    public virtual void AddChild(GameObject child) {
        if (child == null) return;

        // Given object is not a child
        if (!ContainsChild(child)) {
            child.SetParent(this);
            Children.Add(child);

            // Given object is a child
        } else {
            // Check if given object is a parent
            if (child.GetParent( ) != this)
                child.SetParent(this);
        }
    }
    public virtual void RemoveChild(GameObject child) {
        if (!ContainsChild(child)) return;

        child.SetParent(null);
        Children.Remove(child);
    }
    public virtual void RemoveAllChildren( ) {
        while (Children.Count != 0)
            Children[0].Dispose( );
    }
    public virtual bool ContainsChild(GameObject child) => Children.Contains(child);
    public virtual void AddChildren(List<GameObject> children) => children?.ForEach(child => AddChild(child));
    public virtual List<GameObject> GetChildren( ) => Children;

    // Position
    protected GameObject SetX(float x, UnitType unit) {
        if (_sourceX == x && _sourceUnitX == unit) return this;
        _sourceUnitX = unit;
        _sourceX = x;
        IsPositionUpdateRequired = true;
        return this;
    }
    protected GameObject SetY(float y, UnitType unit) {
        if (_sourceY == y && _sourceUnitY == unit) return this;
        _sourceUnitY = unit;
        _sourceY = y;
        IsPositionUpdateRequired = true;
        return this;
    }
    protected virtual void AddX(float x, UnitType unit) {
        // If unit is the same - simple add value
        if (_sourceUnitX == unit)
            SetX(_sourceX + x, unit);

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
                if (Parent != null) {
                    float parentWidth = Parent.GetWidth( );
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
    protected virtual void AddY(float y, UnitType unit) {
        // If unit is the same - simple add value
        if (_sourceUnitY == unit)
            SetY(_sourceY + y, unit);

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
                if (Parent != null) {
                    float parentHeight = Parent.GetHeight( );
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

    public virtual GameObject SetX(float x) => SetX(x, UnitType.Pixel);
    public virtual GameObject SetY(float y) => SetY(y, UnitType.Pixel);
    public virtual GameObject SetPosition(float position) => SetPosition(position, position);
    public virtual GameObject SetPosition(float x, float y) {
        SetX(x, UnitType.Pixel);
        SetY(y, UnitType.Pixel);
        return this;
    }

    public virtual GameObject SetXAsPercent(float x) => SetX(x, UnitType.Percent);
    public virtual GameObject SetYAsPercent(float y) => SetY(y, UnitType.Percent);
    public virtual GameObject SetPositionAsPercent(float position) => SetPositionAsPercent(position, position);
    public virtual GameObject SetPositionAsPercent(float x, float y) {
        SetX(x, UnitType.Percent);
        SetY(y, UnitType.Percent);
        return this;
    }

    public virtual GameObject SetPosition(BasicInputController.MouseData mouse) => SetPosition(mouse.X, mouse.Y);

    public virtual void AddX(float x) => AddX(x, UnitType.Pixel);
    public virtual void AddY(float y) => AddX(y, UnitType.Pixel);
    public virtual void AddPosition(float position) => AddPosition(position, position);
    public virtual void AddPosition(float x, float y) {
        AddX(x, UnitType.Pixel);
        AddY(y, UnitType.Pixel);
    }

    public virtual void AddXAsPercent(float x) => AddX(x, UnitType.Percent);
    public virtual void AddYAsPercent(float y) => AddX(y, UnitType.Percent);
    public virtual void AddPositionAsPercent(float position) => AddPositionAsPercent(position, position);
    public virtual void AddPositionAsPercent(float x, float y) {
        AddX(x, UnitType.Percent);
        AddY(y, UnitType.Percent);
    }

    public virtual float GetX( ) => _sourceUnitX == UnitType.Pixel ? _sourceX : (Parent != null ? Parent.RenderWidth * _sourceX : _config.ViewWidth * _sourceX);
    public virtual float GetY( ) => _sourceUnitY == UnitType.Pixel ? _sourceY : (Parent != null ? Parent.RenderHeight * _sourceY : _config.ViewHeight * _sourceY);

    // Size
    protected virtual GameObject SetWidth(float width, UnitType unit) {
        if (width < 0) width = 0;
        if (_sourceWidth == width && _sourceUnitWidth == unit) return this;
        _sourceUnitWidth = unit;
        _sourceWidth = width;
        IsSizeUpdateRequired = true;
        IsPositionUpdateRequired = true;
        return this;
    }
    protected virtual GameObject SetHeight(float height, UnitType unit) {
        if (height < 0) height = 0;
        if (_sourceHeight == height && _sourceUnitHeight == unit) return this;
        _sourceUnitHeight = unit;
        _sourceHeight = height;
        IsSizeUpdateRequired = true;
        IsPositionUpdateRequired = true;
        return this;
    }
    protected virtual void AddWidth(float value, UnitType unit) {
        // If unit is the same - simple add value
        if (_sourceUnitWidth == unit)
            SetWidth(_sourceWidth + value, unit);

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
                if (Parent != null) {
                    float parentWidth = Parent.GetWidth( );
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
    protected virtual void AddHeight(float value, UnitType unit) {
        // If unit is the same - simple add value
        if (_sourceUnitHeight == unit)
            SetHeight(_sourceHeight + value, unit);

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
                if (Parent != null) {
                    float parentHeight = Parent.GetHeight( );
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

    public virtual GameObject SetWidth(float width) => SetWidth(width, UnitType.Pixel);
    public virtual GameObject SetHeight(float height) => SetHeight(height, UnitType.Pixel);
    public virtual GameObject SetSize(float width, float height) {
        SetWidth(width, UnitType.Pixel);
        SetHeight(height, UnitType.Pixel);
        return this;
    }
    public virtual GameObject SetSize(float size) => SetSize(size, size);

    public virtual GameObject SetWidthAsPercent(float width) => SetWidth(width, UnitType.Percent);
    public virtual GameObject SetHeightAsPercent(float height) => SetHeight(height, UnitType.Percent);
    public virtual GameObject SetSizeAsPercent(float width, float height) {
        SetWidth(width, UnitType.Percent);
        SetHeight(height, UnitType.Percent);
        return this;
    }
    public virtual GameObject SetSizeAsPercent(float size) => SetSizeAsPercent(size, size);

    public virtual void AddWidth(float value) => AddWidth(value, UnitType.Pixel);
    public virtual void AddHeight(float value) => AddHeight(value, UnitType.Pixel);
    public virtual void AddSize(float width, float height) {
        AddWidth(width, UnitType.Pixel);
        AddHeight(height, UnitType.Pixel);
    }
    public virtual void AddSize(float size) => AddSize(size, size);

    public virtual void AddWidthAsPercent(float value) => AddWidth(value, UnitType.Percent);
    public virtual void AddHeightAsPercent(float value) => AddHeight(value, UnitType.Percent);
    public virtual void AddSizeAsPercent(float width, float height) {
        AddWidth(width, UnitType.Percent);
        AddHeight(height, UnitType.Percent);
    }
    public virtual void AddSizeAsPercent(float size) => AddSize(size, size);

    public virtual float GetWidth( ) => _sourceUnitWidth == UnitType.Pixel ? _sourceWidth : (Parent != null ? Parent.GetWidth( ) * _sourceWidth : _config.ViewWidth * _sourceWidth);
    public virtual float GetHeight( ) => _sourceUnitHeight == UnitType.Pixel ? _sourceHeight : (Parent != null ? Parent.GetHeight( ) * _sourceHeight : _config.ViewHeight * _sourceHeight);
    public virtual GameObject SetFullSize( ) => SetSizeAsPercent(1, 1);
    public virtual GameObject SetHalfSize( ) => SetSizeAsPercent(.5f, .5f);

    // Dimension (Position & Size)
    public virtual GameObject SetDimension(float x, float y, float width, float height) {
        if (width < 0) width = 0;
        if (height < 0) height = 0;
        if (_sourceX == x && _sourceY == y && _sourceUnitX == UnitType.Pixel && _sourceUnitY == UnitType.Pixel &&
            _sourceWidth == width && _sourceHeight == height && _sourceUnitWidth == UnitType.Pixel && _sourceUnitHeight == UnitType.Pixel)
            return this;
        _sourceUnitX = UnitType.Pixel;
        _sourceUnitY = UnitType.Pixel;
        _sourceUnitWidth = UnitType.Pixel;
        _sourceUnitHeight = UnitType.Pixel;
        _sourceX = x;
        _sourceY = y;
        _sourceWidth = width;
        _sourceHeight = height;
        IsPositionUpdateRequired = true;
        IsSizeUpdateRequired = true;
        return this;
    }

    // Opacity
    public virtual GameObject SetOpacity(float opacity) {
        if (opacity < 0) opacity = 0;
        if (opacity > 1) opacity = 1;
        if (_sourceOpacity == opacity) return this;

        _sourceOpacity = opacity;
        IsOpacityUpdateRequired = true;
        return this;
    }
    public virtual float GetOpacity( ) => _sourceOpacity;

    // Color
    public virtual GameObject SetColor(Color? color) {
        if ((color == null && _sourceTint == Color.White) || (color != null && color == _sourceTint)) return this;
        _sourceTint = color ?? Color.White;
        IsTintUpdateRequired = true;
        return this;
    }
    public virtual Color? GetColor( ) => _sourceTint;

    // Scale
    public virtual GameObject SetScaleX(float scale) {
        if (_sourceScaleX == scale) return this;
        if (scale < 0) scale = 0;
        _sourceScaleX = scale;
        IsPositionUpdateRequired = true;
        IsSizeUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetScaleY(float scale) {
        if (_sourceScaleY == scale) return this;
        if (scale < 0) scale = 0;
        _sourceScaleY = scale;
        IsPositionUpdateRequired = true;
        IsSizeUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetScale(float scaleX, float scaleY) {
        if (_sourceScaleX == scaleX && _sourceScaleY == scaleY) return this;
        if (scaleX < 0) scaleX = 0;
        if (scaleY < 0) scaleY = 0;
        _sourceScaleX = scaleX;
        _sourceScaleY = scaleY;
        IsPositionUpdateRequired = true;
        IsSizeUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetScale(float scale) => SetScale(scale, scale);
    public virtual float GetScaleX( ) => _sourceScaleX;
    public virtual float GetScaleY( ) => _sourceScaleY;

    // Angle
    public virtual GameObject SetAngleInRadians(float angle) {
        if (_sourceAngle == angle) return this;
        _sourceAngle = angle;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetAngleInDegrees(float angle) {
        if (_sourceAngle == MathHelper.ToRadians(angle)) return this;
        _sourceAngle = MathHelper.ToRadians(angle);
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetRotationOriginX(float originX) {
        if (_sourceAngleOriginX == originX) return this;
        _sourceAngleOriginX = originX;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetRotationOriginY(float originY) {
        if (_sourceAngleOriginY == originY) return this;
        _sourceAngleOriginY = originY;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetRotationOrigin(float originX, float originY) {
        if (_sourceAngleOriginX == originX && _sourceAngleOriginY == originY) return this;
        _sourceAngleOriginX = originX;
        _sourceAngleOriginY = originY;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetRotationOrigin(float origin) => SetRotationOrigin(origin, origin);
    public virtual void AddAngleInRadians(float angle) => SetAngleInRadians(_sourceAngle + angle);
    public virtual void AddAngleInDegrees(float angle) => SetAngleInRadians(_sourceAngle + MathHelper.ToRadians(angle));
    public virtual float GetAngleInRadians( ) => _sourceAngle;
    public virtual float GetAngleInDegrees( ) => MathHelper.ToDegrees(_sourceAngle);
    public virtual float GetRotationOriginX( ) => _sourceAngleOriginX;
    public virtual float GetRotationOriginY( ) => _sourceAngleOriginY;

    // Depth
    public virtual GameObject SetDepth(float depth) {
        if (depth < 0) depth = 0;
        if (depth > 1) depth = 1;
        _sourceDepth = depth;

        return this;
    }
    public virtual float GetDepth( ) => _sourceDepth;

    // Align
    public virtual GameObject SetAlignX(float align) {
        if (_sourceAlignOriginX == align) return this;
        _sourceAlignOriginX = align;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetAlignY(float align) {
        if (_sourceAlignOriginY == align) return this;
        _sourceAlignOriginY = align;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetAlign(float alignX, float alignY) {
        if (_sourceAlignOriginX == alignX && _sourceAlignOriginY == alignY) return this;
        _sourceAlignOriginX = alignX;
        _sourceAlignOriginY = alignY;
        IsPositionUpdateRequired = true;
        return this;
    }
    public virtual GameObject SetAlign(float align) => SetAlign(align, align);
    public virtual float GetAlignX( ) => _sourceAlignOriginX;
    public virtual float GetAlignY( ) => _sourceAlignOriginY;

    // Interaction
    public virtual void EnableInteraction( ) => _isInteractive = true;
    public virtual void DisableInteraction( ) => _isInteractive = false;
    public virtual GameObject SetInteraction(bool isEnabled) {
        _isInteractive = isEnabled;
        return this;
    }
    public virtual bool IsPointInside(float x, float y) => RenderWidth > 0 && RenderHeight > 0 && x >= RenderX && x <= RenderX + RenderWidth && y >= RenderY && y <= RenderY + RenderHeight;

    // Dispose
    public virtual void Dispose( ) {
        // Remove children
        RemoveAllChildren( );

        // Remove parent and inform it about loosing its child
        SetParent(null);

        // Remove references
        OnPositionChange.Clear( );
        OnSizeChange.Clear( );
        OnOpacityChange.Clear( );
        OnColorChange.Clear( );
        OnHover.Clear( );
        Children.Clear( );
    }

    // Update color
    public virtual void UpdateColor( ) {
        // Tint is empty - inherit tint from parent
        if (_sourceTint == null) {
            RenderTint = Parent?.RenderTint;

        // Custom tint - add it
        } else {
            if (Parent != null && Parent.RenderTint != null) {
                float r = (Parent.RenderTint.Value.R + _sourceTint.Value.R) / 510f;
                float g = (Parent.RenderTint.Value.G + _sourceTint.Value.G) / 510f;
                float b = (Parent.RenderTint.Value.B + _sourceTint.Value.B) / 510f;
                float a = (Parent.RenderTint.Value.A + _sourceTint.Value.A) / 510f;
                RenderTint = new Color(r, g, b, a);
            } else {
                RenderTint = _sourceTint;
            }
        }

        // Trigger event
        foreach (Action action in OnColorChange)
            action?.Invoke( );
        
        // Propagate to children
        Children.ForEach(child => child.IsTintUpdateRequired = true);

        // Reset flag
        IsTintUpdateRequired = false;
    }

    // Update opacity
    public virtual void UpdateOpacity( ) {
        RenderOpacity = (Parent != null ? Parent.RenderOpacity : 1) * _sourceOpacity;

        // Trigger event
        foreach (Action action in OnOpacityChange)
            action?.Invoke( );

        // Propagate to children
        Children.ForEach(child => child.IsOpacityUpdateRequired = true);

        // Reset flag
        IsOpacityUpdateRequired = false;
    }

    /// <summary>
    /// Update output render size of the object including all transformations and calculations
    /// </summary>
    /// <param name="time"></param>
    public virtual void UpdateSize( ) {
        RenderScaleX = (Parent != null ? Parent.RenderScaleX : 1) * _sourceScaleX;
        RenderScaleY = (Parent != null ? Parent.RenderScaleY : 1) * _sourceScaleY;
        RenderWidth = GetWidth( ) * RenderScaleX;
        RenderHeight = GetHeight( ) * RenderScaleY;

        OnSizeChange.ForEach(e => e?.Invoke( ));
        Children.ForEach(child => child.IsSizeUpdateRequired = true);

        IsSizeUpdateRequired = false;
    }

    /// <summary>
    /// Update output render position of the object including all transformations and calculations
    /// </summary>
    /// <param name="time"></param>
    public virtual void UpdatePosition( ) {
        RenderAngle = (Parent != null ? Parent.RenderAngle : 0) + _sourceAngle;

        RenderAngleOriginX = _sourceAngleOriginX * RenderWidth;
        RenderAngleOriginY = _sourceAngleOriginY * RenderHeight;
        
        RenderX = (Parent != null ? Parent.RenderX : 0) + GetX( ) - _sourceAlignOriginX * RenderWidth;
        RenderY = (Parent != null ? Parent.RenderY : 0) + GetY( ) - _sourceAlignOriginY * RenderHeight;

        if (Parent != null) {
            float parentRotationPointX = Parent.RenderX + Parent.RenderAngleOriginX;
            float parentRotationPointY = Parent.RenderY + Parent.RenderAngleOriginY;

            float objectRotationPointX = RenderX + RenderAngleOriginX;
            float objectRotationPointY = RenderY + RenderAngleOriginY;

            float radius = (float)Math.Sqrt(Math.Pow(parentRotationPointX - objectRotationPointX, 2) + Math.Pow(parentRotationPointY - objectRotationPointY, 2));
            float angle = (float)Math.Atan2(objectRotationPointY - parentRotationPointY, objectRotationPointX - parentRotationPointX);

            float offsetX = (float)Math.Cos(Parent.RenderAngle + angle) * radius;
            float offsetY = (float)Math.Sin(Parent.RenderAngle + angle) * radius;

            RenderX += (parentRotationPointX - objectRotationPointX) + offsetX;
            RenderY += (parentRotationPointY - objectRotationPointY) + offsetY;
        }

        OnPositionChange.ForEach(e => e?.Invoke( ));
        Children.ForEach(child => child.IsPositionUpdateRequired = true);
        IsPositionUpdateRequired = false;
    }

    // Update depth
    public virtual void UpdateDepth( ) {
        RenderDepth = (Parent != null ? Parent.RenderDepth : 0) + _sourceDepth;

        // Propagate to children
        Children.ForEach(child => child.IsDepthUpdateRequired = true);

        // Reset flag
        IsDepthUpdateRequired = false;
    }

    // Update interaction
    public virtual void UpdateInteraction( ) {
        // Skip if it's not interactive
        if (!_isInteractive) return;

        // Mouse is over this element now
        if (_input.PropagateEvent && IsPointInside(_input.MouseInGame.X, _input.MouseInGame.Y)) {
            // It's hovered for the first time
            if (!_wasPreviouslyHovered) {
                OnHoverEnter?.ForEach(e => e?.Invoke( ));
            }

            // Hover event
            if (OnHover != null)
                for (int i = 0; i < OnHover.Count; i++)
                    OnHover[i]?.Invoke( );

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
        if (IsSizeUpdateRequired) UpdateSize( );
        if (IsPositionUpdateRequired) UpdatePosition( );
        if (IsOpacityUpdateRequired) UpdateOpacity( );
        if (IsDepthUpdateRequired) UpdateDepth( );
        if (IsTintUpdateRequired) UpdateColor( );

        // Update children
        if (!updateOnlyThisObject)
            Children.ForEach(child => child.Update(time));

        // Update interaction
        UpdateInteraction( );
    }

    // Render
    public virtual void Render(GameTime time, bool renderOnlyThisObject = false) {
        if (!renderOnlyThisObject)
            Children.ForEach(child => child.Render(time, false));
    }

}