using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Drawing;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.Primitives;
using System;
using System.IO;

namespace ProfileMgrApp.Helpers
{
    public class ImageHelper
    {
        public static string DrawRectangle(byte[] imageData, PointF[] points)
        {
            using (Image<Rgba32> image = Image.Load(imageData))
            {
                image.Mutate(ctx => ctx.DrawPolygon(Rgba32.Red, 5, points));

                using (var memoryStream = new MemoryStream())
                {
                    image.SaveAsJpeg(memoryStream);
                    return Convert.ToBase64String(memoryStream.ToArray());
                }                    
            }
        }

        // if height = 0, resize proportionally based on original size
        public static string Resize(byte[] imageData, int width, int height = 0)
        {
            using (Image<Rgba32> image = Image.Load(imageData))
            {
                if (height == 0) {
                    var size = image.Size();
                    float ratio = size.Width / width;
                    height = Convert.ToInt16(size.Height / ratio);
                }
                image.Mutate(ctx => ctx.Resize(width, height));

                using (var memoryStream = new MemoryStream())
                {
                    image.SaveAsJpeg(memoryStream);
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }
    }
}
