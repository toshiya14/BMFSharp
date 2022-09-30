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
var font = deserializer.Deserialize<FontSpec>(File.ReadAllText("./TestCases/Salma_12.yml"));
var output = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");
if (outputDebugFiles)
    Directory.CreateDirectory(output);
var list = GlyphMapGenerator.Make(font, outputDebugFiles: true);
var loader = new RBMFLoader();

foreach (var item in list) loader.PushGlyph(item);

loader.SaveAsFile(Path.Combine(output, "atlas.rbmf"));
Console.ReadLine();


