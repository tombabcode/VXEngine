namespace VXEngine.Objects.Interfaces;

public interface INestable {

    GameObject GetParent( );
    GameObject SetParent(GameObject parent);

    List<GameObject> GetChildren( );
    bool ContainsChild(GameObject child);
    void AddChild(GameObject child);
    void AddChildren(List<GameObject> children);
    void RemoveChild(GameObject child);
    void RemoveAllChildren();

}
