using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;
using VXEngine.Models;
using VXEngine.Types;

namespace VXEngine.Objects.Primitives;

/// <summary>
/// Text object
/// </summary>
public class Text : GameObject {

    /// <summary>
    /// Content reference
    /// </summary>
    protected BasicContentController _content;

    /// <summary>
    /// Holder for <see cref="TextDynamic"/> output to compare changes
    /// </summary>
    protected string _textOutput = null;

    /// <summary>
    /// Lines of text
    /// </summary>
    protected List<TextLineModel> _lines;

    /// <summary>
    /// Line height
    /// </summary>
    protected int _lineHeight;

    /// <summary>
    /// Font's data
    /// </summary>
    public FontData Data { get; protected set; }

    /// <summary>
    /// Text's font
    /// </summary>
    public SpriteFont Font => Data != null ? Data.Font : null;

    /// <summary>
    /// Font's size
    /// </summary>
    public int Size { get; protected set; }

    /// <summary>
    /// Dynamic text
    /// </summary>
    public Func<string> TextDynamic { get; protected set; }

    /// <summary>
    /// Maximum text's line width
    /// </summary>
    public float MaxWidth { get; protected set; }

    /// <summary>
    /// Distance between lines
    /// </summary>
    public float LineSpacing { get; protected set; }

    /// <summary>
    /// Text alignment
    /// </summary>
    public TextAlign Align { get; set; } = TextAlign.Left;

    /// <summary>
    /// Set text justify
    /// </summary>
    public bool Justify { get; protected set; } = false;

    /// <summary>
    /// How distant can words be between each other in justify mode
    /// </summary>
    protected int _justifyMargin = 30;

