using Microsoft.Xna.Framework;

namespace VXEngine.Textures {
    /// <summary>
    /// Instance of the texture
    /// </summary>
    public sealed class TextureInstance {

        /// <summary>
        /// Original data
        /// </summary>
        public TextureBase Data { get; private set; }

        /// <summary>
        /// Current frame
        /// </summary>
        public float FrameID { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        public TextureInstance(TextureBase data) {
            Data = data;
        }

        /// <summary>
        /// Get source rectangle
        /// </summary>
        public Rectangle GetSource( ) {
            if (Data == null)
                return Rectangle.Empty;
            return Data.GetSource((int)FrameID);
        }

    }
}
