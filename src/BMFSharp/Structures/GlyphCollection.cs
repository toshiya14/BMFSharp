using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpFont;

namespace BMFSharp.Structures;

public record GlyphCollection
{
    public List<GlyphBitmap> Items { get; set; } = new();
    public BitmapFormat Format { get; set; }

    public GlyphCollection(BitmapFormat format) {
        this.Format = format;
    }
}
