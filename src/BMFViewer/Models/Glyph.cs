using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BMFViewer.Models;

public record Glyph
{
    public BitmapSource BitmapSource { get; set; }
    public string Character { get; set; }
    public string CharCode { get; set; }
}
