using Microsoft.Xna.Framework;
using VXEngine.Utility;

namespace VXEngine.Objects;

/// <summary>
/// In-game camera
/// </summary>
public class Camera {

    /// <summary>
    /// Matrix of all the transformations
    /// </summary>
    public Matrix Matrix { get; protected set; }

    /// <summary>
    /// Offset to the screen. Static, non-changing
    /// </summary>
    public Vector2 Offset { get; protected set; } = new Vector2(0, 0);

    /// <summary>
    /// Offset to the screen. Dynamic, changing based on "where we're looking"
    /// </summary>
    public Vector2 Target { get; protected set; } = new Vector2(0, 0);

    /// <summary>
    /// Destination of <see cref="Target"/>, for smooth camera movement. It's not used when <see cref="IsMoveSmooth"/> is set to <see langword="false"/>
    /// </summary>
    public Vector2 TargetTowards { get; protected set; } = new Vector2(0, 0);

    /// <summary>
    /// Describes how smoothly the camera will move. It's not used when <see cref="IsMoveSmooth"/> is set to <see langword="false"/>
    /// </summary>
    public float MoveInertia { get; protected set; } = 20;

    /// <summary>
    /// Enables or disables the camera's smooth movement
    /// </summary>
    public bool IsMoveSmooth { get; set; } = false;

    /// <summary>
    /// Value of camera' zoom. In range of (<see cref="ZoomMin"/>; <see cref="ZoomMax"/>)
    /// </summary>
    public float Zoom { get; protected set; } = 1.0f;

    /// <summary>
    /// Destination of <see cref="Zoom"/>, for smooth camera zooming. It's not used when <see cref="IsZoomSmooth"/> is set to <see langword="false"/>
    /// </summary>
    public float ZoomTowards { get; protected set; } = 1.0f;

    /// <summary>
    /// Minimal value of <see cref="Zoom"/>. Cannot be negative. Must be less or equal to <see cref="ZoomMax"/>
    /// </summary>
    public float ZoomMin { get; protected set; } = 0.5f;

    /// <summary>
    /// Maximum value of <see cref="Zoom"/>. Cannot be negative. Must be greater or equal to <see cref="ZoomMin"/>
    /// </summary>
    public float ZoomMax { get; protected set; } = 2.0f;

    /// <summary>
    /// How much should <see cref="Zoom"/> change on zooming action. Cannot be negative
    /// </summary>
    public float ZoomStep { get; protected set; } = 0.25f;

    /// <summary>
    /// Describes how smoothly the camera will zoom. It's not used when <see cref="IsZoomSmooth"/> is set to <see langword="false"/>
    /// </summary>
    public float ZoomInertia { get; protected set; } = 20;

    /// <summary>
    /// Enables or disables the camera's smooth zooming
    /// </summary>
    public bool IsZoomSmooth { get; set; } = false;

    /// <summary>
    /// Event that is triggered when camera is moving
    /// </summary>
    public Action OnCameraMovementUpdate { get; set; } = null;

    /// <summary>
    /// Event that is triggered when camera is zooming
    /// </summary>
    public Action OnCameraZoomUpdate { get; set; } = null;

    /// <summary>
    /// Default constructor
    /// </summary>
    public Camera( ) { }

    /// <summary>
    /// Constructor with default offset as <see cref="Vector2"/>
    /// </summary>
    /// <param name="offset"></param>
    public Camera(Vector2 offset) { Offset = offset; }

    /// <summary>
    /// Constructor with default offset as <see langword="float"/>
    /// </summary>
    /// <param name="offsetX"></param>
    /// <param name="offsetY"></param>
    public Camera(float offsetX, float offsetY) { Offset = new Vector2(offsetX, offsetY); }

    /// <summary>
    /// Move camera to given position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="force">Skip smooth effect if enabled</param>
    public virtual void LookAt(float x, float y, bool force = false) {
        TargetTowards = new Vector2(x, y);
        if (!IsMoveSmooth || force) {
            Target = new Vector2(x, y);
            OnCameraMovementUpdate?.Invoke( );
        }
    }
    public virtual void LookAt(Vector2 target, bool force = false) => LookAt(target.X, target.Y, force);
    public virtual void LookAt(float xy, bool force = false) => LookAt(xy, xy, force);

