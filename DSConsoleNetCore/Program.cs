using DSClass;
using Microsoft.Extensions.Configuration;

namespace DSConsoleNetCore;

using System;
using DocuSign.eSign.Client;
using System.Linq;
using static DocuSign.eSign.Client.Auth.OAuth;
using static DocuSign.eSign.Client.Auth.OAuth.UserInfo;

internal class Program
{
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json");

        var config = configuration.Build();
        var clientId = config["DocuSign:ClientId"];
        var userId = config["DocuSign:UserId"];
        var authServer = config["DocuSign:AuthServer"];
        var path = config["DocuSign:PrivateKeyFile"];
        
        DSFirma firma = new DSFirma(clientId,userId,authServer,path);

        OAuthToken? accessToken = firma.ConectarJwt();

        var docuSignClient = new DocuSignClient();
        docuSignClient.SetOAuthBasePath(authServer);
        UserInfo userInfo = docuSignClient.GetUserInfo(accessToken.access_token);
        Account acct = userInfo.Accounts.FirstOrDefault();

        Console.WriteLine("Welcome to the JWT Code example! ");
        Console.Write("Enter the signer's email address: ");
        string signerEmail = "csegura@estrategiasdocumentales.com";
        Console.Write("Enter the signer's name: ");
        string signerName = "Camilos";
        string ccEmail = "csegura+demo@estrategiasdocumentales.com";
        string ccName = "crhis";
        // string docDocx = Path.Combine(@"..", "..", "..", "..", "launcher-csharp", "World_Wide_Corp_salary.docx");
        // string docPdf = Path.Combine(@"..", "..", "..", "..", "launcher-csharp", "World_Wide_Corp_lorem.pdf");
        Console.WriteLine("");
        string envelopeId = SigningViaEmail.SendEnvelopeViaEmail(signerEmail, signerName, ccEmail, ccName,
            accessToken.access_token, acct.BaseUri + "/restapi", acct.AccountId, "sent");
        Console.WriteLine("");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Successfully sent envelope with envelopeId {envelopeId}");
        Console.WriteLine("");
        Console.WriteLine("");
        Console.ForegroundColor = ConsoleColor.White;
        Environment.Exit(0);
    }
}