using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;
using VXEngine.Controllers;
using VXEngine.Models;
using VXEngine.Types;

namespace VXEngine.Objects.Primitives;

public class Text : GameObject {

    private enum TextCommand {
        NONE,
        BOLD,
        BOLD_END,
        ITALIC,
        ITALIC_END,
        COLOR,
        COLOR_END,
        BREAK_LINE
    };

    // Sentence model (one or more words that share common font style and color
    protected struct Word {
        public Color Color = Color.White;
        public SpriteFont Font = null;
        public string Text = null;
        public float Width = 0;

        public Word(string text, Color color, SpriteFont font) {
            Text = text;
            Color = color;
            Font = font;
            Width = Font.MeasureString(Text).X;
        }
    };

    // Single text line that contains words
    protected struct Line {
        public List<Word> Words;
        public int TotalWidth = 0;

        public Line(List<Word> words, int totalWidth) {
            Words = words;
            TotalWidth = totalWidth < 0 ? 0 : totalWidth;
        }
    }

    // Reference
    protected BasicContentController _content { get; private set; } = null;

    // Flag
    protected bool _isRebuildRequired = true;

    // Text data
    protected Func<string> _sourceText = null;
    protected string _renderText = null;

    // Splitted text data, contains text and tags (<b>, <i>, <color=#FFF> etc)
    protected List<string> _textData = new List<string>( );

    // All lines of this text
    protected List<Line> _lines = new List<Line>( );

    // Font ID
    protected int _fontID = 0;

    protected float _lineSpacing = 0;
    protected int _lineHeight = 0;
    protected float _spaceSize = 0;
    protected float _fontCustomSize = 0;
    protected float _fontBaseSize = 0;

    // Align
    protected TextAlign _textAlign = TextAlign.Left;

    // Maximum line width. If list of words will reach this point, current or next word will be in new line. If there is no width limit - set it to -1
    protected int _maximumWidth = -1;

    public Text(BasicContentController content, BasicConfigController config, BasicInputController input, string text) : this(content, config, input, ( ) => text, null, 0, -1) { }
    public Text(BasicContentController content, BasicConfigController config, BasicInputController input, Func<string> text) : this(content, config, input, text, null, 0, -1) { }
    public Text(BasicContentController content, BasicConfigController config, BasicInputController input, string text, Color? color) : this(content, config, input, ( ) => text, color, 0, -1) { }
    public Text(BasicContentController content, BasicConfigController config, BasicInputController input, Func<string> text, Color? color) : this(content, config, input, text, color, 0, -1) { }
    public Text(BasicContentController content, BasicConfigController config, BasicInputController input, string text, Color? color, int fontID) : this(content, config, input, ( ) => text, color, fontID, -1) { }
    public Text(BasicContentController content, BasicConfigController config, BasicInputController input, Func<string> text, Color? color, int fontID) : this(content, config, input, text, color, fontID, -1) { }
    public Text(BasicContentController content, BasicConfigController config, BasicInputController input, string text, Color? color, int fontID, int maxiumWidth) : this(content, config, input, ( ) => text, color, fontID, maxiumWidth) { }
    public Text(BasicContentController content, BasicConfigController config, BasicInputController input, Func<string> text, Color? color, int fontID, int maxiumWidth) : base(config, input) {
        _content = content;

        FontData font = _content.GetFontData(fontID);

        _sourceText = text;
        _fontID = fontID;
        _fontBaseSize = font.BaseSize;
        _fontCustomSize = _fontBaseSize;
        _spaceSize = font.GetStyle( ).MeasureString(" ").X;
        _lineHeight = (int)font.GetStyle( ).MeasureString("A").Y;
        _maximumWidth = maxiumWidth;

        SetColor(color ?? Color.Black);
    }

    public Text SetTextAlign(TextAlign align) {
        _textAlign = align;
        return this;
    }

    public Text SetLineSpacing(float lineSpacing) {
        _lineSpacing = lineSpacing;
        _isRebuildRequired = true;
        return this;
    }

    public Text SetFontSize(float fontSize) {
        if (fontSize <= 0) fontSize = _fontBaseSize;
        _fontCustomSize = fontSize;
        _isRebuildRequired = true;
        return this;
    }

