using Microsoft.Xna.Framework;
using VXEngine.Scenes;
using VXEngine.Utility;

namespace VXEngine.Controllers {
    public class BasicSceneController {

        private bool _isSceneChanging = true;

        protected Dictionary<int, SceneBase> _scenes { get; private set; } = new Dictionary<int, SceneBase>( );

        public int CurrentSceneID { get; protected set; } = -1;
        public SceneBase CurrentScene { get; protected set; } = null;

        public virtual void AddScene(int id, SceneBase scene) {
            if (!_scenes.ContainsKey(id)) {
                _scenes.Add(id, scene);
                scene.OnLoad( );
            }
        }

        public virtual void ChangeScene(int id) {
            if (!_scenes.ContainsKey(id) || CurrentSceneID == id)
                return;

            if (CurrentScene != null)
                CurrentScene.OnHide( );

            _isSceneChanging = true;

            CurrentScene = _scenes[id];
            CurrentScene.OnShow( );
        }

        public virtual void Update(GameTime time) {
            _isSceneChanging = false;

            if (CurrentScene != null)
                CurrentScene.Update(time);
        }

        public virtual void Render(GameTime time) {
            if (CurrentScene == null || _isSceneChanging)
                return;

            CurrentScene.Render(time);
            CurrentScene.Display( );
        }

    }
}
