using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMEGo.Game.BMFSharp.Structures;
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

public record TextLine(
    string Text,
    int MaxTextHeight,
    int Width,
    int BaseLinePositionFromTop,
    TextAlign Align
) : DrawingTextLine(Text);

public class TextRenderer
{
    private List<DrawingTextLine> text = new();
    private GlyphCollection Glyphs { get; }
    private Dictionary<char, GlyphBitmap> IndexedGlyphs { get; }
    public int MaxWidth { get; private set; } = -1;

    public TextRenderer(GlyphCollection glyphs)
    {
        this.Glyphs = glyphs;
        this.IndexedGlyphs = this.Glyphs.GetIndexedCollection(x => (char)x.CharCode);
    }

    public void PushLine(string text, TextAlign align = TextAlign.Left)
    {
        this.text.AddRange(this.SplitTextIntoLines(text, align));
        this.text.Add(new LineBreak());
    }

    public void PushLine()
    {
        this.text.Add(new LineBreak());
    }

    public SKImage? Draw(double lineHeightRate)
    {
        var eachLine = this.DrawingEachLines();
        //var lineHeight = eachLine.Max(x => x?.Height ?? 0) * lineHeightRate;
        var totalWidth = eachLine.Max(x => x?.Width ?? 0);
        var totalHeight = eachLine.Sum(x=> x?.Height ?? 0);

        if (totalWidth == 0 || totalHeight == 0)
        {
            return null;
        }

        using var surface = SKSurface.Create(new SKImageInfo(totalWidth, (int)totalHeight));
        var canvas = surface.Canvas;
        var yOffset = 0f;
        for (var lineIndex = 0; lineIndex < eachLine.Count; lineIndex += 1)
        {
            var lineImage = eachLine[lineIndex];
            if (lineImage is not null)
            {
                var lineHeight = lineImage.Height * lineHeightRate;
                canvas.DrawImage(eachLine[lineIndex], new SKPoint(0, yOffset));
                yOffset += (float)lineHeight;
            }
        }
        var freezed = surface.Snapshot();
        return freezed;
    }

    public byte[]? DrawAsPng(double lineHeightRate)
    {
        using var image = this.Draw(lineHeightRate);
        if (image is null)
        {
            return null;
        }
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var ms = new MemoryStream();
        data.SaveTo(ms);
        return ms.ToArray();
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

#warning Delete these debug outputs.

        Directory.CreateDirectory("debug_output");
        var i = 0;
        foreach (var line in list)
        {

            if (line is not null)
            {
                using var fs = File.OpenWrite($"debug_output/{i}.png");
                line.Encode(SKEncodedImageFormat.Png, 100).SaveTo(fs);
            }

            i++;
        }

        return list;
    }

    private SKImage? DrawingSingleLine(TextLine line)
    {
        if (line.Width == 0 || line.MaxTextHeight == 0)
        {
            return null;
        }

        using var surface = SKSurface.Create(new SKImageInfo(line.Width, line.MaxTextHeight));
        var pen = new Position(0, line.BaseLinePositionFromTop);
        var canvas = surface.Canvas;

        foreach (var ch in line.Text)
        {
            if (this.IndexedGlyphs.ContainsKey(ch))
            {
                var glyph = this.IndexedGlyphs[ch];
                var drawingPosition = new Position(pen.X + glyph.HorizontalBearingX, pen.Y - glyph.HorizontalBearingY);
                var bitmap = glyph.LoadFromGlyph(this.Glyphs.Format);
                if (bitmap is not null) canvas.DrawBitmap(bitmap, new SKPoint(drawingPosition.X, drawingPosition.Y));
                pen = pen with { X = pen.X + glyph.HorizontalAdvance };
            }
            else
            {
                throw new NotImplementedException("Glyph not be presented, and no drawing method for this situation is specified.");
            }
        }

        var freezed = surface.Snapshot();
        return freezed;
    }

    private List<TextLine> SplitTextIntoLines(string text, TextAlign align)
    {
        var currentText = new TextLine("", 0, 0, 0, align);
        var result = new List<TextLine>
        {
            currentText
        };
        var widthThisLine = 0;
        foreach (var c in text)
        {
            var glyph = this.IndexedGlyphs[c];
            widthThisLine += glyph.HorizontalAdvance;
            var top = glyph.HorizontalBearingY;
            var bottom = glyph.HorizontalBearingY - glyph.BitmapHeight;
            widthThisLine += glyph.HorizontalAdvance;
            if (this.MaxWidth >= 0 && widthThisLine > this.MaxWidth)
            {
                result.Add(currentText);
                currentText = new TextLine("", 0, 0, 0, align);
                widthThisLine = 0;
            }
            if (currentText.MaxTextHeight < top + bottom)
            {
                currentText = currentText with
                {
                    MaxTextHeight = top + bottom,
                    BaseLinePositionFromTop = top
                };
            }
            currentText = currentText with
            {
                Text = currentText.Text + c,
                Width = widthThisLine
            };
        }
        result.Add(currentText);
        return result;
    }


}
