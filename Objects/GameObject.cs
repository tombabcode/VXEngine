using Microsoft.Xna.Framework;
using VXEngine.Controllers;
using VXEngine.Utility;

namespace VXEngine.Objects;

/// <summary>
/// Abstract base for all in-game objects
/// </summary>
public abstract class GameObject {

    /// <summary>
    /// Helper variable to do single-time hover in/out event
    /// </summary>
    private bool _isHovering;

    /// <summary>
    /// Helper variable to differentiate click from press
    /// </summary>
    private bool _isPressed;

    /// <summary>
    /// Reference
    /// </summary>
    protected BasicInputController _input;

    /// <summary>
    /// Parent object
    /// </summary>
    protected GameObject? _parent;

    /// <summary>
    /// Children objects
    /// </summary>
    protected List<GameObject> _children;

    /// <summary>
    /// Object's relative position in X axis
    /// </summary>
    protected float _x = 0;

    /// <summary>
    /// Object's relative position in Y axis
    /// </summary>
    protected float _y = 0;

    /// <summary>
    /// Object's relative width
    /// </summary>
    protected float _width = 0;

    /// <summary>
    /// Object's relative height
    /// </summary>
    protected float _height = 0;

    /// <summary>
    /// Object's width scale
    /// </summary>
    protected float _scaleX = 1;

    /// <summary>
    /// Object's height scale
    /// </summary>
    protected float _scaleY = 1;

    /// <summary>
    /// Object's relative origin in X axis
    /// </summary>
    protected float _originX = 0;

    /// <summary>
    /// Object's relative origin in Y axis
    /// </summary>
    protected float _originY = 0;

    /// <summary>
    /// Object's rotation angle in radians
    /// </summary>
    protected float _angle = 0;

    /// <summary>
    /// Object's relative rotation origin in X axis
    /// </summary>
    protected float _rotationOriginX = 0;

    /// <summary>
    /// Object's relative rotation origin in Y axis
    /// </summary>
    protected float _rotationOriginY = 0;

    /// <summary>
    /// Opacity of the object
    /// </summary>
    protected float _alpha = 1;

    /// <summary>
    /// Color of the object
    /// </summary>
    protected Color _color = Color.White;

    /// <summary>
    /// Is interactive for input events
    /// </summary>
    protected bool _isInteractive = false;

    /// <summary>
    /// Is visible. Object might not be visible but still interactive
    /// </summary>
    protected bool _isVisible = true;

    // ============================================================================================================================================ EVENTS

    public List<Action<GameObject>> OnHoverIn { get; private set; } = new List<Action<GameObject>>( );
    public List<Action<GameObject>> OnHoverOut { get; private set; } = new List<Action<GameObject>>( );
    public List<Action<GameObject>> OnClick { get; private set; } = new List<Action<GameObject>>( );
    public List<Action<GameObject>> OnPress { get; private set; } = new List<Action<GameObject>>( );
    public List<Action<GameObject>> OnRelease { get; private set; } = new List<Action<GameObject>>( );

    public Func<bool> OnHoverCondition { get; set; } = null;
    public Func<bool> OnClickCondition { get; set; } = null;
    public Func<bool> OnPressCondition { get; set; } = null;

    // ============================================================================================================================================ CONSTRUCTOR

    /// <summary>
    /// Constructor
    /// </summary>
    public GameObject(BasicInputController input) {
        _input = input;
        _children = new List<GameObject>( );
    }

    // ============================================================================================================================================ GETTERS

    /// <summary>
    /// Get parent
    /// </summary>
    /// <returns></returns>
    public virtual GameObject? GetParent( ) { return _parent; }

    /// <summary>
    /// Get children
    /// </summary>
    /// <returns></returns>
    public virtual List<GameObject> GetChildren( ) { return _children; }

    /// <summary>
    /// Get object's X position
    /// </summary>
    public virtual float GetX( ) { return _x; }

    /// <summary>
    /// Get object's Y position
    /// </summary>
    public virtual float GetY( ) { return _y; }

    /// <summary>
    /// Get object's width
    /// </summary>
    public virtual float GetWidth( ) { return _width; }

    /// <summary>
    /// Get object's height
    /// </summary>
    public virtual float GetHeight( ) { return _height; }