    public Text SetMaximumWidth(int maxWidth) {
        if (maxWidth <= 0) maxWidth = -1;
        _maximumWidth = maxWidth;
        _isRebuildRequired = true;
        return this;
    }

    public override void Dispose( ) {
        base.Dispose( );

        _textData.Clear( );
        _lines.ForEach(line => line.Words.Clear( ));
        _lines.Clear( );
    }

    public void UpdateTextData( ) {
        // Get source text
        string source = _sourceText?.Invoke( );
        if (_renderText == source)
            return;

        // Set new render text
        _renderText = source;

        // Clear current data
        _textData.Clear( );

        // If current text is null - skip
        if (string.IsNullOrWhiteSpace(source))
            return;

        // Split by tags (like <b>, <i>, spaces, text, etc)
        string[] splitByTag = Regex.Matches(source, @"<[^>]+>|[^<>\s]+").Cast<Match>( ).Select(match => match.Value).ToArray( );

        // Add splitted data to list
        for (int j = 0; j < splitByTag.Length; j++)
            _textData.Add(splitByTag[j]);

        // Set flag for the rebuild
        _isRebuildRequired = true;
    }

    public void Rebuild( ) {
        List<Word> words = new List<Word>( );
        List<Line> lines = new List<Line>( );

        // Settings
        bool isBold = false;
        bool isItalic = false;
        Color color = GetColor( ) ?? Color.Black;
        float offset = 0;
        float scaleX = RenderScaleX * (_fontCustomSize / _fontBaseSize);
        float scaleY = RenderScaleY * (_fontCustomSize / _fontBaseSize);

        // Function for getting font based on current styles
        Func<SpriteFont> getFont = ( ) => {
            // Get font style
            FontStyle style = FontStyle.Regular;
            if (isBold && isItalic) { style = FontStyle.BoldItalic; } else
            if (isBold) { style = FontStyle.Bold; } else
            if (isItalic) { style = FontStyle.Italic; }

            // Get font
            return _content.GetFont(_fontID, style);
        };

        // Helper variables
        TextCommand command;
        string word;
        SpriteFont font = getFont( );

        // Check all text data
        for (int i = 0; i < _textData.Count; i++) {
            // Reset variables
            word = _textData[i];
            command = TextCommand.NONE;

            // Basic commands
            switch (word) {
                case "<br>": command = TextCommand.BREAK_LINE; break;
                case "<b>": command = TextCommand.BOLD; break;
                case "</b>": command = TextCommand.BOLD_END; break;
                case "<i>": command = TextCommand.ITALIC; break;
                case "</i>": command = TextCommand.ITALIC_END; break;
                case "</color>": command = TextCommand.COLOR_END; break;
            }

            // Color command
            if (command == TextCommand.NONE && word.StartsWith("<color=#"))
                command = TextCommand.COLOR;

            // Space (skip)
            if (command == TextCommand.NONE && string.IsNullOrWhiteSpace(word))
                continue;

            // If word is a normal text
            if (command == TextCommand.NONE) {
                float size = font.MeasureString(word).X;

                // Add text if possible
                if (words.Count == 0 || _maximumWidth <= 0 || offset + size <= _maximumWidth / scaleX) {
                    words.Add(new Word(word, color, font));
                    offset += font.MeasureString(word + ' ').X;
                    continue;
                }

                // If not possible - create new line and then add the text
                lines.Add(new Line(words, (int)(offset - _spaceSize)));
                offset = 0;
                words = new List<Word> { new Word(word, color, font) };
                offset += font.MeasureString(word + ' ').X;
            }

            // Word is a command and text is empty (ex. "<b><i>...")
            switch (command) {
                case TextCommand.BREAK_LINE:
                    lines.Add(new Line(words, (int)(offset - _spaceSize)));
                    offset = 0;
                    words = new List<Word>( );
                    break;
                case TextCommand.BOLD:
                    isBold = true;
                    font = getFont( );
                    break;
                case TextCommand.BOLD_END:
                    isBold = false;
                    font = getFont( );
                    break;
                case TextCommand.ITALIC:
                    isItalic = true;
                    font = getFont( );
                    break;
                case TextCommand.ITALIC_END:
                    isItalic = false;
                    font = getFont( );
                    break;
                case TextCommand.COLOR_END:
                    color = GetColor( ) ?? Color.Black;
                    break;
                case TextCommand.COLOR:
                    // <color=#012> (RGB)
                    if (word.Length == 12)
                        color = new Color(
                            Convert.ToByte("" + word[8] + word[8], 16),
                            Convert.ToByte("" + word[9] + word[9], 16),
                            Convert.ToByte("" + word[10] + word[10], 16)
                        );

                    // <color=#0123> (RGBA)
                    else if (word.Length == 13)
                        color = new Color(
                            Convert.ToByte("" + word[8] + word[8], 16),
                            Convert.ToByte("" + word[9] + word[9], 16),
                            Convert.ToByte("" + word[10] + word[10], 16),
                            Convert.ToByte("" + word[11] + word[11], 16)
                        );

                    // <color=#001122> (RGB)
                    else if (word.Length == 15)
                        color = new Color(
                            Convert.ToByte("" + word[8] + word[9], 16),
                            Convert.ToByte("" + word[10] + word[11], 16),
                            Convert.ToByte("" + word[12] + word[13], 16)
                        );

                    // <color=#00112233> (RGBA)
                    else if (word.Length == 17)
                        color = new Color(
                            Convert.ToByte("" + word[8] + word[9], 16),
                            Convert.ToByte("" + word[10] + word[11], 16),
                            Convert.ToByte("" + word[12] + word[13], 16),
                            Convert.ToByte("" + word[14] + word[15], 16)
                        );

                    break;
            }
        }

        // If there is some text left - add it to the poll as well
        if (words.Count > 0)
            lines.Add(new Line(words, (int)(offset - _spaceSize)));

        // Set current lines
        _lines = lines;

        // Update sizes
        SetWidth(_lines.Max(line => line.TotalWidth) * scaleX, UnitType.Pixel);
        SetHeight(_lines.Count * (_lineHeight + _lineSpacing) * scaleY);

        // Reset flag
        _isRebuildRequired = false;
    }

