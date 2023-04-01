using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;
using VXEngine.Utility;

namespace VXEngine.Scenes {
    /// <summary>
    /// Base for all scenes
    /// </summary>
    public abstract class SceneBase {

        /// <summary>
        /// Content reference
        /// </summary>
        protected BasicContentController _content { get; private set; }

        /// <summary>
        /// Config reference
        /// </summary>
        protected BasicConfigController _config { get; private set; }

        /// <summary>
        /// Scene unique ID
        /// </summary>
        public int SceneID { get; private set; }

        /// <summary>
        /// Target view
        /// </summary>
        public RenderTarget2D View { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="content"><see cref="BasicContentController"/></param>
        /// <param name="config"><see cref="BasicConfigController"</param>
        /// <param name="sceneID">Scene unique ID</param>
        public SceneBase(BasicContentController content, BasicConfigController config, int sceneID) {
            _content = content;
            _config = config;

            SceneID = sceneID;
            View = new RenderTarget2D(content.Device, (int)config.ViewWidth, (int)config.ViewHeight);
        }

        /// <summary>
        /// On initialization event
        /// </summary>
        public virtual void OnInitialize( ) { }

        /// <summary>
        /// On show scene event
        /// </summary>
        public virtual void OnShow( ) { }

        /// <summary>
        /// On hide scene event
        /// </summary>
        public virtual void OnHide( ) { }

        /// <summary>
        /// Update method
        /// </summary>
        public abstract void Update(GameTime time);

        /// <summary>
        /// Render method
        /// </summary>
        public abstract void Render(GameTime time);

        /// <summary>
        /// Displays whole scene onto screen
        /// </summary>
        public virtual void Display( ) => RenderUtility.RenderFinalScene(_content, _config, View, () => RenderUtility.DisplayScene(_content, _config, View));

    }
}
