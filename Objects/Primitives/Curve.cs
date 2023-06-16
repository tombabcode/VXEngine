using VXEngine.Controllers;

namespace VXEngine.Objects.Primitives;

public class Curve : GameObject {

    /// <summary>
    /// Content reference
    /// </summary>
    private BasicContentController _content;

    public Curve(BasicContentController content, BasicInputController input) : base(input) {
        _content = content;
    }

}
