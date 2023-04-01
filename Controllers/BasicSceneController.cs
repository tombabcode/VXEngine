using Microsoft.Xna.Framework;
using VXEngine.Scenes;

namespace VXEngine.Controllers {
    /// <summary>
    /// Controls all scenes
    /// </summary>
    public class BasicSceneController {

        /// <summary>
        /// Container for all scenes. Each has unique ID
        /// </summary>
        protected Dictionary<int, SceneBase> _scenes;

        /// <summary>
        /// Current scene's unique ID
        /// </summary>
        public int CurrentSceneID { get; protected set; } = -1;
        
        /// <summary>
        /// Current scene
        /// </summary>
        public SceneBase CurrentScene { get; protected set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public BasicSceneController( ) {
            _scenes = new Dictionary<int, SceneBase>( );
        }

        /// <summary>
        /// Adds new scene
        /// </summary>
        /// <param name="id">Unique ID</param>
        /// <param name="scene">Given scene</param>
        public virtual void AddScene(int id, SceneBase scene) {
            if (!_scenes.ContainsKey(id)) {
                _scenes.Add(id, scene);
                scene.OnInitialize( );
            }
        }

        /// <summary>
        /// Switch between scenes
        /// </summary>
        /// <param name="id">New scene's ID</param>
        public virtual void ChangeScene(int id) {
            if (!_scenes.ContainsKey(id) || CurrentSceneID == id)
                return;

            if (CurrentScene != null)
                CurrentScene.OnHide( );
            CurrentScene = _scenes[id];
            CurrentScene.OnShow( );
        }

        /// <summary>
        /// Update method
        /// </summary>
        public virtual void Update(GameTime time) {
            if (CurrentScene != null)
                CurrentScene.Update(time);
        }

        /// <summary>
        /// Render & Display method
        /// </summary>
        public virtual void Display(GameTime time) {
            if (CurrentScene == null)
                return;

            CurrentScene.Render(time);
            CurrentScene.Display( );
        }

    }
}
