using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;
using VXEngine.Objects;
using VXEngine.Scenes;
using VXEngine.Textures;

namespace VXEngine.Utility {
    public static class RenderUtility {

        public static void DisplayOntoMainScreen(BasicContentController content, BasicConfigController config, Action logic = null) {
            content.Device.SetRenderTarget(null);
            content.Device.Clear(Color.Black);
            content.Canvas.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, config.IsPixelart ? SamplerState.PointClamp : SamplerState.AnisotropicClamp);
                logic?.Invoke( );
            content.Canvas.End( );
        }

        public static void DisplayScene(BasicContentController content, SceneBase scene) {
            content.Canvas.Draw(
                scene.View,
                new Vector2(0, 0),
                Color.White
            );
        }

        public static void RenderScene(BasicContentController content, BasicConfigController config, RenderTarget2D scene, SamplerState state = null, Camera camera = null, Color? color = null, Action sceneLogic = null) {
            content.Device.SetRenderTarget(scene);
            content.Device.Clear(color ?? Color.Transparent);
            content.Canvas.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, state != null ? state : config.IsPixelart ? SamplerState.PointClamp : SamplerState.AnisotropicClamp, null, null, null, camera?.Matrix);
            sceneLogic?.Invoke( );
            content.Canvas.End( );
        }

        public static void DisplayRawTexture(SpriteBatch canvas, Texture2D texture, float x, float y, Rectangle? source) {
            canvas.Draw(
                texture,
                new Vector2(x, y),
                source,
                Color.White
            );
        }

    }
}