    /// <summary>
    /// Get object's scale X
    /// </summary>
    public virtual float GetScaleX( ) { return _scaleX; }

    /// <summary>
    /// Get object's scale Y
    /// </summary>
    public virtual float GetScaleY( ) { return _scaleY; }

    /// <summary>
    /// Get object's origin in X axis
    /// </summary>
    public virtual float GetOriginX( ) { return _originX; }

    /// <summary>
    /// Get object's origin in Y axis
    /// </summary>
    public virtual float GetOriginY( ) { return _originY; }

    /// <summary>
    /// Get object's rotation angle in radians
    /// </summary>
    public virtual float GetAngle( ) { return _angle; }

    /// <summary>
    /// Get object's rotation angle in degrees
    /// </summary>
    public virtual float GetAngleInDegrees( ) { return Calculations.RadiansToDegrees(_angle); }

    /// <summary>
    /// Get object's rotation origin in X axis
    /// </summary>
    public virtual float GetRotationOriginX( ) { return _rotationOriginX; }

    /// <summary>
    /// Get object's rotation origin in Y axis
    /// </summary>
    public virtual float GetRotationOriginY( ) { return _rotationOriginY; }

    /// <summary>
    /// Get object's opacity
    /// </summary>
    public virtual float GetAlpha( ) { return _alpha; }

    /// <summary>
    /// Get object's color
    /// </summary>
    public virtual Color GetColor( ) { return _color; }

    /// <summary>
    /// Get object's interactivness
    /// </summary>
    /// <returns></returns>
    public virtual bool GetInteractive( ) { return _isInteractive; }

    /// <summary>
    /// Get object's visibility
    /// </summary>
    /// <returns></returns>
    public virtual bool GetVisible( ) { return _isVisible; }

    // ============================================================================================================================================ SETTERS

    public virtual void SetParent(GameObject? parent) {
        // Skip if parent is already the same
        if (_parent == parent) return;

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
    }

    /// <summary>
    /// Set object's X position
    /// </summary>
    /// <param name="x">X position</param>
    public virtual void SetX(float x) { _x = x; }

    /// <summary>
    /// Set object's Y position
    /// </summary>
    /// <param name="y">Y position</param>
    public virtual void SetY(float y) { _y = y; }

