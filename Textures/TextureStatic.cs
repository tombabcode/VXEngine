using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VXEngine.Textures {
    /// <summary>
    /// Static image
    /// </summary>
    public sealed class TextureStatic : TextureBase {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="image"><see cref="Texture2D"/></param>
        public TextureStatic(Texture2D image) : base(image) {
            FrameWidth = Image.Width;
            FrameHeight = Image.Height;
            FramesColumns = 1;
            FramesRows = 1;
            FramesTotal = 1;
        }

        /// <inheritdoc/>
        public override Rectangle GetSource(int id = 0) {
            if (Image == null)
                return Rectangle.Empty;
            return new Rectangle(0, 0, FrameWidth, FrameHeight);
        }

    }
}
