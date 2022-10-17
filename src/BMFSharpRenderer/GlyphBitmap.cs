using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace RMEGo.Game.BMFSharp;

public static class GlyphBitmapExtensions
{
    public static SKBitmap? LoadFromGlyph(this GlyphBitmap glyph, BitmapFormat format)
    {
        if (glyph.ImageDataPayload is null || glyph.BitmapWidth == 0 || glyph.BitmapHeight == 0) {
            return null;
        }

        using var surface = SKSurface.Create(new SKImageInfo(glyph.BitmapWidth, glyph.BitmapHeight));
        var canvas = surface.Canvas;
        SKBitmap bitmap;
        switch (format)
        {
            case BitmapFormat.Raw:
                var ms = new MemoryStream(glyph.ImageDataPayload);
                var bw = new BinaryReader(ms);
                bitmap = new SKBitmap(glyph.BitmapWidth, glyph.BitmapHeight);
                for (var x = 0; x < glyph.BitmapWidth; x++)
                {
                    for (var y = 0; y < glyph.BitmapHeight; y++)
                    {
                        var alpha = bw.ReadByte();
                        var red = bw.ReadByte();
                        var green = bw.ReadByte();
                        var blue = bw.ReadByte();
                        bitmap.SetPixel(x, y, new SKColor(red, green, blue, alpha));
                    }
                }
                return bitmap;

            case BitmapFormat.Bmp:
            case BitmapFormat.Png:
                bitmap = SKBitmap.Decode(glyph.ImageDataPayload);
                return bitmap;

            default:
                throw new NotImplementedException("This format have not been supported.");
        }
    }
}
