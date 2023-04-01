using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;
using VXEngine.Objects;

namespace VXEngine.Utility {
    public static class RenderUtility {

        public static void DisplayView(BasicContentController content, BasicConfigController config, RenderTarget2D view, int x, int y) {
            content.Canvas.Draw(view, new Rectangle(x, y, (int)(config.ViewWidth * config.ViewScaleX), (int)(config.ViewHeight * config.ViewScaleY)), Color.White);
        }

        public static void DisplayScene(BasicContentController content, BasicConfigController config, RenderTarget2D scene) {
            content.Canvas.Draw(scene, new Rectangle((int)config.ViewOffsetX, (int)config.ViewOffsetY, (int)(config.ViewWidth * config.ViewScaleX), (int)(config.ViewHeight * config.ViewScaleY)), Color.White);
        }

        public static void RenderFinalScene(BasicContentController content, BasicConfigController config, RenderTarget2D scene, Action logic = null) {
            content.Device.SetRenderTarget(null);
            content.Device.Clear(Color.Black);
            content.Canvas.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, config.IsPixelart ? SamplerState.PointClamp : SamplerState.AnisotropicClamp);
            logic?.Invoke( );
            content.Canvas.End( );
        }

        public static void RenderScene(BasicContentController content, BasicConfigController config, RenderTarget2D scene, SamplerState state = null, Camera camera = null, Action sceneLogic = null) {
            content.Device.SetRenderTarget(scene);
            content.Device.Clear(Color.Transparent);
            content.Canvas.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, state != null ? state : config.IsPixelart ? SamplerState.PointClamp : SamplerState.AnisotropicClamp, null, null, null, camera?.Matrix);
            sceneLogic?.Invoke( );
            content.Canvas.End( );
        }

    }
}
