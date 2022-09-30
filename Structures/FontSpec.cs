namespace RMEGo.Game.BMFSharp.Structures;

public record FontSpec
{
    public string FontFamily{ get; set; }
    public float FontSize{ get; set; }
    public GlyphRange[] GlyphRange{ get; set; }
}