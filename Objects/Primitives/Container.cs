using Microsoft.Xna.Framework;
using VXEngine.Controllers;

namespace VXEngine.Objects.Primitives;

public class Container : GameObject {

    public Container(BasicConfigController config, BasicInputController input) : base(config, input) { }

    // Render
    public override void Render(GameTime time, bool renderOnlyThisObject = false) {
        if (DisplayOpacity <= 0) return;
        if (DisplayWidth <= 0 || DisplayHeight <= 0) {
            base.Render(time, renderOnlyThisObject);
            return;
        }

        base.Render(time, renderOnlyThisObject);
    }

}
