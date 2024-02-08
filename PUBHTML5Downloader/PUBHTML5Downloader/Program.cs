using static System.Net.Mime.MediaTypeNames;
using System.Net;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

string title = """
    ╔═══╗╔╗ ╔╗╔══╗ ╔╗ ╔╗╔════╗╔═╗╔═╗╔╗   ╔═══╗    ╔═══╗              ╔╗            ╔╗       
    ║╔═╗║║║ ║║║╔╗║ ║║ ║║║╔╗╔╗║║║╚╝║║║║   ║╔══╝    ╚╗╔╗║              ║║            ║║       
    ║╚═╝║║║ ║║║╚╝╚╗║╚═╝║╚╝║║╚╝║╔╗╔╗║║║   ║╚══╗     ║║║║╔══╗╔╗╔╗╔╗╔═╗ ║║ ╔══╗╔══╗ ╔═╝║╔══╗╔═╗
    ║╔══╝║║ ║║║╔═╗║║╔═╗║  ║║  ║║║║║║║║ ╔╗╚══╗║     ║║║║║╔╗║║╚╝╚╝║║╔╗╗║║ ║╔╗║╚ ╗║ ║╔╗║║╔╗║║╔╝
    ║║   ║╚═╝║║╚═╝║║║ ║║ ╔╝╚╗ ║║║║║║║╚═╝║╔══╝║    ╔╝╚╝║║╚╝║╚╗╔╗╔╝║║║║║╚╗║╚╝║║╚╝╚╗║╚╝║║║═╣║║ 
    ╚╝   ╚═══╝╚═══╝╚╝ ╚╝ ╚══╝ ╚╝╚╝╚╝╚═══╝╚═══╝    ╚═══╝╚══╝ ╚╝╚╝ ╚╝╚╝╚═╝╚══╝╚═══╝╚══╝╚══╝╚╝ 
    """;

Console.WriteLine(title);
Console.WriteLine("\nWhen you enter the link below, copy and paste using this format: \"pubhtml5.com/first/second\"\n");
string tempFolderPath = "ImgTemp";
Directory.CreateDirectory(tempFolderPath); // Create temporary folder

Console.Write("Give link: ");
string link = Console.ReadLine();
link = link.Trim();

if (link.StartsWith("http://"))
    link = link.Substring(7);

if (link.EndsWith("/"))
    link = link.Remove(link.Length - 1);

if (link.StartsWith("static."))
{
    Console.WriteLine("Sorry, this type is not implemented. Try some other book :(");
    return;
}
else if (!link.StartsWith("online."))
{
    link = "online." + link;
}
link = "http://" + link + "/files/large/";

Console.Write("Give number of pages: ");
int pageNumber = int.Parse(Console.ReadLine());

Console.Write("Enter desired output PDF file name (without extension): ");
string outputFileName = Console.ReadLine().Trim();
if (string.IsNullOrWhiteSpace(outputFileName))
{
    outputFileName = "output"; // Default file name if none is provided
}
string pdfPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{outputFileName}.pdf");

WebClient client = new WebClient();

for (int i = 1; i <= pageNumber; i++)
{
    string imageUrl = link + i.ToString() + ".jpg";
    Console.WriteLine("Downloading " + imageUrl);
    try
    {
        string imagePath = Path.Combine(tempFolderPath, i.ToString() + ".jpg");
        client.DownloadFile(new Uri(imageUrl), imagePath);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Exception while downloading: " + ex.Message);
    }
}

Console.WriteLine("Completed downloading");

// PDF creation
using (PdfDocument document = new PdfDocument())
{
    var files = Directory.GetFiles(tempFolderPath, "*.jpg").OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f)));
    foreach (var file in files)
    {
        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XImage image = XImage.FromFile(file);
        gfx.DrawImage(image, 0, 0, page.Width, page.Height);
    }

    document.Save(pdfPath);
}

// Cleanup
Directory.GetFiles(tempFolderPath).ToList().ForEach(File.Delete);
Directory.Delete(tempFolderPath);

Console.WriteLine("PDF has been created.");