    /// <summary>
    /// Move camera by given value
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="force">Skip smooth effect if enabled</param>
    public virtual void LookBy(float x, float y, bool force = false) {
        TargetTowards -= new Vector2(x, y);
        if (!IsMoveSmooth || force) {
            Target -= new Vector2(x, y);
            OnCameraMovementUpdate?.Invoke( );
        }
    }
    public virtual void LookBy(Vector2 target, bool force = false) => LookBy(target.X, target.Y, force);
    public virtual void LookBy(float xy, bool force = false) => LookBy(xy, xy, force);

    /// <summary>
    /// Sets movement intertia
    /// </summary>
    /// <param name="inertia"></param>
    public virtual void SetMoveInertia(float inertia = 20f) {
        if (inertia < 0) inertia = 0;
        MoveInertia = inertia;
    }

    /// <summary>
    /// Increase the zoom
    /// </summary>
    /// <param name="force">Skip smooth effect if enabled</param>
    public virtual void ZoomIn(bool force = false) {
        ZoomTowards = ZoomTowards + ZoomStep;
        if (ZoomTowards > ZoomMax) { ZoomTowards = ZoomMax; }
        if (!IsZoomSmooth || force) {
            Zoom = ZoomTowards;
            OnCameraZoomUpdate?.Invoke( );
        }
    }

    /// <summary>
    /// Decrease the zoom
    /// </summary>
    /// <param name="force">Skip smooth effect if enabled</param>
    public virtual void ZoomOut(bool force = false) {
        ZoomTowards = ZoomTowards - ZoomStep;
        if (ZoomTowards < ZoomMin) { ZoomTowards = ZoomMin; }
        if (!IsZoomSmooth || force) {
            Zoom = ZoomTowards;
            OnCameraZoomUpdate?.Invoke( );
        }
    }

    /// <summary>
    /// Set the zoom
    /// </summary>
    /// <param name="zoom"></param>
    /// <param name="force"></param>
    public virtual void SetZoom(float zoom, bool force = false) {
        ZoomTowards = zoom;
        if (ZoomTowards > ZoomMax) { ZoomTowards = ZoomMax; }
        if (ZoomTowards < ZoomMin) { ZoomTowards = ZoomMin; }
        if (!IsZoomSmooth || force) {
            Zoom = ZoomTowards; 
            OnCameraZoomUpdate?.Invoke( );
        }
    }

    /// <summary>
    /// Sets zoom intertia
    /// </summary>
    /// <param name="inertia"></param>
    public virtual void SetZoomInertia(float inertia = 20f) {
        if (inertia < 0) inertia = 0;
        ZoomInertia = inertia;
    }

    public virtual void Update(GameTime time) {
        // Smoothen zoom if required
        if (IsZoomSmooth && Zoom != ZoomTowards) {
            Zoom = Math.Abs(Zoom - ZoomTowards) < .01f
                ? ZoomTowards
                : Motion.Smooth(Zoom, ZoomTowards, (float)time.ElapsedGameTime.TotalMilliseconds, ZoomInertia);
            OnCameraZoomUpdate?.Invoke( );
        }

        // Smoothen movement if required
        if (IsMoveSmooth && (Target.X != TargetTowards.X || Target.Y != TargetTowards.Y)) {
            Target = Math.Abs(Target.X - TargetTowards.X) < .01f && Math.Abs(Target.Y - TargetTowards.Y) < .01f
                ? TargetTowards
                : new Vector2(
                    Motion.Smooth(Target.X, TargetTowards.X, (float)time.ElapsedGameTime.TotalMilliseconds, MoveInertia),
                    Motion.Smooth(Target.Y, TargetTowards.Y, (float)time.ElapsedGameTime.TotalMilliseconds, MoveInertia)
                );
            OnCameraMovementUpdate?.Invoke( );
        }
        
        // Update matrix
        Matrix = Matrix.CreateTranslation(new Vector3(Offset / Zoom, 0)) *
                 Matrix.CreateScale(Zoom) *
                 Matrix.CreateTranslation(new Vector3(-Target * Zoom, 0));
    }

}
