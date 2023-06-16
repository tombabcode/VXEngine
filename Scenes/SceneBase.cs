using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;
using VXEngine.Objects;
using VXEngine.Utility;

namespace VXEngine.Scenes;

public abstract class SceneBase : GameObjectV2 {

    protected BasicContentController _content { get; private set; }

    public int SceneID { get; private set; }

    public RenderTarget2D View { get; private set; }

    protected SceneBase(BasicContentController content, BasicConfigController config, int sceneID) : base(config) {
        _content = content;

        SceneID = sceneID;
        View = new RenderTarget2D(content.Device, (int)config.ViewWidth, (int)config.ViewHeight);
    }

    public virtual void OnLoad( ) { }
    public virtual void OnShow( ) { }
    public virtual void OnHide( ) { }

    public virtual void Display( ) => RenderUtility.DisplayOntoMainScreen(_content, _config, ( ) => RenderUtility.DisplayScene(_content, this));

}
