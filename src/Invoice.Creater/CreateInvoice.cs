using ImageMagick;
using Net.Codecrete.QrCodeGenerator;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Invoice.Creater
{
    public class CreateInvoice
    {
        private readonly double dpi;
        private readonly DoubleExtension doubleExtension;
        private double PxYPointer { get; set; } = 0d;

        public CreateInvoice(double dpi)
        {
            this.dpi = dpi;
            doubleExtension = new DoubleExtension(dpi);
        }

        /// <summary>
        /// 建立圖層
        /// </summary>
        /// <param name="pxWidth"></param>
        /// <param name="pxHeight"></param>
        /// <param name="backgroundColor"></param>
        /// <returns></returns>
        public MagickImage CreateImage(int pxWidth, int pxHeight, MagickColor backgroundColor = null)
        {
            backgroundColor = backgroundColor ?? MagickColors.Transparent;
            return new MagickImage(backgroundColor, pxWidth, pxHeight)
            {
                Format = MagickFormat.Png,
                Density = new Density(dpi)
            };
        }

        /// <summary>
        /// 輸出圖片
        /// </summary>
        /// <param name="image"></param>
        /// <param name="path"></param>
        public void Write(MagickImage image, string path)
        {
            image.Write(path);
        }

        /// <summary>
        /// 壓 LOGO
        /// </summary>
        /// <param name="image"></param>
        /// <param name="logoPath"></param>
        /// <param name="pxWidth"></param>
        /// <param name="pxHeight"></param>
        public void AddLogo(MagickImage image, string logoPath, double pxWidth, double pxHeight)
        {
            using (var logoImage = new MagickImage(logoPath))
            {
                var resizeSettings = new MagickGeometry((int)Math.Floor(pxWidth), (int)Math.Floor(pxHeight))
                {
                    IgnoreAspectRatio = true,
                };

                logoImage.Resize(resizeSettings);
                image.Composite(logoImage, 0, (int)Math.Floor(PxYPointer), CompositeOperator.Over);
                PxYPointer += pxHeight;
            }
        }

        /// <summary>
        /// 置中文字
        /// </summary>
        /// <param name="image"></param>
        /// <param name="fontSize"></param>
        /// <param name="text"></param>
        /// <param name="pxWidth"></param>
        /// <param name="isBold"></param>
        public void AddTextToCenter(MagickImage image, double pxFontSize, string text, double pxWidth, bool isBold = false)
        {
            PxYPointer += pxFontSize;

            var drawables = new Drawables();

            drawables.TextEncoding(Encoding.UTF8)
                .FontPointSize(pxFontSize)
                .Text(pxWidth / 2, PxYPointer, text)
                .TextAlignment(TextAlignment.Center);

            AssignFontFamily(drawables, isBold);

            drawables.Draw(image);
        }

        /// <summary>
        /// 置左文字
        /// </summary>
        /// <param name="image"></param>
        /// <param name="pxFontSize"></param>
        /// <param name="text"></param>
        /// <param name="isBold"></param>
        public void AddTextToLeft(MagickImage image, double pxFontSize, string text, double pxShiftX, bool isBold = false)
        {
            PxYPointer += pxFontSize;

            Drawables drawables = new Drawables();

            drawables.TextEncoding(Encoding.UTF8)
                .FontPointSize(pxFontSize)
                .Text(pxShiftX, PxYPointer, text);

            AssignFontFamily(drawables, isBold);

            drawables.Draw(image);
        }

        public void AddTextLeftRight(MagickImage image, double pxFontSize, string textLeft, string textRight, double pxMarginX, double pxWidth)
        {
            PxYPointer += pxFontSize;
            var leftDrawables = new Drawables();
            leftDrawables.TextEncoding(Encoding.UTF8)
                .FontPointSize(pxFontSize)
                .TextAlignment(TextAlignment.Left)
                .Text(pxMarginX, PxYPointer, textLeft);
            AssignFontFamily(leftDrawables, false);

            leftDrawables.Draw(image);

            var rightDrawables = new Drawables();
            rightDrawables.TextEncoding(Encoding.UTF8)
                .FontPointSize(pxFontSize)
                .TextAlignment(TextAlignment.Right)
                .Text(pxWidth - pxMarginX, PxYPointer, textRight);
            AssignFontFamily(rightDrawables, false);

            rightDrawables.Draw(image);
        }

        /// <summary>
        /// 取得字型
        /// </summary>
        /// <param name="drawables"></param>
        /// <param name="isBold"></param>
        /// <returns></returns>
        private void AssignFontFamily(Drawables drawables, bool isBold)
        {
            var fontFamily = isBold ? "Microsoft JhengHei Bold & Microsoft JhengHei UI" : "Microsoft JhengHei & Microsoft JhengHei UI";

            fontFamily = MagickNET.FontFamilies.FirstOrDefault(x => x == fontFamily);

            if (fontFamily != null)
                drawables.Font(fontFamily);
        }

        public void AddQRCode(MagickImage image, string source1, string source2, int scale, int pxPaperWidth, int pxContentShiftX)
        {
            var qr1 = QrCode.EncodeText(source1, QrCode.Ecc.High);
            var qr2 = QrCode.EncodeText(source2, QrCode.Ecc.High);

            var qrCodeImage1 = qr1.ToImage(scale, 0, MagickColors.Black, MagickColors.Transparent, 203);
            var qrCodeImage2 = qr2.ToImage(scale, 0, MagickColors.Black, MagickColors.Transparent, 203);

            PxYPointer += doubleExtension.ToPixal(0.2d);

            var qrCodeWidth = (int)Math.Ceiling(doubleExtension.ToPixal(1.5d));

            var size = new MagickGeometry(qrCodeWidth, qrCodeWidth)
            {
            };

            qrCodeImage1.Resize(size);
            qrCodeImage2.Resize(size);

            image.Composite(qrCodeImage1, pxPaperWidth / 4 - qrCodeWidth/2, (int)Math.Floor(PxYPointer), CompositeOperator.Over);
            image.Composite(qrCodeImage2, (pxPaperWidth / 4 * 3) - qrCodeWidth/2, (int)Math.Floor(PxYPointer), CompositeOperator.Over);
            PxYPointer += qr1.Size > qr2.Size ? qr1.Size : qr2.Size;
        }

        /// <summary>
        /// 增加 Code 39
        /// </summary>
        /// <param name="image"></param>
        /// <param name="source"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void AddCode39(MagickImage image, string strSource, double pxWidth, double pxHeight)
        {
            //Code39的各字母對應碼
            var code39 = new Dictionary<char, string>
            {
                {'0', "000110100"}, {'1', "100100001"}, {'2', "001100001"}, {'3', "101100000"},
                {'4', "000110001"}, {'5', "100110000"}, {'6', "001110000"}, {'7', "000100101"},
                {'8', "100100100"}, {'9', "001100100"}, {'A', "100001001"}, {'B', "001001001"},
                {'C', "101001000"}, {'D', "000011001"}, {'E', "100011000"}, {'F', "001011000"},
                {'G', "000001101"}, {'H', "100001100"}, {'I', "001001100"}, {'J', "000011100"},
                {'K', "100000011"}, {'L', "001000011"}, {'M', "101000010"}, {'N', "000010011"},
                {'O', "100010010"}, {'P', "001010010"}, {'Q', "000000111"}, {'R', "100000110"},
                {'S', "001000110"}, {'T', "000010110"}, {'U', "110000001"}, {'V', "011000001"},
                {'W', "111000000"}, {'X', "010010001"}, {'Y', "110010000"}, {'Z', "011010000"},
                {'-', "010000101"}, {'.', "110000100"}, {' ', "011000100"}, {'$', "010101000"},
                {'/', "010100010"}, {'+', "010001010"}, {'%', "000101010"}, {'*', "010010100" }
            };

            foreach (var item in strSource)
                if (!code39.ContainsKey(item) || item == '*')
                    throw new Exception($"含有非法字元 {strSource}");

            strSource = $"*{strSource.ToUpper()}*";

            var code39Ary = strSource.Select(x => code39[x]).ToArray();

            var strEncode = string.Join("0", code39Ary);


            int pxPointerX = 0,
                pxCode39Width = strEncode.Count(x => x == '1') * 3 + strEncode.Count(x => x == '0') * 1;

            //實作圖片
            using (var code39Image = CreateImage(pxCode39Width, (int)pxHeight, MagickColors.Transparent))
            {
                var drawables = new Drawables();
                drawables.StrokeAntialias(false);

                for (var i = 0; i < strEncode.Length; i++) //依碼畫出Code39 BarCode
                {
                    var lineColor = i % 2 == 0 ? MagickColors.Black : MagickColors.Transparent;

                    int intBarWidth = strEncode[i] == '1' ? 3 : 1,
                        pxShiftX = strEncode[i] == '1' ? 1 : 0;

                    drawables.Line(pxPointerX + pxShiftX, 0, pxPointerX + pxShiftX, pxHeight)
                        .FillColor(lineColor)
                        .StrokeWidth(intBarWidth)
                        .StrokeColor(lineColor);

                    pxPointerX += intBarWidth;
                }

                drawables.Draw(code39Image);

                PxYPointer += doubleExtension.ToPixal(0.2d);

                image.Composite(code39Image, (int)((pxWidth - pxCode39Width) / 2), (int)Math.Floor(PxYPointer), CompositeOperator.Over);
                PxYPointer += pxHeight;
            }
        }
    }
}
