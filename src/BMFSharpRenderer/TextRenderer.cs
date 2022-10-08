using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace RMEGo.Game.BMFSharp;

public enum TextAlign
{
    Left,
    Middle,
    Right
}

public record DrawingTextLine(string Text);

public record LineBreak() : DrawingTextLine("\n");

public record TextLine(string Text, int MaxTextHeight, int Width, int BaseLineTopOffset, TextAlign Align) : DrawingTextLine(Text);

public class TextRenderer
{
    private List<DrawingTextLine> text = new();
    private Dictionary<char, GlyphBitmap> Glyphs { get; }
    public int MaxWidth { get; private set; } = -1;

    public TextRenderer(ICollection<GlyphBitmap> glyphs)
    {
        this.Glyphs = new Dictionary<char, GlyphBitmap>();
        foreach (var glyph in glyphs)
        {
            this.Glyphs[(char)glyph.CharCode] = glyph;
        }
    }

    public void PushLine(string text, TextAlign align)
    {
        this.text.AddRange(this.SplitTextIntoLines(text, align));
        this.text.Add(new LineBreak());
    }

    public void PushLine()
    {
        this.text.Add(new LineBreak());
    }

    private List<SKImage?> DrawingEachLines()
    {
        var list = new List<SKImage?>();
        foreach (var line in text)
        {
            if (line is TextLine textline)
            {
                list.Add(this.DrawingSingleLine(textline));
            }
            else
            {
                list.Add(null);
            }
        }
    }

    private SKImage DrawingSingleLine(TextLine line)
    {
        using (var surface = SKSurface.CreateNull(line.Width, line.MaxTextHeight))
        {

        }
    }

    private List<TextLine> SplitTextIntoLines(string text, TextAlign align)
    {
        var currentText = new TextLine("", 0, 0, 0, align);
        var result = new List<TextLine>
        {
            currentText
        };
        foreach (var c in text)
        {
            var widthThisLine = 0;
            var glyph = this.Glyphs[c];
            widthThisLine += glyph.HorizontalAdvance;
            var top = glyph.HorizontalBearingY;
            var bottom = glyph.HorizontalBearingY - glyph.BitmapHeight;
            if (this.MaxWidth >= 0 && widthThisLine > this.MaxWidth)
            {
                currentText = new TextLine("", 0, 0, 0, align);
                result.Add(currentText);
                widthThisLine = glyph.HorizontalAdvance;
            }
            if (currentText.MaxTextHeight < top + bottom)
            {
                currentText = currentText with {
                    MaxTextHeight = top + bottom,
                    BaseLineTopOffset = top
                };
            }
            currentText = currentText with
            {
                Text = currentText.Text + c,
                Width = widthThisLine
            };
        }
        return result;
    }


}
