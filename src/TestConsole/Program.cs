using RMEGo.Game.BMFSharp;

var loader = new BMFLoader();
loader.LoadFromFile(@"D:\Codes\Game\BMFSharp\src\BMFSharpGenerator\bin\Debug\net6.0-windows\output\cjk_png_ff.bmf");
var renderer = new TextRenderer(loader.GetGlyphes());
renderer.PushLine("测试用文本");
renderer.PushLine("The quick brown fox jumps");
renderer.PushLine("over the lazy dogs.");
renderer.PushLine("1 + 1 = 2 -> !@#$%^&*()");
var png = renderer.DrawAsPng(1.0);
if (png is not null) File.WriteAllBytes("result.png", png);
else Console.WriteLine("Failed to draw. returns null.");