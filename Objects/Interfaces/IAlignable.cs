namespace VXEngine.Objects.Interfaces;

public interface IAlignable {

    GameObject SetAlignX(float align);
    GameObject SetAlignY(float align);
    GameObject SetAlign(float align);
    GameObject SetAlign(float alignX, float alignY);
    float GetAlignX();
    float GetAlignY();

}
