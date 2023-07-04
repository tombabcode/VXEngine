namespace VXEngine.Objects.Interfaces;

public interface ISizeable {

    float GetWidth( );
    float GetHeight( );

    GameObject SetWidth(float width);
    GameObject SetHeight(float height);
    GameObject SetSize(float size);
    GameObject SetSize(float width, float height);

    void AddWidth(float value);
    void AddHeight(float value);
    void AddSize(float width, float height);
    void AddSize(float size);

    GameObject SetWidthAsPercent(float width);
    GameObject SetHeightAsPercent(float height);
    GameObject SetSizeAsPercent(float size);
    GameObject SetSizeAsPercent(float width, float height);

    void AddWidthAsPercent(float value);
    void AddHeightAsPercent(float value);
    void AddSizeAsPercent(float width, float height);
    void AddSizeAsPercent(float size);

}