    /// <summary>
    /// Constructor
    /// </summary>
    public Text(FontData fontData, BasicContentController content, BasicInputController input, string text, float maxWidth = 0, int size = -1, bool justify = false) : base(input) {
        _content = content;
        _lines = new List<TextLineModel>( );

        Data = fontData;
        Size = size <= 0 ? Data.FontSize : size;
        TextDynamic = ( ) => text;
        MaxWidth = maxWidth;
        Justify = justify;
        _scaleX = (float)Size / Data.FontSize;
        _scaleY = (float)Size / Data.FontSize;

        UpdateText(true);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public Text(FontData fontData, BasicContentController content, BasicInputController input, Func<string> text, float maxWidth = 0, int size = -1, bool justify = false) : base(input) {
        _content = content;
        _lines = new List<TextLineModel>( );

        Data = fontData;
        Size = size <= 0 ? Data.FontSize : size;
        TextDynamic = text;
        MaxWidth = maxWidth;
        Justify = justify;
        _scaleX = (float)Size / Data.FontSize;
        _scaleY = (float)Size / Data.FontSize;

        UpdateText(true);
    }

    // Overwrites
    public override void SetWidth(float width) => throw new NotSupportedException( );
    public override void SetHeight(float height) => throw new NotSupportedException( );
    public override void SetSize(float width, float height) => throw new NotSupportedException( );
    public override void SetDimensions(float x, float y, float width, float height) => SetPosition(x, y);
    public override void SetScaleX(float scaleX) => throw new NotSupportedException( );
    public override void SetScaleY(float scaleY) => throw new NotSupportedException( );
    public override void SetScale(float scale) => throw new NotSupportedException( );

    /// <summary>
    /// Change text's justification
    /// </summary>
    public virtual void SetJustification(bool justify) {
        if (Justify == justify) return;
        Justify = justify;
        UpdateText(true);
    }

    /// <summary>
    /// Sets font size
    /// </summary>
    public virtual void SetFontSize(int size) {
        Size = size <= 0 ? Data.FontSize : size;
        _scaleX = (float)Size / Data.FontSize;
        _scaleY = (float)Size / Data.FontSize;
    }

    /// <summary>
    /// Sets max width for the text
    /// </summary>
    public virtual void SetMaxWidth(float maxWidth) {
        if (maxWidth <= 0)
            MaxWidth = 0;
        else
            MaxWidth = maxWidth;

        UpdateText(true);
    }

    /// <summary>
    /// Sets distance between lines
    /// </summary>
    public virtual void SetLineSpacing(float lineSpacing) {
        LineSpacing = lineSpacing;
        UpdateText(true);
    }

    /// <summary>
    /// Sets dynamic text
    /// </summary>
    public virtual void SetText(Func<string> dynamic) {
        TextDynamic = dynamic;
        UpdateText(true);
    }

    /// <summary>
    /// Sets dynamic text
    /// </summary>
    public virtual void SetText(string text) {
        TextDynamic = ( ) => text;
        UpdateText(true);
    }

    /// <summary>
    /// Updates text
    /// </summary>
    public void UpdateText(bool force = false) {
        string output = TextDynamic?.Invoke( );

        // If text didn't changed
        if (_textOutput == output && !force)
            return;

        // Clear lines
        _lines.Clear( );

        // Change text
        _textOutput = output;

        // Check if there is max width
        if (MaxWidth <= 0) {
            _lines = new List<TextLineModel>( ) { new TextLineModel(string.IsNullOrWhiteSpace(_textOutput) ? new List<string>( ) { } : _textOutput.Split(' ').ToList( )) };
            Vector2 size = Font.MeasureString(_textOutput.TrimEnd( ));
            _width = size.X;
            _height = size.Y;
            return;
        }

        // Split text
        string[] splitted = _textOutput.Split(' ');

        // Split into lines
        List<List<string>> lines = new List<List<string>>( );
        List<string> line = new List<string>( );
        float lineWidth = 0;
        for (int i = 0; i < splitted.Length; i++) {
            float wordWidth = Font.MeasureString(splitted[i]).X;

            // If line is too wide
            if (lineWidth + wordWidth >= MaxWidth / GetScaleX( )) {
                // If line is empty - simple add word
                if (line.Count == 0) {
                    line.Add(splitted[i].TrimEnd( ));
                    lines.Add(line);
                    line.Clear( );
                    lineWidth = 0;

                 // There is text
                } else {
                    lines.Add(line);
                    line = new List<string>( );
                    line.Add(splitted[i].TrimEnd( ));
                    lineWidth = Font.MeasureString(splitted[i] + " ").X;
                }

            // Line is fine, continue
            } else {
                lineWidth += Font.MeasureString(splitted[i] + " ").X;
                line.Add(splitted[i]);
            }
        }

        // If there is some text left
        if (line.Count != 0)
            lines.Add(line);

        // Add lines and update size
        lines.ForEach(line => _lines.Add(new TextLineModel(line)));
        _width = _lines.Max(line => Font.MeasureString(line.Output).X);
        _height = _lines.Count * Font.MeasureString("A").Y + LineSpacing * (_lines.Count - 1);

        // Justify if needed (except last line)
        if (Justify)
            for (int i = 0; i < _lines.Count - 1; i++) {
                TextLineModel model = _lines[i];
                if (model.Words.Count <= 1) continue;
                model.JustifyLine = true;
                model.JustifyOffset = (_width - Font.MeasureString(model.Output).X) / (model.Words.Count - 1);
            }
    }

    /// <inheritdoc/>
    public override void Update(GameTime time) {
        base.Update(time);
        UpdateText( );
    }

    /// <inheritdoc/>
    public override void Render(GameTime time) {
        if (_lines == null || _lines.Count == 0 || GetAlpha( ) <= 0 || !GetVisible( ))
            return;

        // Standard display
        for (int i = 0; i < _lines.Count; i++) {
            TextLineModel line = _lines[i];
            float offsetY = _height / _lines.Count * i;

            // Justify
            if (line.JustifyLine) {
                float offsetX = 0;

                for (int j = 0; j < line.Words.Count; j++) {
                    string word = line.Words[j];

                    _content.Canvas.DrawString(
                        Font,
                        word,
                        new Vector2(
                            DisplayX + GetRotationOriginX( ) * DisplayWidth + offsetX * GetScaleX( ),
                            (int)(DisplayY + GetRotationOriginY( ) * DisplayHeight + offsetY * GetScaleY( ))
                        ),
                        OutputColor,
                        GetAngle( ),
                        new Vector2(RotationOriginX, RotationOriginY),
                        new Vector2(GetScaleX( ), GetScaleY( )),
                        SpriteEffects.None,
                        0
                    );

                    offsetX += Font.MeasureString(word + (j == line.Words.Count - 1 ? "" : " ")).X + line.JustifyOffset;
                }

            // Standard
            } else {
                float offsetX = 0;
                Vector2 size = Font.MeasureString(line.Output);

                switch (Align) {
                    case TextAlign.Left: offsetX = 0; break;
                    case TextAlign.Center: offsetX = GetWidth( ) / 2 - size.X / 2; break;
                    case TextAlign.Right: offsetX = GetWidth( ) - size.X; break;
                }

                // TODO
                // Each line rotates separatly, and should rotate as whole container

                _content.Canvas.DrawString(
                    Font,
                    line.Output,
                    new Vector2(
                        DisplayX + GetRotationOriginX( ) * DisplayWidth + offsetX * GetScaleX( ),
                        (int)(DisplayY + GetRotationOriginY( ) * DisplayHeight + offsetY * GetScaleY( ))
                    ),
                    OutputColor,
                    GetAngle( ),
                    new Vector2(RotationOriginX, RotationOriginY),
                    new Vector2(GetScaleX( ), GetScaleY( )),
                    SpriteEffects.None,
                    0
                );
            }
        }
    }

}
