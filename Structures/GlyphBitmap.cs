namespace RMEGo.Game.BMFSharp.Structures;

public record GlyphBitmap { 
    public uint CharCode{ get; set; }
    public short HorizontalBearingX{ get; set; }
    public short HorizontalBearingY{ get; set; }
    public short BitmapWidth{ get; set; }
    public short BitmapHeight{ get; set; }
    public short HorizontalAdvance{ get; set; }
    public byte[]? PngDataPayload{ get; set; }
}