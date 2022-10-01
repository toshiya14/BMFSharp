using System.Drawing;

namespace RMEGo.Game.BMFSharp.Structures;

public record FontSpec
{
    public string FontFamily { get; set; } = string.Empty;
    public int FontIndex { get; set; } = 0;
    public float FontSize { get; set; } = 12;
    public GlyphRange[] GlyphRange { get; set; } = Array.Empty<GlyphRange>();
    public string BitmapFormat { get; set; } = "raw";
    public string ForeColor { get; set; } = "FF000000";
    public string BackColor { get; set; } = "00FFFFFF";

    private static Color String2Color(string color)
    {
        if (color.Length != 8)
        {
            throw new ArgumentException("color string is not with length of 8.");
        }
        var a = Convert.ToInt16(color[0..2], 16);
        var r = Convert.ToInt16(color[2..4], 16);
        var g = Convert.ToInt16(color[4..6], 16);
        var b = Convert.ToInt16(color[6..8], 16);
        return Color.FromArgb(a, r, g, b);
    }

    private static BitmapFormat GetFormat(string format)
    {
        var bitmapFormat = Enum.Parse<BitmapFormat>(format, true);
        if (!Enum.IsDefined(bitmapFormat))
        {
            throw new ArgumentException($"Unsupported format: {format}.");
        }
        return bitmapFormat;
    }

    public Color ForegroundColor => String2Color(this.ForeColor);
    public Color BackgroundColor => String2Color(this.BackColor);
    public BitmapFormat BitmapImageFormat => GetFormat(this.BitmapFormat);
}