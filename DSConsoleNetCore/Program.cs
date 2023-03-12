using Microsoft.Extensions.Configuration;

namespace DSConsoleNetCore;

using DocuSign.eSign.Client;
using static DocuSign.eSign.Client.Auth.OAuth;
using static DocuSign.eSign.Client.Auth.OAuth.UserInfo;
using System;
using System.Diagnostics;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.IO;

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
        Console.WriteLine($"Hello, World! {clientId}");
        OAuthToken accessToken = null;

        string DevCenterPage = "https://developers.docusign.com/platform/auth/consent";

        try
        {
            accessToken =
                JWTAuth.AuthenticateWithJWT("ESignature", clientId, userId, authServer, DSHelper.ReadFileContent(path));
        }
        catch (ApiException apiExp)
        {
            // Consent for impersonation must be obtained to use JWT Grant
            if (apiExp.Message.Contains("consent_required"))
            {
                // Caret needed for escaping & in windows URL
                string caret = "";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    caret = "^";
                }

                // build a URL to provide consent for this Integration Key and this userId
                string url = "https://" + authServer + "/oauth/auth?response_type=code" +
                             caret + "&scope=impersonation%20signature" + caret +
                             "&client_id=" + clientId + caret + "&redirect_uri=" +
                             DevCenterPage;
                Console.WriteLine($"Consent is required - launching browser (URL is {url.Replace(caret, "")})");

                // Start new browser window for login and consent to this app by DocuSign user
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = false });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    "Unable to send envelope; Exiting. Please rerun the console app once consent was provided");
                Console.ForegroundColor = ConsoleColor.White;
                Environment.Exit(-1);
            }
        }

        var docuSignClient = new DocuSignClient();
        docuSignClient.SetOAuthBasePath(authServer);
        UserInfo userInfo = docuSignClient.GetUserInfo(accessToken.access_token);
        Account acct = userInfo.Accounts.FirstOrDefault();

        Console.WriteLine("Welcome to the JWT Code example! ");
        Console.Write("Enter the signer's email address: ");
        string signerEmail = Console.ReadLine();
        Console.Write("Enter the signer's name: ");
        string signerName = Console.ReadLine();
        Console.Write("Enter the carbon copy's email address: ");
        string ccEmail = "demo@gmai.co";
        Console.Write("Enter the carbon copy's name: ");
        string ccName = "test";
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