using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VXEngine.Textures;

/// <summary>
/// Stores data of image and frames for single texture
/// </summary>
public abstract class TextureBase {

    /// <summary>
    /// Original image
    /// </summary>
    public Texture2D Image { get; private set; }

    /// <summary>
    /// Total image width
    /// </summary>
    public int ImageWidth { get; private set; } = 0;

    /// <summary>
    /// Total image height
    /// </summary>
    public int ImageHeight { get; private set; } = 0;

    /// <summary>
    /// Single frame's width
    /// </summary>
    public int FrameWidth { get; protected set; } = 0;

    /// <summary>
    /// Single frame's height
    /// </summary>
    public int FrameHeight { get; protected set; } = 0;

    /// <summary>
    /// Frames count in row
    /// </summary>
    public int FramesRows { get; protected set; } = 1;

    /// <summary>
    /// Frames count in columns
    /// </summary>
    public int FramesColumns { get; protected set; } = 1;

    /// <summary>
    /// Total number of frames
    /// </summary>
    public int FramesTotal { get; protected set; } = 1;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="image">Original texture image</param>
    public TextureBase(Texture2D image) {
        Image = image;

        if (Image != null) {
            ImageWidth = Image.Width;
            ImageHeight = Image.Height;
        }
    }

    /// <summary>
    /// Get new instance of this texture
    /// </summary>
    /// <returns><see cref="TextureInstance"/></returns>
    public TextureInstance GetInstance( )
        => new TextureInstance(this);

    /// <summary>
    /// Get new instance of this texture with specific ID
    /// </summary>
    /// <returns><see cref="TextureInstance"/></returns>
    public TextureInstance GetInstance(int frameID) {
        TextureInstance instance = new TextureInstance(this);
        instance.FrameID = frameID;
        return instance;
    }

    /// <summary>
    /// Get new instance of this texture with random ID
    /// </summary>
    /// <returns><see cref="TextureInstance"/></returns>
    public TextureInstance GetInstance(Random rand) {
        TextureInstance instance = new TextureInstance(this);
        instance.FrameID = rand.Next(0, instance.Data.FramesTotal);
        return instance;
    }

    /// <summary>
    /// Get source rectangle
    /// </summary>
    /// <param name="id">Frame ID</param>
    public abstract Rectangle GetSource(int id = 0);

}
