namespace VXEngine.Objects.Interfaces;
public interface IInteractable {

    void EnableInteraction( );
    void DisableInteraction( );
    GameObject SetInteraction(bool isEnabled);

}
