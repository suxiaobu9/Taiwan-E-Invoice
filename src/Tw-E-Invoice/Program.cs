//using SkiaSharp;
//using System.Drawing;
//using System.Drawing.Printing;

using ImageMagick;
using Invoice.Creater;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;


// https://github.com/dlemstra/Magick.NET/blob/main/docs/Readme.md


string logoPath = @"C:\tmp\logo.png",
    invoicePath = @"C:\tmp\invoice.png";


double dpi = 203d;

var doubleExtension = new DoubleExtension(dpi);

double cmWidth = 5.1d,
    cmHeight = 7.0d,
    pxPaperWidth = doubleExtension.ToPixal(cmWidth),
    pxHeight = doubleExtension.ToPixal(cmHeight),
    pxTitleFontSize = doubleExtension.ToPixal(0.65d),
    pxContentFontSize = doubleExtension.ToPixal(0.3d),
    pxContentShiftX = doubleExtension.ToPixal(0.3d),
    pxCode39Height = doubleExtension.ToPixal(0.65d),
    pxLogoSize = doubleExtension.ToPixal(1.3d);

var imageDrawer = new CreateInvoice(dpi);

using (var baseImage = imageDrawer.CreateImage((int)Math.Floor(pxPaperWidth), (int)Math.Floor(pxHeight)))
{
    imageDrawer.AddLogo(baseImage, logoPath, pxPaperWidth, pxLogoSize);
    imageDrawer.AddTextToCenter(baseImage, pxTitleFontSize, "電子發票證明聯", pxPaperWidth);
    imageDrawer.AddTextToCenter(baseImage, pxTitleFontSize, "102年05-06月", pxPaperWidth, true);
    imageDrawer.AddTextToCenter(baseImage, pxTitleFontSize, "AB-11223344", pxPaperWidth, true);
    imageDrawer.AddTextLeftRight(baseImage, pxContentFontSize, "隨機碼:9990", "總計:340", pxContentShiftX, pxPaperWidth);
    imageDrawer.AddTextLeftRight(baseImage, pxContentFontSize, "賣方:01234567", "買方:01234567", pxContentShiftX, pxPaperWidth);
    imageDrawer.AddCode39(baseImage, "11108CF067745194632", pxPaperWidth, pxCode39Height);

    string qr1 = "CF0677451911107104632000000fc000000fc00000000707752938btFhfsJbFqqto1bisUg2A==:**********:7:14:1:餐-雙層牛肉:1:65:零卡可-中:1:38:",
        qr2 = "**配-經典中薯:1:17:餐-大麥克:1:75:零卡可-中:1:38:配-經典中薯:1:17:$2塑膠袋:1:2";
    
    imageDrawer.AddQRCode(baseImage, qr1, qr2, 2, (int)pxPaperWidth, (int)pxContentShiftX);
    imageDrawer.Write(baseImage, invoicePath);

}

if (false)
{
    //產生列印物件
    var printDoc = new PrintDocument();
    printDoc.PrinterSettings.PrinterName = "Star BSC10 (複件 2)";

    // 綁定事件
    printDoc.PrintPage += PrintPage;
    printDoc.Print();

}


async void PrintPage(object o, PrintPageEventArgs e)
{
    var img = Image.FromFile(invoicePath);

    var loc = new Point(0, 0);
    e.Graphics.DrawImage(img, loc);
}


