
using PdfSharp.Fonts;
using System.IO;

namespace PropertyManagementAPI.Application.Services.InvoiceExport;

public class CustomFontResolver : IFontResolver
{
    public static readonly CustomFontResolver Instance = new CustomFontResolver();

    public string DefaultFontName => "Verdana";

    public byte[] GetFont(string faceName)
    {
        var fontPath = Path.Combine("C:\\Windows\\Fonts", faceName);
        return File.Exists(fontPath) ? File.ReadAllBytes(fontPath) : null;
    }

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (familyName.Equals("Verdana", StringComparison.OrdinalIgnoreCase))
        {
            if (isBold && isItalic) return new FontResolverInfo("verdana-bold-italic.ttf");
            if (isBold) return new FontResolverInfo("verdana-bold.ttf");
            if (isItalic) return new FontResolverInfo("verdana-italic.ttf");
            return new FontResolverInfo("verdana.ttf");
        }
        return null;
    }
}
