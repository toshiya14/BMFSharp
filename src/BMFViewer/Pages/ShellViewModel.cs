using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using BMFSharp.Structures;
using BMFViewer.Events;
using BMFViewer.Models;
using RMEGo.Game.BMFSharp;
using SharpFont;
using Stylet;
using Glyph = BMFViewer.Models.Glyph;

namespace BMFViewer.Pages;

public class ShellViewModel : Screen, IHandle<DropFileEvent>
{
    public struct ZoomRateOption
    {
        public string Display { get; set; }
        public float Value { get; set; }
    }

    public ShellViewModel(IEventAggregator events)
    {
        events.Subscribe(this);
        this.ZoomRateOptions = new[] {
            new ZoomRateOption { Value = 0.125f, Display = "1 / 8" },
            new ZoomRateOption { Value = 0.25f, Display = "1 / 4" },
            new ZoomRateOption { Value = 0.5f, Display = "1 / 2" },
            new ZoomRateOption { Value = 1f, Display = "原始" },
            new ZoomRateOption { Value = 1.5f, Display = "1.5" },
            new ZoomRateOption { Value = 2f, Display = "2.0" },
        };
    }

    public IEnumerable<Glyph> Items
    {
        get
        {
            if (this.FilteredItems is null || !this.FilteredItems.Any()) return Array.Empty<Glyph>();
            var skips = (this.Page - 1) * pageSize;
            var items = this.FilteredItems.Skip(skips).Take(pageSize);
            return items.Select(x => new Glyph { BitmapSource = ConvertBytesToSource(x.ImageDataPayload, this.Collection.Format), Character = ((char)x.CharCode).ToString(), CharCode = x.CharCode.ToString("X4") });
        }
    }
    public GlyphCollection Collection { get; set; }
    public IEnumerable<GlyphBitmap> FilteredItems { get; set; }
    private const int pageSize = 102;
    private string keyword;
    public int Page { get; set; } = 1;
    public int TotalPage => (int)Math.Ceiling(this.FilteredItems.Count() / (float)pageSize);
    public float Zoom { get; set; } = 1f;
    public string Keyword
    {
        get => keyword;
        set
        {
            this.keyword = value;
            this.Page = 1;
            if (!string.IsNullOrWhiteSpace(this.Keyword))
            {
                this.FilteredItems = this.Collection.Items.Where(x =>
                {
                    var code = x.CharCode.ToString("X4");
                    var ch = (char)x.CharCode;
                    return code.Contains(this.Keyword) || this.Keyword.Contains(ch);
                });
            }
            else
            {
                this.FilteredItems = this.Collection.Items;
            }
        }
    }
    public ZoomRateOption[] ZoomRateOptions { get; set; }
    public string PageInfo => $"{this.Page} / {this.TotalPage}";
    public void NextPage() => this.Page = Math.Min(this.Page + 1, this.TotalPage);
    public void PrevPage() => this.Page = Math.Max(this.Page - 1, 1);
    public void DragFileIn(string filename)
    {
        if (File.Exists(filename))
        {
            var loader = new BMFLoader();
            loader.LoadFromFile(filename);
            this.Collection = loader.GetGlyphes();
            this.Page = 1;
            this.Keyword = string.Empty;
        }
    }

    public void Handle(DropFileEvent message)
    {
        if (message.Handler.Equals(this.GetType()))
        {
            this.DragFileIn(message.FileName);
        }
    }

    private BitmapSource ConvertBytesToSource(byte[] payload, BitmapFormat format)
    {
        if (payload is null || payload.Length == 0)
        {
            return null;
        }

        using var ms = new MemoryStream();
        if (format is BitmapFormat.Raw)
        {
            using var fromms = new MemoryStream(payload);
            using var reader = new BinaryReader(fromms);
            var width = reader.ReadInt32();
            var height = reader.ReadInt32();
            using var bitmap = new Bitmap(width, height);
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var a = reader.ReadByte();
                    var r = reader.ReadByte();
                    var g = reader.ReadByte();
                    var b = reader.ReadByte();
                    bitmap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }
            }

            bitmap.Save(ms, ImageFormat.Png);
        }
        else
        {
            ms.Write(payload);
        }
        var image = new BitmapImage();
        image.BeginInit();
        image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = ms;
        image.EndInit();
        image.Freeze();
        return image;
    }
}