    public override void Update(GameTime time, bool updateOnlyThisObject = false) {
        // Updates
        if (IsTintUpdateRequired) UpdateColor( );
        if (IsOpacityUpdateRequired) UpdateOpacity( );
        if (IsSizeUpdateRequired) UpdateSize( );
        if (IsPositionUpdateRequired) UpdatePosition( );
        if (IsDepthUpdateRequired) UpdateDepth( );

        // Check if text changed
         UpdateTextData( );

        // Rebuild text if needed
        if (_isRebuildRequired)
            Rebuild( );

        // Update children
        if (!updateOnlyThisObject)
            Children.ForEach(child => child.Update(time));
    }

    public override void Render(GameTime time, bool renderOnlyThisObject = false) {
        if (RenderOpacity <= 0) return;

        if (_lines.Count > 0) {
            float scaleX = RenderScaleX * (_fontCustomSize / _fontBaseSize);
            float scaleY = RenderScaleY * (_fontCustomSize / _fontBaseSize);

            for (int i = 0; i < _lines.Count; i++) {
                Line line = _lines[i];
                Word word;
                float alignOffset = 0;
                float lineWidth = line.TotalWidth * scaleX;

                switch (_textAlign) {
                    case TextAlign.Center: alignOffset = RenderWidth / 2 - lineWidth / 2; break;
                    case TextAlign.Right: alignOffset = RenderWidth - lineWidth; break;
                }

                float wordsOffset = 0;
                for (int j = 0; j < line.Words.Count; j++) {
                    word = line.Words[j];
                    _content.Canvas.DrawString(
                        word.Font,
                        word.Text,
                        new Vector2(RenderX + alignOffset + wordsOffset * scaleX, RenderY + i * (_lineHeight + _lineSpacing) * scaleY),
                        word.Color,
                        0,
                        new Vector2(0, 0),
                        new Vector2(scaleX, scaleY),
                        SpriteEffects.None,
                        RenderDepth
                    );

                    wordsOffset += word.Width + _spaceSize;
                }
            }
        }

        base.Render(time, renderOnlyThisObject);
    }

}
