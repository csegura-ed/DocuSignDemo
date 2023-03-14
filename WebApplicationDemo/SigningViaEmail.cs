namespace WebApplicationDemo;

using System;
using System.Collections.Generic;
using System.Text;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;

public class SigningViaEmail
{
    public static EnvelopeSummary SendEnvelopeViaEmail(string signerEmail, string signerName,
        string accessToken, string basePath, string accountId, string envStatus, Document document, string yPos,
        string subject)
    {
        // Step 1 start
        EnvelopeDefinition env = MakeEnvelope(signerEmail, signerName, envStatus, document, yPos);
        var docuSignClient = new DocuSignClient(basePath);
        docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
        env.EmailSubject = subject;
        env.EmailBlurb = "Easte texto se puede ajustar para la firma";
        EnvelopesApi envelopesApi = new EnvelopesApi(docuSignClient);
        EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, env);
        return results;

        // Step 1 end
    }

    public static EnvelopeDefinition MakeEnvelope(string signerEmail, string signerName, string envStatus, Document doc,
        string yPos)
    {
        // Step 3 start
        EnvelopeDefinition env = new EnvelopeDefinition();
        env.EmailSubject = "Please sign this document set";

        // Create document objects, one per document
        Document doc1 = doc;
        // The order in the docs array determines the order in the envelope
        env.Documents = new List<Document> { doc1 }; //, doc2, doc3 };
        // create a signer recipient to sign the document, identified by name and email
        // We're setting the parameters via the object creation
        Signer signer1 = new Signer
        {
            Email = signerEmail,
            Name = signerName,
            RecipientId = "1",
            RoutingOrder = "1",
        };
        // use the same anchor string for their "signer 1" tabs.
        SignHere signHere1 = new SignHere
        {
            XPosition = "100",
            YPosition = yPos,
            PageNumber = "1",
            DocumentId = doc1.DocumentId
        };

        // Tabs are set per recipient / signer
        Tabs signer1Tabs = new Tabs
        {
            SignHereTabs = new List<SignHere> { signHere1 }
        };
        signer1.Tabs = signer1Tabs;


        // Add the recipients to the envelope object
        Recipients recipients = new Recipients
        {
            Signers = new List<Signer> { signer1 },
        };

        env.Recipients = recipients;

        // Request that the envelope be sent by setting |status| to "sent".
        // To request that the envelope be created as a draft, set to "created"
        env.Status = envStatus;

        return env;
        // Step 3 end
    }
}