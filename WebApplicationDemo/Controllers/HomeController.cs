using System.Diagnostics;
using System.Runtime.InteropServices;
using DocuSign.eSign.Client;
using DocuSign.eSign.Client.Auth;
using DocuSign.eSign.Model;
using DSClass;
using Microsoft.AspNetCore.Mvc;
using WebApplicationDemo.Models;
using static DocuSign.eSign.Client.Auth.OAuth;
using static DocuSign.eSign.Client.Auth.OAuth.UserInfo;

namespace WebApplicationDemo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;
    private readonly string signerClientId = "1000";
    private readonly string dsPingUrl = "https://localhost:44361/";
    private readonly string dsReturnUrl = "https://localhost:44361/Result";

    private DSFirma firma { get; set; }
    private Document doc1 { get; set; }
    private DSFirmante Firmante { get; set; }

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        var clientId = _configuration["DocuSign:ClientId"];
        var userId = _configuration["DocuSign:UserId"];
        var authServer = _configuration["DocuSign:AuthServer"];
        var path = _configuration["DocuSign:PrivateKeyFile"];
        string documentPath = _configuration["DocuSign:TestFile"]!;

        firma = new DSFirma(clientId, userId, authServer, path);

        doc1 = DSDocument.CreateDocument(documentPath);
    }


    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(SignerData data, string? onMail, string? onWeb)
    {
        if (ModelState.IsValid)
        {
            var res = new ResultModel();

            var yPos = data.Position switch
            {
                Position.Arriba => "100",
                Position.Medio => "400",
                Position.Abajo => "700"
            };
            var subject_pos = data.Position switch
            {
                Position.Arriba => " arriba",
                Position.Medio => " medio",
                Position.Abajo => " abajo"
            };

            Firmante = new DSFirmante(
                email: data.Email,
                name: data.Name,
                recipientId: "1",
                routingOrder: "1",
                xPosition: "100",
                yPosition: yPos,
                pageNumber: "1",
                documentId: doc1.DocumentId
            );

            var aut = firma.ConectarJwt();
            Account? acct = null;
            if (aut is not null)
            {
                var docuSignClient = new DocuSignClient();
                docuSignClient.SetOAuthBasePath(_configuration["DocuSign:AuthServer"]);
                OAuth.UserInfo userInfo = docuSignClient.GetUserInfo(firma.AccessToken.access_token);
                acct = userInfo.Accounts.FirstOrDefault();
            }

            if (onWeb is not null)
            {
                res.Message = data.Name;
                res.Data = data;
                string accessToken = firma.AccessToken.access_token;
                string basePath = acct.BaseUri + "/restapi";
                string accountId = acct.AccountId;
               


                // Call the method from Examples API to send envelope and generate url for embedded signing
                var result = EmbeddedSigningCeremony.SendEnvelopeForEmbeddedSigning(
                    Firmante,
                    doc1,
                    this.signerClientId,
                    accessToken,
                    basePath,
                    accountId,
                    this.dsReturnUrl,
                    this.dsPingUrl);

                // Save for future use within the example launcher
                // this.RequestItemsService.EnvelopeId = result.Item1;

                // Redirect the user to the Signing Ceremony
                res.url = result.Item2;
                return RedirectToAction("FirmaWeb", "Result",new { url = res.url });
            }

            if (onMail is not null)
            {
                if (aut is null)
                {
                    res.Value = 0;
                    res.Message = "Problema de autenticacion";
                }
                else
                {
                    var envelopeResult = SigningViaEmail.SendEnvelopeViaEmail(data.Email, data.Name,
                        firma.AccessToken.access_token, acct.BaseUri + "/restapi", acct.AccountId, "sent",
                        doc1, yPos, $"Correo de prueba{subject_pos}.");

                    if (envelopeResult is not null)
                    {
                        res.Value = 1;
                        res.Message = "Envio correcto";
                    }
                }

                return RedirectToAction("Index", "Result", res);
            }
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}