using RMEGo.Game.BMFSharp.Structures;

namespace RMEGo.Game.BMFSharp;

public record RBMFItem
{
    public uint CharCode { get; set; }
    public short HorizontalBearingX { get; set; }
    public short HorizontalBearingY { get; set; }
    public short HorizontalAdvance { get; set; }
    public long? PngBodyOffset { get; set; }
    public long? PngBodyLength { get; set; }
    public byte[]? PngBody { get; set; }
}

public class RBMFLoader
{
    private static readonly byte[] MAGIC = new byte[] { (byte)'R', (byte)'B', (byte)'M', (byte)'F' };
    private static readonly byte MAJOR_VERSION = 1;
    private static readonly byte MINOR_VERSION = 1;
    private static readonly byte TYPE = 1; // 1-single 2-bitmap

    private List<GlyphBitmap> _table = new();

    public void PushGlyph(GlyphBitmap glyph)
    {
        _table.Add(glyph);
    }

    public void LoadFromFile()
    {

    }

    public void SaveAsFile(string path)
    {
        // Calculate the size of header
        var singleTableItemLength = 4 + 2 + 2 + 2 + 8 + 8;
        var tableSize = singleTableItemLength * _table.Count;

        using var fs = new FileStream(path, FileMode.Create);
        using var writer = new BinaryWriter(fs);
        writer.Write(MAGIC);
        writer.Write(MAJOR_VERSION);
        writer.Write(MINOR_VERSION);
        writer.Write(TYPE);
        writer.Write((byte)0); // place holder.
        // Remember out the header position
        var headerOffset = fs.Position;
        // Skip Header size (LONG)
        fs.Skip(8);
        // Skip Header
        fs.Skip(tableSize);
        // Build up table and write body.
        using var tableStream = new MemoryStream();
        using var tableWriter = new BinaryWriter(tableStream);
        var bodyCurrent = 0L;
        foreach (var item in _table)
        {
            // Write table
            tableWriter.Write(item.CharCode);
            tableWriter.Write(item.HorizontalBearingX);
            tableWriter.Write(item.HorizontalBearingY);
            tableWriter.Write(item.HorizontalAdvance);
            tableWriter.Write((long)(item.PngDataPayload?.Length ?? 0));
            tableWriter.Write(bodyCurrent);
            // Write body
            if (item.PngDataPayload is not null)
            {
                writer.Write(item.PngDataPayload);
                // Calculate body offset.
                bodyCurrent += item.PngDataPayload?.Length ?? 0;
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