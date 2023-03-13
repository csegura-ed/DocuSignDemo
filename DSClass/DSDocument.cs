using DocuSign.eSign.Model;

namespace DSClass;

public class DSDocument
{
    public static Document CreateDocument(string documentPath, string documentNumber = "1", string? fileName=null)
    {
        var name = fileName ?? documentPath.Split('/').Last().Split('.').First(); 
        var b64 = Convert.ToBase64String(File.ReadAllBytes(documentPath));
        return new Document()
        {
            DocumentBase64 = b64,
            Name = "Order acknowledgement",
            DocumentId = documentNumber,
            FileExtension = documentPath.Split('.').Last()!
        };
    }
}