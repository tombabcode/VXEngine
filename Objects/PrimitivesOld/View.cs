using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;

namespace VXEngine.Objects.PrimitivesOld;

public class View : GameObjectOld {

    protected BasicContentController _content { get; private set; }
    protected BasicConfigController _config { get; private set; }

    public RenderTarget2D Renderer { get; private set; } = null;

    public View(BasicContentController content, BasicConfigController config, BasicInputController input, int width, int height) : base(input) {
        if (width < 1) width = 1;
        if (height < 1) height = 1;

        _content = content;
        _config = config;
        _width = width;
        _height = height;

        Rebuild( );
    }

    public override void SetWidth(float width) {
        base.SetWidth(width);
        Rebuild( );
    }

    public override void SetHeight(float height) {
        base.SetHeight(height);
        Rebuild( );
    }

    public override void SetSize(float width, float height) {
        base.SetSize(width, height);
        Rebuild( );
    }

    public void Rebuild( ) {
        Renderer = new RenderTarget2D(_content.Device, (int)_width, (int)_height);
    }

    public void Display( ) {
        _content.Canvas.Draw(Renderer, new Vector2(DisplayX, DisplayY), Color.White);
    }

}
