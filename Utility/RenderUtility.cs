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

        public static void DisplayScene(BasicContentController content, BasicConfigController config, SceneBase scene) {
            content.Canvas.Draw(
                scene.View,
                new Rectangle(
                    (int)config.ViewOffsetX,
                    (int)config.ViewOffsetY,
                    (int)(config.ViewWidth * config.ViewScaleX),
                    (int)(config.ViewHeight * config.ViewScaleY)
                ),
                Color.White
            );
        }

        public static void RenderScene(BasicContentController content, BasicConfigController config, RenderTarget2D scene, Action sceneLogic, SpriteSortMode? sortMode = null, BlendState? blendState = null, SamplerState? sampler = null, DepthStencilState? depthStencilState = null, RasterizerState? rasterizerState = null, Effect? effect = null, Camera? camera = null, Color? backgroundColor = null)
        {
            content.Device.SetRenderTarget(scene);
            content.Device.Clear(backgroundColor ?? Color.Transparent);
            content.Canvas.Begin(
                sortMode ?? SpriteSortMode.Deferred,
                blendState ?? BlendState.AlphaBlend,
                sampler != null ? sampler : (config.IsPixelart ? SamplerState.PointClamp : SamplerState.AnisotropicClamp),
                depthStencilState,
                rasterizerState,
                effect,
                camera?.Matrix
            );
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
