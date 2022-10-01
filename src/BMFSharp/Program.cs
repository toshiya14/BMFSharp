// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Drawing;
using RMEGo.Game.BMFSharp;
using RMEGo.Game.BMFSharp.Structures;
using SharpFont;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var outputDebugFiles = true;
var deserializer = new DeserializerBuilder()
    .WithNamingConvention(UnderscoredNamingConvention.Instance)
    .Build();
var font = deserializer.Deserialize<FontSpec>(File.ReadAllText("./TestCases/GenEiLateMin_16.yml"));
var output = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");
if (outputDebugFiles)
    Directory.CreateDirectory(output);

var watch = Stopwatch.StartNew();
var cols = GlyphMapGenerator.Make(font);
watch.Stop();
Console.WriteLine($"MakeGlyphMap: {watch.ElapsedMilliseconds} ms");

watch = Stopwatch.StartNew();

var loader = new BMFLoader();
loader.PutGlyphes(cols);

loader.SaveAsFile(Path.Combine(output, "atlas.bmf"));
watch.Stop();
Console.WriteLine($"SaveAsFile: {watch.ElapsedMilliseconds} ms");

Console.ReadLine();


