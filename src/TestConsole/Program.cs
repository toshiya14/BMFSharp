using RMEGo.Game.BMFSharp;

var loader = new BMFLoader();
loader.LoadFromFile(@"D:\Codes\Game\BMFSharp\src\BMFSharpGenerator\bin\Debug\net6.0-windows\output\atlas.bmf");
var renderer = new TextRenderer(loader.GetGlyphes());
renderer.PushLine("測試用文本");
renderer.PushLine("第三天那些遠來的人們，一樣是歹命人！");
renderer.PushLine("又有人不平地說，不是一件很合理得快活的事嗎？");
renderer.PushLine("西門那賣點心的老人，地方領導人。");
var png = renderer.DrawAsPng(1.0);
if (png is not null) File.WriteAllBytes("result.png", png);
else Console.WriteLine("Failed to draw. returns null.");