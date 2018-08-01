using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Drawing;
using SixLabors.Primitives;
using System.IO;
using System.Numerics;

namespace ConsoleApp1
{
    public class ImageProccessor
    {
        static void TestMain()
        {
            string imgFile = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\..\\images\\test_pics\\Test4.jpg");

            using (Image<Rgba32> image = Image.Load(imgFile))
            {
                var borderSize = 3;

                image.Mutate(ctx => ctx.DrawPolygon(Rgba32.Red, borderSize, new PointF[] {
                        new Vector2(646, 367),
                        new Vector2(646, 367+237),
                        new Vector2(646+237, 367+237),
                        new Vector2(646+237, 367)
                    }));

                image.Save("bar.jpg"); // Automatic encoder selected based on extension.
            }
        }
    }
}
