namespace RMEGo.Game.BMFSharp.Structures;

public record GlyphCollection
{
    public List<GlyphBitmap> Items { get; set; } = new();
    public BitmapFormat Format { get; set; }

    public GlyphCollection(BitmapFormat format) {
        this.Format = format;
    }
}
