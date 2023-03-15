using DSClass;

namespace WebApplicationDemo;

using System;
using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;

public static class EmbeddedSigningCeremony
{
    public static (string, string) SendEnvelopeForEmbeddedSigning(
        DSFirmante firmante,
        Document document,
        string signerClientId,
        string accessToken,
        string basePath,
        string accountId,
        string returnUrl,
        string pingUrl = null)
    {
        // Step 1 start
        // Step 1. Create the envelope definition
        EnvelopeDefinition envelope = MakeEnvelope(firmante.Signer, signerClientId, document);

        // Step 1 end

        // Step 2 start
        // Step 2. Call DocuSign to create the envelope
        var docuSignClient = new DocuSignClient(basePath);
        docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

        EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
        EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, envelope);
        string envelopeId = results.EnvelopeId;

        // Step 2 end

        // Step 3 start
        // Step 3. create the recipient view, the Signing Ceremony
        RecipientViewRequest viewRequest =
            MakeRecipientViewRequest(firmante.Signer, returnUrl, signerClientId, pingUrl);

        // call the CreateRecipientView API
        ViewUrl results1 = envelopesApi.CreateRecipientView(accountId, envelopeId, viewRequest);

        // Step 3 end

        // query parameter on the returnUrl (see the makeRecipientViewRequest method)
        string redirectUrl = results1.Url;

        // returning both the envelopeId as well as the url to be used for embedded signing
        return (envelopeId, redirectUrl);

        // Step 4 end
    }

    public static RecipientViewRequest MakeRecipientViewRequest(Signer firmante, string returnUrl,
        string signerClientId, string pingUrl = null)
    {
        // dsReturnUrl -- class global
        RecipientViewRequest viewRequest = new RecipientViewRequest();

        // can be changed/spoofed very easily.
        viewRequest.ReturnUrl = returnUrl + "?state=123";

        // Eg, SMS authentication
        viewRequest.AuthenticationMethod = "none";

        // Recipient information must match embedded recipient info
        // we used to create the envelope.
        viewRequest.Email = firmante.Email;
        viewRequest.UserName = firmante.Name;
        viewRequest.ClientUserId = signerClientId;

        if (pingUrl != null)
        {
            viewRequest.PingFrequency = "600"; // seconds
            viewRequest.PingUrl = pingUrl; // optional setting
        }

        return viewRequest;
    }

    public static EnvelopeDefinition MakeEnvelope(Signer firmante, string signerClientId, Document document)
    {
        EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition();
        envelopeDefinition.EmailSubject = "Please sign this document";
        Document doc1 = document;

        // The order in the docs array determines the order in the envelope
        envelopeDefinition.Documents = new List<Document> { doc1 };

        firmante.ClientUserId = signerClientId;

        // Add the recipient to the envelope object
        Recipients recipients = new Recipients
        {
            Signers = new List<Signer> { firmante },
        };
        envelopeDefinition.Recipients = recipients;

        // Request that the envelope be sent by setting |status| to "sent".
        // To request that the envelope be created as a draft, set to "created"
        envelopeDefinition.Status = "sent";

        return envelopeDefinition;
    }
}