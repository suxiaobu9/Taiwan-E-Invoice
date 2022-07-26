using ImageMagick;
using Net.Codecrete.QrCodeGenerator;
using System;
using System.IO;

namespace Invoice.Creater
{
    public static class QrCodeBitmapExtensions
    {
        public static MagickImage ToImage(this QrCode qrCode, int scale, int border, MagickColor foreground, MagickColor background, int dpi)
        {
            if (scale <= 0)
                throw new ArgumentOutOfRangeException(nameof(scale), " Value out of range");
            if (border < 0)
                throw new ArgumentOutOfRangeException(nameof(border), " Value out of range");

            var size = qrCode.Size;
            var dim = (size + border * 2) * scale;

            if (dim > short.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(scale), " Scale or border too large");

            var image = new MagickImage(background, dim, dim)
            {
                Format = MagickFormat.Png,
                Density = new Density(dpi)
            };

            var drawables = new Drawables();
            drawables.FillColor(foreground);

            for (var x = 0; x < size; x++)
            {
                var pointerX = (x + border) * scale;

                for (var y = 0; y < size; y++)
                {
                    if (qrCode.GetModule(x, y))
                    {
                        var pointerY = (y + border) * scale;

                        drawables.Rectangle(pointerX, pointerY, pointerX + scale - 1, pointerY + scale - 1);
                    }
                }
            }
            drawables.Draw(image);
            return image;
        }

    }
}
