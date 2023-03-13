using DocuSign.eSign.Client.Auth;
using DocuSign.eSign.Model;
using DSClass;
using Microsoft.Extensions.Configuration;
using FluentAssertions;
namespace DocuSign.Test;


[TestFixture]
public class DSFirmaTests
{
    private DSFirma sut;
    private Signer Signer1, Signer2;
    private SignHere SignHere1, SignHere2;
    private Document doc1;
    private IConfigurationBuilder? Configuration { get; set; }
    
    [OneTimeSetUp]
    public void SetUp()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json");
        
        var config = Configuration.Build();
        
        var clientId = config["DocuSign:ClientId"];
        var userId = config["DocuSign:UserId"];
        var authServer = config["DocuSign:AuthServer"];
        var path = config["DocuSign:PrivateKeyFile"];
        string documentPath = config["DocuSign:TestFile"];;
        
        sut = new DSFirma(clientId, userId, authServer, path);

        Signer1 = new()
        {
            Email = "csegura@estrategiasdocumentales.com",
            Name = "cami",
            RecipientId = "1",
            RoutingOrder = "1",
        };
        Signer2 = new()
        {
            Email = "csegura+second@estrategiasdocumentales.com",
            Name = "crhis",
            RecipientId = "2",
            RoutingOrder = "2",
        };
        doc1 = DSDocument.CreateDocument(documentPath);

        SignHere1 = new SignHere
        {
            XPosition = "0",
            YPosition = "20",
            PageNumber = "1",
            DocumentId = doc1.DocumentId
        };

        SignHere2 = new SignHere
        {
            XPosition = "10",
            YPosition = "10",
            PageNumber = "1",
            DocumentId = doc1.DocumentId
        };
        
    }
    [Test]
    public void ConnectionSucessfull()
    {
        var aut = sut.ConectarJwt();
        aut?.access_token.Should().NotBeNull();
    }

    [Test]
    public void SignOnePerson_ShouldPass()
    {
        var aut = sut.ConectarJwt();
        if (aut is null) return;
        var signers = new List<Signer>() { Signer1 };
        
        // sut.SendEmail(documents,signers,positions);
    }
}