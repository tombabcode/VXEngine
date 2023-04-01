using Microsoft.Xna.Framework.Graphics;

namespace VXEngine.Models {
    public class FontData {

        public SpriteFont Font { get; private set; }
        public int FontSize { get; private set; }

        public FontData(SpriteFont font, int size) {
            Font = font;
            FontSize = size < 1 ? 1 : size;
        }

    }
}
