using VXEngine.Controllers;

namespace VXEngine.Objects.Primitives; 

/// <summary>
/// Non-visible, container that holds other elements
/// </summary>
public class Container : GameObject {

    /// <summary>
    /// Constructor
    /// </summary>
    public Container(BasicInputController input) : base(input) { }

}
