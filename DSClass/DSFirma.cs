using DocuSign.eSign.Model;

using DocuSign.eSign.Client;
using static DocuSign.eSign.Client.Auth.OAuth;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DSClass;

public class DSFirma
{
    private string? ClientId { get; set; }
    private OAuthToken? AccessToken { get; set; }
    private string? UserId { get; set; }
    private string? AuthServer { get; set; }
    private string? PrivateKeyPath { get; set; }

    public DSFirma(string? clientId, string? userId, string? authServer, string? privateKeyPath)
    {
        this.ClientId = clientId;
        this.UserId = userId;
        this.AuthServer = authServer;
        this.PrivateKeyPath = privateKeyPath;
        this.AccessToken = null;
    }

    public OAuthToken? ConectarJwt()
    {
        const string devCenterPage = "https://developers.docusign.com/platform/auth/consent";

        try
        {
            AccessToken = JWTAuth.AuthenticateWithJWT("ESignature", ClientId, UserId,
                AuthServer, DSHelper.ReadFileContent(PrivateKeyPath));

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
                string url = "https://" + AuthServer + "/oauth/auth?response_type=code" +
                             caret + "&scope=impersonation%20signature" + caret +
                             "&client_id=" + ClientId + caret + "&redirect_uri=" +
                             devCenterPage;

                Console.WriteLine($"Consent is required - launching browser (URL is {url.Replace(caret, "")})");

                // Start new browser window for login and consent to this app by DocuSign user
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}")
                    {
                        CreateNoWindow = false
                    });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                Console.WriteLine(
                    "Unable to send envelope; Exiting. Please rerun the console app once consent was provided");
            }
        }
        return AccessToken;
    }

    public EnvelopeDefinition SendEmail(List<Document> documents, List<DSFirmante> signers, string subject)
    {
        EnvelopeDefinition env = MakeEnvelop(documents, signers);
        env.EmailSubject = subject;

        return env;
    }

    private EnvelopeDefinition MakeEnvelop(List<Document> documents, List<DSFirmante> signers)
    {
        var env = new EnvelopeDefinition();

        // The order in the docs array determines the order in the envelope
        env.Documents = documents;

        // Add the recipients to the envelope object
        Recipients recipients = new Recipients
        {
            Signers = signers.Select(x => x.Signer).ToList(),
            CarbonCopies = new List<CarbonCopy> { },
        };

        env.Recipients = recipients;

        // Request that the envelope be sent by setting |status| to "sent".
        var envStatus = "sent";
        env.Status = envStatus;

        return env;
    }
}