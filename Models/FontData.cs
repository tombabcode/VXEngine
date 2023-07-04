using Microsoft.Xna.Framework.Graphics;
using VXEngine.Types;

namespace VXEngine.Models {
    public class FontData {

        public int ID { get; private set; }

        private SpriteFont _fontRegular = null;
        private SpriteFont _fontBold = null;
        private SpriteFont _fontItalic = null;
        private SpriteFont _fontBoldItalic = null;

        public int BaseSize { get; private set; }

        public SpriteFont FontRegular => _fontRegular;
        public SpriteFont FontBold => _fontBold ?? _fontRegular;
        public SpriteFont FontItalic => _fontItalic ?? _fontRegular;
        public SpriteFont FontBoldItalic => _fontBoldItalic ?? _fontRegular;

        public FontData(int id, SpriteFont regular, SpriteFont bold = null, SpriteFont italic = null, SpriteFont boldItalic = null, int fontSize = 64) {
            ID = id;
            BaseSize = fontSize < 1 ? 1 : fontSize;
            _fontRegular = regular;
            _fontBold = bold;
            _fontItalic = italic;
            _fontBoldItalic = boldItalic;
        }

        public SpriteFont GetStyle(FontStyle style = FontStyle.Regular) => style switch {
            FontStyle.Bold => FontBold,
            FontStyle.Italic => FontItalic,
            FontStyle.BoldItalic => FontBoldItalic,
            _ => FontRegular,
        };

    }
}
