using RMEGo.Game.BMFSharp.Structures;
using SharpFont;

namespace RMEGo.Game.BMFSharp;

public static class GlyphMapGenerator
{

    public static GlyphCollection Make(FontSpec spec, bool outputDebugFiles = false)
    {
        var output = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");
        if (outputDebugFiles)
            Directory.CreateDirectory(output);
        var fontFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, spec.FontFamily);

        var fontService = new FontService(fontFilePath, spec.FontSize, index: spec.FontIndex);
        var list = new List<GlyphBitmap>();
        var rangeIndex = 0;
        foreach (var range in spec.GlyphRange)
        {
            Console.WriteLine($"Start range for {range.Start} ~ {range.End}.");
            for (var i = range.Start; i < range.End; i++)
            {
                var image = fontService.RenderChar((uint)i, spec.ForegroundColor, spec.BackgroundColor, RenderMode.Normal, spec.BitmapImageFormat);
                list.Add(image);
                if (image is not null)
                {
                    if (image.ImageDataPayload is not null)
                    {
                        if (outputDebugFiles)
                            File.WriteAllBytes(Path.Combine(output, i.ToString("X4") + ".png"), image.ImageDataPayload);
                    }
                    Console.WriteLine($"Generated ({i - range.Start}/{range.End - range.Start}): {i} | {image.BitmapWidth}({image.HorizontalAdvance}) x {image.BitmapHeight} | x: {image.HorizontalBearingX}, y:{image.HorizontalBearingY}");
                }
                else
                {
                    Console.WriteLine($"Skipped ({i - range.Start}/{range.End - range.Start}): {i}");
                }
            }
            rangeIndex++;
        }
        return new GlyphCollection(spec.BitmapImageFormat) { Items = list };
    }
}