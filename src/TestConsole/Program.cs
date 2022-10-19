using System.Diagnostics;
using RMEGo.Game.BMFSharp;

var loadingStopwatch = Stopwatch.StartNew();
var loader = new BMFLoader();
loader.LoadFromFile(@"D:\Repos\github-toshiya14\BMFSharp\src\BMFSharpGenerator\bin\Debug\net6.0-windows\output\atlas.bmf");
loadingStopwatch.Stop();

var renderStopwatch = Stopwatch.StartNew();
var renderer = new TextRenderer(loader.GetGlyphes());
renderer.PushLine("測試文本", TextAlign.Middle);
renderer.PushLine("第三天那些遠來的人們，一樣是歹命人！");
renderer.PushLine("又有人不平地說，不是一件很合理得快活的事嗎？");
renderer.PushLine("西門那賣點心的老人，不知是何人。");
renderer.PushLine("——無意義文本生成器", TextAlign.Right);
var png = renderer.DrawAsPng(1.0);
renderStopwatch.Stop();

if (png is not null) File.WriteAllBytes("result.png", png);
else Console.WriteLine("Failed to draw. returns null.");

Console.WriteLine($"Loading: {loadingStopwatch.ElapsedMilliseconds} ms");
Console.WriteLine($"Render : {renderStopwatch.ElapsedMilliseconds} ms");