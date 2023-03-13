using DocuSign.eSign.Client.Auth;
using DocuSign.eSign.Model;
using DSClass;
using Microsoft.Extensions.Configuration;
using FluentAssertions;
using NSubstitute;

namespace DocuSign.Test;


[TestFixture]
public class DSFirmaTests
{
    private DSFirma sut, sut2;
    private DSFirmante Signer1, Signer2;
    private Document doc1;
    private IConfigurationBuilder? Configuration { get; set; }

    [OneTimeSetUp]
    public void SetUp()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json");

        var config = Configuration.Build();

        var clientId = config["DocuSign:ClientId"];
        var clientId2 = Guid.NewGuid().ToString();

    var userId = config["DocuSign:UserId"];
        var authServer = config["DocuSign:AuthServer"];
        var path = config["DocuSign:PrivateKeyFile"];
        string documentPath = config["DocuSign:TestFile"]!;
        
        sut = new DSFirma(clientId, userId, authServer, path);

        sut2 = new DSFirma(clientId2, userId, authServer, path);

        doc1 = DSDocument.CreateDocument(documentPath);

        Signer1 = new DSFirmante(
            email:"csegura@estrategiasdocumentales.com",
            name : "cami",
            recipientId: "1",
            routingOrder:  "1",
            xPosition:  "0",
            yPosition:"20",
            pageNumber:"1",
            documentId : doc1.DocumentId
        );
        Signer2 = new(
            email : "csegura+second@estrategiasdocumentales.com",
            name : "crhis",
            recipientId : "2",
            routingOrder : "2",
            xPosition : "10",
            yPosition : "10",
            pageNumber : "1",
            documentId : doc1.DocumentId
        );
    }
    [Test]
    public void ConnectionSucessfull()
    {
        var aut = sut.ConectarJwt();
        aut?.access_token.Should().NotBeNull();
    }
    [Test]
    public void ConnectionNotSucessfull()
    {
        var aut = sut2.ConectarJwt();
        aut?.access_token.Should().NotBeNull();
    }

    [Test]
    public void SignOnePerson_ShouldPass()
    {
        var aut = sut.ConectarJwt();
        if (aut is null) return;
        var signers = new List<DSFirmante>() { Signer1 };
        var documents = new List<Document>() { doc1 };
        var env = sut.SendEmail(documents,signers, "Subject");

        env.Should().NotBeNull();
    }
    [Test]
    public void SignMoreThanOnePerson_ShouldPass()
    {
        var aut = sut.ConectarJwt();
        if (aut is null) return;
        var signers = new List<DSFirmante>() { Signer1, Signer2 };
        var documents = new List<Document>() { doc1 };
        var env = sut.SendEmail(documents,signers, "Subject");

        env.Should().NotBeNull();
    }
}