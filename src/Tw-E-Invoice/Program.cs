using Invoice.Creater;
using System.Drawing;
using System.Drawing.Printing;

string logoPath = @"C:\tmp\logo.png",
    invoicePath = @"C:\tmp\invoice.png";

double dpi = 203d;

var doubleExtension = new DoubleExtension(dpi);

double cmWidth = 5.1d,
    cmHeight = 7.5d,
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
    imageDrawer.AddTextToCenter(baseImage, pxTitleFontSize, "111年07-08月", pxPaperWidth, true);
    imageDrawer.AddTextToCenter(baseImage, pxTitleFontSize, "AB-11223344", pxPaperWidth, true);
    imageDrawer.AddTextToLeft(baseImage, pxContentFontSize,"2022-07-25 11:12:59", pxContentShiftX);
    imageDrawer.AddTextLeftRight(baseImage, pxContentFontSize, "隨機碼:9990", "總計:340", pxContentShiftX, pxPaperWidth);
    imageDrawer.AddTextToLeft(baseImage, pxContentFontSize, "賣方:01234567", pxContentShiftX);
    //imageDrawer.AddTextLeftRight(baseImage, pxContentFontSize, "賣方:01234567", "買方:01234567", pxContentShiftX, pxPaperWidth);
    imageDrawer.AddCode39(baseImage, "11108AB112233449990", pxPaperWidth, pxCode39Height);

    string qr1 = "AB1122334411107259990000000fc000000fc00000000012345678btFhfsJbFqqto1bisUg2A==:**********:7:14:1:餐-雙層牛肉:1:65:零卡可-中:1:38:",
        qr2 = "**配-經典中薯:1:17:餐-大麥克:1:75:零卡可-中:1:38:配-經典中薯:1:17:$2塑膠袋:1:2";

    imageDrawer.AddQRCode(baseImage, qr1, qr2, 3, (int)pxPaperWidth);
    imageDrawer.AddTextToLeft(baseImage, pxContentFontSize, "----------------------------------", pxContentShiftX);
    
    imageDrawer.Write(baseImage, invoicePath);
}

var print = false;
//print = true;

if (print)
{
    //產生列印物件
    var printDoc = new PrintDocument();

    foreach (var printer in PrinterSettings.InstalledPrinters)
    {
        if (!printer.ToString()?.Contains("Star BSC10") ?? false)
            continue;

        printDoc.PrinterSettings.PrinterName = printer.ToString() ?? "";

        if (printDoc.PrinterSettings.IsValid)
        {
            // 綁定事件
            printDoc.PrintPage += PrintPage;
            printDoc.Print();
            break;
        }
    }
}


async void PrintPage(object o, PrintPageEventArgs e)
{
    var img = Image.FromFile(invoicePath);

    var loc = new Point(0, 0);
    e.Graphics.DrawImage(img, loc);
}


