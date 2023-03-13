using DocuSign.eSign.Model;

namespace DSClass;

public class DSFirmante
{
    public Signer Signer { get; set; }
    public SignHere Position { get; set; }
    public Tabs SignerTabs; 

    public DSFirmante(string email, string name, string recipientId, string routingOrder, string xPosition, string yPosition, string pageNumber, string documentId)
    {
        this.Signer = new()
        {
            Email = email,
            Name = name,
            RecipientId = recipientId,
            RoutingOrder = routingOrder
        };

        this.Position = new()
        {
            XPosition = xPosition,
            YPosition = yPosition,
            PageNumber = pageNumber,
            DocumentId = documentId
        };

        this.SignerTabs = new Tabs
        {
            SignHereTabs = new List<SignHere> { Position },
        };

        this.Signer.Tabs = this.SignerTabs;
    }

    
}