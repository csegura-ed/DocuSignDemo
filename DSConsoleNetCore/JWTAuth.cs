﻿namespace DSConsoleNetCore;

using System;
using System.Collections.Generic;
using DocuSign.eSign.Client;
using static DocuSign.eSign.Client.Auth.OAuth;

public static class JWTAuth
{
    public static OAuthToken AuthenticateWithJWT(string api, string clientId, string impersonatedUserId,
        string authServer, byte[] privateKeyBytes)
    {
        var docuSignClient = new DocuSignClient();
        var apiType = Enum.Parse<ExamplesAPIType>(api);
        var scopes = new List<string>
        {
            "signature",
            "impersonation",
        };
        if (apiType == ExamplesAPIType.Rooms)
        {
            scopes.AddRange(new List<string>
            {
                "dtr.rooms.read",
                "dtr.rooms.write",
                "dtr.documents.read",
                "dtr.documents.write",
                "dtr.profile.read",
                "dtr.profile.write",
                "dtr.company.read",
                "dtr.company.write",
                "room_forms",
            });
        }

        if (apiType == ExamplesAPIType.Click)
        {
            scopes.AddRange(new List<string>
            {
                "click.manage",
                "click.send",
            });
        }

        if (apiType == ExamplesAPIType.Monitor)
        {
            scopes.AddRange(new List<string>
            {
                "signature",
                "impersonation",
            });
        }

        if (apiType == ExamplesAPIType.Admin)
        {
            scopes.AddRange(new List<string>
            {
                "user_read",
                "user_write",
                "account_read",
                "organization_read",
                "group_read",
                "permission_read",
                "identity_provider_read",
                "domain_read",
            });
        }

        return docuSignClient.RequestJWTUserToken(
            clientId,
            impersonatedUserId,
            authServer,
            privateKeyBytes,
            1,
            scopes);
    }
}