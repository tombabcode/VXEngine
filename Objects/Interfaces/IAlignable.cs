namespace VXEngine.Objects.Interfaces;

public interface IAlignable {

    GameObjectV2 SetAlignX(float align);
    GameObjectV2 SetAlignY(float align);
    GameObjectV2 SetAlign(float align);
    GameObjectV2 SetAlign(float alignX, float alignY);
    float GetAlignX();
    float GetAlignY();

}
