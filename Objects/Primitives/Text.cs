using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;
using VXEngine.Controllers;
using VXEngine.Types;

namespace VXEngine.Objects.Primitives;

public class Text : GameObject {

    protected struct Sentence {
        public Color Color = Color.White;
        public SpriteFont Font = null;
        public string Text = null;
        public int Offset = 0;

        public Sentence(string text, int offset, Color color, SpriteFont font) {
            Text = text;
            Offset = offset;
            Color = color;
            Font = font;
        }
    };

    protected struct Line {
        public List<Sentence> Sentences;
        public int X = 0;
        public int Y = 0;

        public Line(List<Sentence> sentences, int x, int y) {
            Sentences = sentences;
            X = x;
            Y = y;
        }
    }

    protected BasicContentController _content { get; private set; } = null;

    protected Func<string> _sourceText = null;
    protected string _renderText = null;

    protected int _lineSpacing = 0;
    protected bool _isJustifying = false;
    protected TextAlign _textAlign = TextAlign.Left;

    // Splitted text data, contains text and tags (<b>, <i>, <color=#FFF> etc)
    protected List<string> _textData = new List<string>( );

    protected List<Line> _lines = new List<Line>();

    public Text(BasicContentController content, BasicConfigController config, BasicInputController input, string text) : base(config, input) {
        _content = content;
        _sourceText = ( ) => text;
    }

    public override void Dispose( ) {
        base.Dispose( );

        _textData.Clear( );
        _textData = null;
    }

    public void UpdateTextData( ) {
        // Clear current data
        _textData.Clear( );

        // Get source text
        string source = _sourceText?.Invoke( );

        // If current text is null - skip
        if (source == null)
            return;

        // Separate text by spaces
        string[] splitBySpace = source.Split(null);

        for (int i = 0; i < splitBySpace.Length; i++) {
            // Skip if word is empty
            if (string.IsNullOrWhiteSpace(splitBySpace[i]))
                continue;

            // Split by tag (like <b>, <i>, etc)
            string[] splitByTag = Regex.Matches(splitBySpace[i], "<[^>]+>|[^<>]+").Cast<Match>( ).Select(match => match.Value).ToArray( );

            // Add splitted data to list
            for (int j = 0; j < splitByTag.Length; j++) {
                if (string.IsNullOrWhiteSpace(splitByTag[j]))
                    continue;

                _textData.Add(splitByTag[j]);
            }
        }
    }

    public void Rebuild( ) {

    }

    public override void Update(GameTime time, bool updateOnlyThisObject = false) {
        // Updates
        if (IsColorUpdateRequired) UpdateColor(time);
        if (IsOpacityUpdateRequired) UpdateOpacity(time);
        if (IsSizeUpdateRequired) UpdateSize(time);
        if (IsPositionUpdateRequired) UpdatePosition(time);

        // If currently displaying text differs from source text - rebuild
        if (_renderText != _sourceText?.Invoke( ))
            UpdateTextData( );

        // Update children
        if (!updateOnlyThisObject)
            _children.ForEach(child => child.Update(time));
    }

    public override void Render(GameTime time, bool renderOnlyThisObject = false) {
        if (DisplayOpacity <= 0) return;

        if (_lines.Count > 0) {
            foreach (Line line in _lines)
                foreach (Sentence sentence in line.Sentences) {
                    _content.Canvas.DrawString(
                        sentence.Font,
                        sentence.Text,
                        new Vector2(DisplayX + line.X + sentence.Offset, DisplayY + line.Y),
                        sentence.Color,
                        0,
                        new Vector2(0, 0),
                        new Vector2(1, 1),
                        SpriteEffects.None,
                        _depth
                    );
                }
        }

        base.Render(time, renderOnlyThisObject);
    }

}
