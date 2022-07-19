using ImageMagick;
using Net.Codecrete.QrCodeGenerator;
using SkiaSharp;
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

        public static MagickImage ToImage(this QrCode qrCode, int scale, int border, MagickColor foreground, MagickColor background)
        {
            return qrCode.ToImage(scale, border, foreground, background, 96);
        }

        public static MagickImage ToImage(this QrCode qrCode, int scale, int border, int dpi)
        {
            return qrCode.ToImage(scale, border, MagickColors.Black, MagickColors.White, dpi);
        }

        public static MagickImage ToImage(this QrCode qrCode, int scale, int border)
        {
            return qrCode.ToImage(scale, border, MagickColors.Black, MagickColors.White, 96);
        }

        public static byte[] ToPng(this QrCode qrCode, int scale, int border, MagickColor foreground, MagickColor background, int dpi)
        {
            using (var image = qrCode.ToImage(scale, border, foreground, background, dpi))
            using (var stream = new MemoryStream())
            {
                image.Write(stream);
                var result = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(result, 0, result.Length);
                return result;
            }
        }
        public static byte[] ToPng(this QrCode qrCode, int scale, int border, MagickColor foreground, MagickColor background)
        {
            return qrCode.ToPng(scale, border, foreground, background, 96);
        }

        public static byte[] ToPng(this QrCode qrCode, int scale, int border, int dpi)
        {
            return qrCode.ToPng(scale, border, MagickColors.Black, MagickColors.White, dpi);
        }

        public static byte[] ToPng(this QrCode qrCode, int scale, int border)
        {
            return qrCode.ToPng(scale, border, MagickColors.Black, MagickColors.White, 96);
        }

        public static void SaveAsPng(this QrCode qrCode, string fileName, int scale, int border, MagickColor foreground, MagickColor background, int dpi)
        {
            using (var image = qrCode.ToImage(scale, border, foreground, background, dpi))
                image.Write(fileName);
        }

        public static void SaveAsPng(this QrCode qrCode, string fileName, int scale, int border, MagickColor foreground, MagickColor background)
        {
            qrCode.SaveAsPng(fileName, scale, border, foreground, background, 96);
        }

        public static void SaveAsPng(this QrCode qrCode, string fileName, int scale, int border, int dpi)
        {
            qrCode.SaveAsPng(fileName, scale, border, MagickColors.Black, MagickColors.White, dpi);
        }

        public static void SaveAsPng(this QrCode qrCode, string fileName, int scale, int border)
        {
            qrCode.SaveAsPng(fileName, scale, border, MagickColors.Black, MagickColors.White, 96);
        }

    }
}
