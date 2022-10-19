#region MIT License
/*Copyright (c) 2016 Robert Rouhani <robert.rouhani@gmail.com>
SharpFont based on Tao.FreeType, Copyright (c) 2003-2007 Tao Framework Team
Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/
#endregion

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using RMEGo.Game.BMFSharp;

namespace SharpFont
{
    internal class FontService : IDisposable
    {
        private Library lib;
        private Face _fontFace;

        internal FontFormatCollection SupportedFormats { get; private set; }

        #region Constructor

        /// <summary>
        /// If multithreading, each thread should have its own FontService.
        /// </summary>
        internal FontService(string face, float size = 9, int index = 0)
        {
            lib = new Library();
            SupportedFormats = new FontFormatCollection();
            AddFormat("TrueType", "ttf");
            AddFormat("OpenType", "otf");
            _fontFace = new Face(lib, face, index);
            _fontFace.SetCharSize(0, size, 0, 96);
        }

        private void AddFormat(string name, string ext)
        {
            SupportedFormats.Add(name, ext);
        }

        #endregion


        internal GlyphBitmap RenderChar(uint charCode, Color foreColor, Color backColor, RenderMode mode, BitmapFormat format)
        {
            var c = (char)charCode;
            var glyphIndex = _fontFace.GetCharIndex(c);
            var bitmap = new GlyphBitmap();
            _fontFace.LoadGlyph(glyphIndex, LoadFlags.NoHinting | LoadFlags.Render, LoadTarget.Normal);
            bitmap.CharCode = charCode;
            bitmap.HorizontalAdvance = (short)_fontFace.Glyph.Metrics.HorizontalAdvance.ToInt32();
            bitmap.HorizontalBearingX = (short)_fontFace.Glyph.Metrics.HorizontalBearingX.ToInt32();
            bitmap.HorizontalBearingY = (short)_fontFace.Glyph.Metrics.HorizontalBearingY.ToInt32();
            // var bmp = new Bitmap(bitmap.BitmapWidth, bitmap.BitmapHeight);
            // using var g = Graphics.FromImage(bmp);
            // g.CompositingQuality = CompositingQuality.HighQuality;
            // g.SmoothingMode = SmoothingMode.HighQuality;
            // g.CompositingMode = CompositingMode.SourceOver;
            // g.Clear(backColor);
            _fontFace.Glyph.RenderGlyph(mode);
            if (_fontFace.Glyph.Bitmap.Width > 0 && _fontFace.Glyph.Bitmap.Rows > 0)
            {
                Bitmap bmp;
                if (backColor.A == 0)
                {
                    bmp = _fontFace.Glyph.Bitmap.ToGdipBitmap(foreColor);
                }
                else
                {
                    var ft = _fontFace.Glyph.Bitmap.ToGdipBitmap(foreColor);
                    bmp = new Bitmap((int)ft.Width, (int)ft.Height);
                    using var g = Graphics.FromImage(bmp);
                    g.CompositingMode = CompositingMode.SourceOver;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.Clear(backColor);
                    g.DrawImageUnscaled(ft, 0, 0);
                    g.Flush();
                }

                bitmap.BitmapHeight = (short)bmp.Height;
                bitmap.BitmapWidth = (short)bmp.Width;

                if (format is not BitmapFormat.Raw)
                {
                    using var ms = new MemoryStream();
                    var saveFormat = format switch
                    {
                        BitmapFormat.Bmp => ImageFormat.Bmp,
                        BitmapFormat.Png => ImageFormat.Png,
                        _ => throw new NotImplementedException()
                    };
                    bmp.Save(ms, saveFormat);
                    bitmap.ImageDataPayload = ms.ToArray();
                }
                else
                {
                    var ms = new MemoryStream();
                    var binaryWriter = new BinaryWriter(ms);
                    binaryWriter.Write(bmp.Width);
                    binaryWriter.Write(bmp.Height);
                    for (var x = 0; x < bmp.Width; x++)
                    {
                        for (var y = 0; y < bmp.Height; y++)
                        {
                            var pixel = bmp.GetPixel(x, y);
                            ms.WriteByte(pixel.A);
                            ms.WriteByte(pixel.R);
                            ms.WriteByte(pixel.G);
                            ms.WriteByte(pixel.B);
                        }
                    }
                    bitmap.ImageDataPayload = ms.ToArray();
                }
            }
            else
            {
                bitmap.ImageDataPayload = null;
            }
            return bitmap;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this._fontFace != null && !_fontFace.IsDisposed)
                        try
                        {
                            _fontFace.Dispose();
                        }
                        catch { }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FontService() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

}