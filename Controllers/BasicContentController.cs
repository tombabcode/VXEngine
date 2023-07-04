using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Models;
using VXEngine.Textures;
using VXEngine.Types;

namespace VXEngine.Controllers {
    /// <summary>
    /// Controller for handling assets throught the applicaiton
    /// </summary>
    public class BasicContentController {

        /// <summary>
        /// Core canvas
        /// </summary>
        public SpriteBatch Canvas { get; private set; }

        /// <summary>
        /// Device
        /// </summary>
        public GraphicsDevice Device { get; private set; }

        /// <summary>
        /// Device manager
        /// </summary>
        public GraphicsDeviceManager DeviceMNG { get; private set; }

        /// <summary>
        /// Content manager
        /// </summary>
        protected ContentManager _content { get; private set; }

        /// <summary>
        /// List of fonts
        /// </summary>
        protected Dictionary<int, FontData> _fonts { get; private set; }

        /// <summary>
        /// Basic single-pixel texture
        /// </summary>
        public TextureStatic TexturePixel { get; private set; }

        /// <summary>
        /// Contructor
        /// </summary>
        public BasicContentController(SpriteBatch canvas, GraphicsDevice device, ContentManager content, GraphicsDeviceManager manager) {
            Canvas = canvas;
            Device = device;
            DeviceMNG = manager;
            _content = content;
            _fonts = new Dictionary<int, FontData>( );
        }

        /// <summary>
        /// Loads all in-game assets
        /// </summary>
        public virtual void LoadAssets( ) {
            Texture2D texture = new Texture2D(Device, 1, 1);
            texture.SetData(new Color[] { Color.White });
            TexturePixel = new TextureStatic(texture);
        }

        /// <summary>
        /// Loads given font and assign it to given ID
        /// </summary>
        public virtual void LoadFont(FontData font) {
            if (font != null && !_fonts.ContainsKey(font.ID))
                _fonts.Add(font.ID, font);
        }

        /// <summary>
        /// Loads given font and assign it to given ID
        /// </summary>
        public virtual void LoadFont(int id, int size, string regularPath, string boldPath = null, string italicPath = null, string boldItalicPath = null) {
            if (!_fonts.ContainsKey(id))
                _fonts.Add(id, new FontData(
                    id,
                    regularPath != null ? _content.Load<SpriteFont>(regularPath) : null,
                    boldPath != null ? _content.Load<SpriteFont>(boldPath) : null,
                    italicPath != null ? _content.Load<SpriteFont>(italicPath) : null,
                    boldItalicPath != null ? _content.Load<SpriteFont>(boldItalicPath) : null,
                    size)
                );
        }

        /// <summary>
        /// Gets font data with given ID
        /// </summary>
        public virtual FontData GetFontData(int id) {
            if (_fonts.ContainsKey(id))
                return _fonts[id];
            return null;
        }

        /// <summary>
        /// Gets font with given ID and style
        /// </summary>
        public virtual SpriteFont GetFont(int id, FontStyle style = FontStyle.Regular) {
            if (_fonts.ContainsKey(id))
                return _fonts[id].GetStyle(style);
            return null;
        }
    }
}