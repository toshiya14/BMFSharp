using System.Diagnostics;
using System.Drawing;
using RMEGo.Game.BMFSharp.Structures;
using SharpFont;

namespace RMEGo.Game.BMFSharp;

public static class GlyphMapGenerator
{
    public static ICollection<GlyphBitmap> Make(FontSpec spec, bool outputDebugFiles = false)
    {
        var output = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");
        if(outputDebugFiles)
            Directory.CreateDirectory(output);
        var fontService = new FontService();
        var fontFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, spec.FontFamily);
        fontService.SetFont(fontFilePath);
        fontService.SetSize(spec.FontSize);
        var list = new List<GlyphBitmap>();
        var rangeIndex = 0;
        var watch = Stopwatch.StartNew();
        foreach (var range in spec.GlyphRange)
        {
            Console.WriteLine($"Start range for {range.Start} ~ {range.End}.");
            for (var i = range.Start; i < range.End; i++)
            {
                var image = fontService.RenderChar((uint)i, Color.FromArgb(255, 255, 255, 255), RenderMode.Normal);
                list.Add(image);
                if (image is not null)
                {
                    if (image.PngDataPayload is not null)
                    {
                        if (outputDebugFiles)
                            File.WriteAllBytes(Path.Combine(output, i.ToString("X4") + ".png"), image.PngDataPayload);
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
        watch.Stop();
        Console.WriteLine($"Total ellapsed: {watch.ElapsedMilliseconds} ms");
        return list;
    }
}