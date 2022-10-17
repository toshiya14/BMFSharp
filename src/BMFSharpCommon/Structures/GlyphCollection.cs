namespace RMEGo.Game.BMFSharp.Structures;

public record GlyphCollection
{
    public List<GlyphBitmap> Items { get; set; } = new();
    public BitmapFormat Format { get; set; }

    public GlyphCollection(BitmapFormat format)
    {
        this.Format = format;
    }

    public Dictionary<T, GlyphBitmap> GetIndexedCollection<T>(Func<GlyphBitmap, T> keySelector) where T : notnull => this.Items.ToDictionary(x => keySelector(x), x => x);
}
