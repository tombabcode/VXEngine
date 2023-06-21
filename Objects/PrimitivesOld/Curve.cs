using VXEngine.Controllers;

namespace VXEngine.Objects.PrimitivesOld;

public class Curve : GameObjectOld {

    /// <summary>
    /// Content reference
    /// </summary>
    private BasicContentController _content;

    public Curve(BasicContentController content, BasicInputController input) : base(input) {
        _content = content;
    }

}
