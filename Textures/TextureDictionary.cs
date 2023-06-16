using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;
using VXEngine.Utility;

namespace VXEngine.Textures;

public class TextureDictionary : TextureBase {

    private Texture2D _source;
    private Dictionary<int, TextureBase> _data;

    public TextureDictionary(Texture2D image) : base(image) {
        _source = image;
        _data = new Dictionary<int, TextureBase>( );
    }

    public void Add(BasicContentController content, int id, int x, int y, int width, int height) {
        if (_data.ContainsKey(id))
            throw new ArgumentException("The object with given ID already exists");

        _data.Add(id, new TextureStatic(TextureUtility.GetPart(content, _source, x, y, width, height)));
    }

    public TextureBase GetTexture(int id) => _data.ContainsKey(id) ? _data[id] : null;

    public override Rectangle GetSource(int id = 0) => throw new InvalidOperationException("Cannot return source of the dictionary texture");

}
