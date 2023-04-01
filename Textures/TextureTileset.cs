using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VXEngine.Textures;

/// <summary>
/// Static image
/// </summary>
public sealed class TextureTileset : TextureBase {

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="image"><see cref="Texture2D"/></param>
    public TextureTileset(Texture2D image, int xColumns = 1, int yRows = 1) : base(image) {
        if (xColumns < 1) xColumns = 1;
        if (xColumns > Image.Width) xColumns = Image.Width;
        if (yRows < 1) yRows = 1;
        if (yRows > Image.Height) yRows = Image.Height;

        FrameWidth = Image.Width / xColumns;
        FrameHeight = Image.Height / yRows;
        FramesColumns = xColumns;
        FramesRows = yRows;
        FramesTotal = FramesColumns * FramesRows;
    }

    /// <inheritdoc/>
    public override Rectangle GetSource(int id = 0) {
        if (Image == null)
            return Rectangle.Empty;
        return new Rectangle(
            id % FramesColumns * FrameWidth,
            id / FramesColumns * FrameHeight,
            FrameWidth,
            FrameHeight
        );
    }

}
