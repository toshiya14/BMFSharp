using RMEGo.Game.BMFSharp.Structures;

namespace RMEGo.Game.BMFSharp;

public class BMFLoader
{
    private static readonly byte[] MAGIC = new byte[] { (byte)'R', (byte)'B', (byte)'M', (byte)'F' };
    private static readonly byte MAJOR_VERSION = 1;
    private static readonly byte MINOR_VERSION = 1;
    //private static readonly byte TYPE = 1; // 1-Single File Package 2-Paged Bitmap Atlas
    private byte type = 1;
    private byte? glyphFormat;

    private List<GlyphBitmap> table = new();

    public GlyphCollection GetGlyphes()
    {
        if (glyphFormat is null)
        {
            throw new ArgumentException("Glyph format are not set yet.");
        }
        return new GlyphCollection(GetGlyphFormat(glyphFormat.Value))
        {
            Items = table
        };
    }

    public void PutGlyphes(GlyphCollection glyphes)
    {
        this.glyphFormat = GetGlyphFormatByte(glyphes.Format);
        foreach (var glyph in glyphes.Items) this.PushGlyph(glyph);
    }

    private void PushGlyph(GlyphBitmap glyph)
    {
        if (glyphFormat is null)
        {
            throw new ArgumentException("Glyph format are not set yet.");
        }
        table.Add(glyph);
    }

    public static byte GetGlyphFormatByte(BitmapFormat format)
    {
        return format switch
        {
            BitmapFormat.Raw => 0x0,
            BitmapFormat.Bmp => 0x1,
            BitmapFormat.Png => 0x2,
            _ => throw new ArgumentException($"Unsupported bitmap format: {format}")
        };
    }

    public static BitmapFormat GetGlyphFormat(byte b)
    {
        if (b == 0x0) return BitmapFormat.Raw;
        else if (b == 0x1) return BitmapFormat.Bmp;
        else if (b == 0x2) return BitmapFormat.Png;
        else throw new ArgumentException($"Unsupported bitmap format byte: {b:X2}");
    }

    public void LoadFromFile(string path)
    {
        using var fs = new FileStream(path, FileMode.Open);
        using var reader = new BinaryReader(fs);

        // Read Magic
        var magic = reader.ReadBytes(4);
        if (!magic.SequenceEqual(MAGIC))
            throw new FormatException("This is not a bmf file.");

        // Read Version
        var major = reader.ReadByte();
        var minor = reader.ReadByte();
        if (major != MAJOR_VERSION)
            throw new FormatException($"This loader only supported v{MAJOR_VERSION} format, but this file is v{major} format.");
        if (minor > MINOR_VERSION)
            throw new FormatException($"This loader is out-to-date and not supported this format.");

        // File Type
        var type = reader.ReadByte();
        if (type != 1)
        {
            throw new NotImplementedException();
        }

        // Bitmap Format
        var formatByte = reader.ReadByte();
        var format = GetGlyphFormat(formatByte);
        this.glyphFormat = formatByte;

        // header
        var headerOffset = fs.Position;
        var headerLength = reader.ReadInt64();
        var table = new List<GlyphBitmap>();
        var offsetList = new List<Tuple<int, int>>();

        using var header = new MemoryStream();
        using var headerReader = new BinaryReader(header);
        header.Write(reader.ReadBytes((int)headerLength));
        header.Seek(0, SeekOrigin.Begin);

        while (header.Position < header.Length)
        {
            var charCode = headerReader.ReadUInt32();
            var bearingX = headerReader.ReadInt16();
            var bearingY = headerReader.ReadInt16();
            var advance = headerReader.ReadInt16();
            var offset = headerReader.ReadInt32();
            var length = headerReader.ReadInt32();

            var payload = reader.ReadBytes(length);

            table.Add(new GlyphBitmap
            {
                CharCode = charCode,
                HorizontalBearingX = bearingX,
                HorizontalBearingY = bearingY,
                HorizontalAdvance = advance,
                ImageDataPayload = payload
            });
        }

        this.table = table;
    }

    public void SaveAsFile(string path)
    {
        if (glyphFormat is null)
        {
            throw new ArgumentException("Glyph format are not set yet.");
        }
        // Calculate the size of header
        var singleTableItemLength = 4 + 2 + 2 + 2 + 4 + 4;
        var tableSize = singleTableItemLength * table.Count;

        if (type != 1)
        {
            throw new NotImplementedException();
        }

        using var fs = new FileStream(path, FileMode.Create);
        using var writer = new BinaryWriter(fs);
        writer.Write(MAGIC);
        writer.Write(MAJOR_VERSION);
        writer.Write(MINOR_VERSION);
        writer.Write(type);
        writer.Write(glyphFormat.Value);
        // Remember out the header position
        var headerOffset = fs.Position;
        // Skip Header size (LONG)
        fs.Skip(8);
        // Skip Header
        fs.Skip(tableSize);
        // Build up table and write body.
        using var tableStream = new MemoryStream();
        using var tableWriter = new BinaryWriter(tableStream);
        var bodyCurrent = 0;
        foreach (var item in table)
        {
            // Write table
            tableWriter.Write(item.CharCode);
            tableWriter.Write(item.HorizontalBearingX);
            tableWriter.Write(item.HorizontalBearingY);
            tableWriter.Write(item.HorizontalAdvance);
            tableWriter.Write((int)bodyCurrent);
            tableWriter.Write((int)(item.ImageDataPayload?.Length ?? 0));
            // Write body
            if (item.ImageDataPayload is not null)
            {
                writer.Write(item.ImageDataPayload);
                // Calculate body offset.
                bodyCurrent += item.ImageDataPayload?.Length ?? 0;
            }
        }
        // Write the header.
        fs.Seek(headerOffset, SeekOrigin.Begin);
        // Write the size of header.
        writer.Write(tableStream.Length);
        // Write the header.
        tableStream.Seek(0, SeekOrigin.Begin);
        tableStream.CopyTo(fs);
        // Flush.
        fs.Flush();
    }
}

internal static class FileStreamExtensions
{
    public static void Skip(this Stream stream, int length)
    {
        var payload = new byte[length];
        stream.Write(payload, 0, length);
    }
}