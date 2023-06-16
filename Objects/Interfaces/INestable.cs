namespace VXEngine.Objects.Interfaces;

public interface INestable {

    GameObjectV2 GetParent( );
    GameObjectV2 SetParent(GameObjectV2 parent);

    List<GameObjectV2> GetChildren( );
    bool ContainsChild(GameObjectV2 child);
    void AddChild(GameObjectV2 child);
    void AddChildren(List<GameObjectV2> children);
    void RemoveChild(GameObjectV2 child);
    void RemoveAllChildren();

}