    /// <summary>
    /// Set object's position
    /// </summary>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    public virtual void SetPosition(float x, float y) {
        _x = x;
        _y = y;
    }

    /// <summary>
    /// Set object's position
    /// </summary>
    public virtual void SetPosition(float xy) => SetPosition(xy, xy);

    /// <summary>
    /// Set object's width
    /// </summary>
    /// <param name="width">Width</param>
    public virtual void SetWidth(float width) {
        if (width < 0) width = 0;
        _width = width;
    }

    /// <summary>
    /// Set object's height
    /// </summary>
    /// <param name="height">Height</param>
    public virtual void SetHeight(float height) {
        if (height < 0) height = 0;
        _height = height;
    }

    /// <summary>
    /// Set object's size
    /// </summary>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    public virtual void SetSize(float width, float height) {
        if (width < 0) width = 0;
        if (height < 0) height = 0;
        _width = width;
        _height = height;
    }

    /// <summary>
    /// Set object's size
    /// </summary>
    public virtual void SetSize(float size) => SetSize(size, size);

    /// <summary>
    /// Set object's position and size
    /// </summary>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    public virtual void SetDimensions(float x, float y, float width, float height) {
        if (width < 0) width = 0;
        if (height < 0) height = 0;
        _width = width;
        _height = height;
        _x = x;
        _y = y;
    }

    /// <summary>
    /// Set object's scale on width
    /// </summary>
    /// <param name="scaleX">Scale</param>
    public virtual void SetScaleX(float scaleX) {
        if (scaleX < 0) scaleX = 0;
        _scaleX = scaleX;
    }

    /// <summary>
    /// Set object's scale on height
    /// </summary>
    /// <param name="scaleY">Scale</param>
    public virtual void SetScaleY(float scaleY) {
        if (scaleY < 0) scaleY = 0;
        _scaleY = scaleY;
    }

    /// <summary>
    /// Set object's scale on both axis
    /// </summary>
    /// <param name="scaleX">Scale X</param>
    /// <param name="scaleY">Scale Y</param>
    public virtual void SetScale(float scaleX, float scaleY) {
        if (scaleX < 0) scaleX = 0;
        if (scaleY < 0) scaleY = 0;
        _scaleX = scaleX;
        _scaleY = scaleY;
    }

    /// <summary>
    /// Set obejct's scale on both axis
    /// </summary>
    public virtual void SetScale(float scale) => SetScale(scale, scale);

    /// <summary>
    /// Set object's origin in X axis
    /// </summary>
    /// <param name="originX">Origin in X axis where 0 is left and 1 is right</param>
    public virtual void SetOriginX(float originX) {
        _originX = originX;
    }

    /// <summary>
    /// Set object's origin in Y axis
    /// </summary>
    /// <param name="originX">Origin in Y axis where 0 is top and 1 is bottom</param>
    public virtual void SetOriginY(float originY) {
        _originY = originY;
    }

    /// <summary>
    /// Set object's origin in both axis
    /// </summary>
    /// <param name="originX"></param>
    /// <param name="originY"></param>
    public virtual void SetOrigin(float originX, float originY) {
        _originX = originX;
        _originY = originY;
    }

    /// <summary>
    /// Set object's origin in both axis
    /// </summary>
    /// <param name="origin"></param>
    public virtual void SetOrigin(float origin) => SetOrigin(origin, origin);

    /// <summary>
    /// Set object's rotation angle in radians
    /// </summary>
    /// <param name="angle">Angle in radians</param>
    public virtual void SetAngle(float angle) {
        _angle = angle;
    }

    /// <summary>
    /// Set object's rotation angle in degrees
    /// </summary>
    /// <param name="angle">Angle in degrees</param>
    public virtual void SetAngleInDegrees(float angle) {
        _angle = Calculations.DegreesToRadians(angle);
    }

    /// <summary>
    /// Set object's rotation origin in X axis
    /// </summary>
    /// <param name="originX">Origin in X axis where 0 is left and 1 is right</param>
    public virtual void SetRotationOriginX(float rotationOriginX) {
        _rotationOriginX = rotationOriginX;
    }

    /// <summary>
    /// Set object's rotation origin in Y axis
    /// </summary>
    /// <param name="originX">Origin in Y axis where 0 is top and 1 is bottom</param>
    public virtual void SetRotationOriginY(float rotationOriginY) {
        _rotationOriginY = rotationOriginY;
    }

    /// <summary>
    /// Set object's rotation origin in both axis
    /// </summary>
    /// <param name="rotationOriginX">Origin in X axis</param>
    /// <param name="rotationOriginY">Origin in Y axis</param>
    public virtual void SetRotationOrigin(float rotationOriginX, float rotationOriginY) {
        _rotationOriginX = rotationOriginX;
        _rotationOriginY = rotationOriginY;
    }

    /// <summary>
    /// Set object's rotation origin in both axis
    /// </summary>
    public virtual void SetRotationOrigin(float rotationOrigin) => SetRotationOrigin(rotationOrigin, rotationOrigin);

    /// <summary>
    /// Set object's opacity
    /// </summary>
    /// <param name="alpha">Alpha. In range of [0-1]</param>
    public virtual void SetAlpha(float alpha) {
        if (alpha < 0) alpha = 0;
        if (alpha > 1) alpha = 1;
        _alpha = alpha;
    }

    /// <summary>
    /// Set object's color via RGBA
    /// </summary>
    public virtual void SetColor(byte r, byte g, byte b, byte a = 255) {
        _color = new Color(r, g, b, a);
    }

    /// <summary>
    /// Set object's color
    /// </summary>
    /// <param name="color"></param>
    public virtual void SetColor(Color color) => SetColor(color.R, color.G, color.B, color.A);

    /// <summary>
    /// Set object's interactivness
    /// </summary>
    /// <param name="state"></param>
    public virtual void SetInteractive(bool state = true) {
        _isInteractive = state;
    }

    /// <summary>
    /// Set object's visibility
    /// </summary>
    /// <param name="state"></param>
    public virtual void SetVisible(bool state) {
        _isVisible = state;
    }

    // ============================================================================================================================================ LOGIC

    /// <summary>
    /// Disposes object
    /// </summary>
    public virtual void Dispose( ) {
        OnHoverIn.Clear( );
        OnHoverOut.Clear( );
        OnClick.Clear( );
        OnPress.Clear( );
        OnRelease.Clear( );
    }

    /// <summary>
    /// Update object's logic
    /// </summary>
    /// <param name="time"><see cref="GameTime"/></param>
    public virtual void Update(GameTime time) {
        // If it's not interactive - skip
        if (!GetInteractive( ))
            return;

        // Mouse is over this object
        if (OnHoverCondition != null && OnHoverCondition.Invoke( )) {
            // Hover in event
            if (!_isHovering) {
                _isHovering = true;
                OnHoverIn.ForEach(e => e?.Invoke(this));
            }

            // Click
            if (OnClickCondition != null && OnClickCondition.Invoke( )) {
                _isPressed = true;
                OnClick.ForEach(e => e?.Invoke(this));
            }

        // Mouse is outside this object
        } else {
            if (_isHovering) {
                _isHovering = false;
                OnHoverOut.ForEach(e => e?.Invoke(this));
            }
        }

        // Check if it is pressed
        if (_isPressed) {
            // Skip if there is no press action
            if (OnPressCondition == null) {
                _isPressed = false;

                // On press
            } else {
                if (OnPressCondition.Invoke( )) {
                    OnPress.ForEach(e => e?.Invoke(this));
                } else {
                    _isPressed = false;
                    OnRelease.ForEach(e => e?.Invoke(this));
                }
            }
        }
    }

    /// <summary>
    /// Render object onto screen
    /// </summary>
    /// <param name="time"><see cref="GameTime"/></param>
    public virtual void Render(GameTime time) { }

    // ============================================================================================================================================ CHILDRENS

    /// <summary>
    /// Checks wheter this object contains given child
    /// </summary>
    /// <param name="child">Child</param>
    public virtual bool ContainsChild(GameObject child) {
        return _children.Contains(child);
    }

    /// <summary>
    /// Adds given object as child
    /// </summary>
    /// <param name="child"></param>
    public virtual void AddChild(GameObject child) {
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

    /// <summary>
    /// Remove child
    /// </summary>
    /// <param name="child">Child</param>
    public virtual void RemoveChild(GameObject child) {
        if (ContainsChild(child)) {
            child.SetParent(null);
            _children.Remove(child);
        }
    }

    // ============================================================================================================================================ HELPERS

    /// <summary>
    /// In-game render position X
    /// </summary>
    public float DisplayX => (_parent != null ? _parent.DisplayX : 0) + GetX( ) - GetOriginX( ) * DisplayWidth;

    /// <summary>
    /// In-game render position Y
    /// </summary>
    public float DisplayY => (_parent != null ? _parent.DisplayY : 0) + GetY( ) - GetOriginY( ) * DisplayHeight;

    /// <summary>
    /// In-game render scale of all nested objects in X axis
    /// </summary>
    public float DisplayScaleX => (_parent != null ? _parent.DisplayScaleX : 1) * GetScaleX( );

    /// <summary>
    /// In-game render scale of all nested objects in Y axis
    /// </summary>
    public float DisplayScaleY => (_parent != null ? _parent.DisplayScaleY : 1) * GetScaleY( );

    /// <summary>
    /// In-game render width
    /// </summary>
    public float DisplayWidth => GetWidth( ) * DisplayScaleX;

    /// <summary>
    /// In-game render height
    /// </summary>
    public float DisplayHeight => GetHeight( ) * DisplayScaleY;

    /// <summary>
    /// In-game rotation origin X
    /// </summary>
    public float RotationOriginX => GetRotationOriginX( ) * GetWidth( );

    /// <summary>
    /// In-game rotation origin Y
    /// </summary>
    public float RotationOriginY => GetRotationOriginY( ) * GetHeight( );

    /// <summary>
    /// Color that includes alpha
    /// </summary>
    public Color OutputColor => GetColor( ) * OutputAlpha;

    /// <summary>
    /// Alpha from all parents
    /// </summary>
    public float OutputAlpha => (_parent != null ? _parent.OutputAlpha : 1) * GetAlpha( );

}