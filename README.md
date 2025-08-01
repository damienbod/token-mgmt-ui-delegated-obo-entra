# Token Management using Entra ID and Microsoft On-Behalf-Of flow

ASP.NET Core Token Management using Microsoft On-Behalf-Of flow (delegated tokens)

## Setup

![ASP.NET Core access token management](https://github.com/damienbod/token-mgmt-ui-delegated-obo-entra/blob/main/images/context.png)

## Blogs in this series

- [ASP.NET Core user delegated access token management](https://damienbod.com/2025/01/15/asp-net-core-user-delegated-access-token-management/)
- [ASP.NET Core user application access token management](https://damienbod.com/2025/01/20/asp-net-core-user-application-access-token-management/)
- [ASP.NET Core delegated OAuth token exchange access token management](https://damienbod.com/2025/02/10/asp-net-core-delegated-oauth-token-exchange-access-token-management/)
- [ASP.NET Core delegated Microsoft OBO access token management (Entra only)](https://damienbod.com/2025/03/25/asp-net-core-delegated-microsoft-obo-access-token-management-entra-only/)

## Further examples of the Microsoft On-Behalf-Of flow

### Microsoft OBO with Azure Blob Storage (delegated)

ASP.NET Core Razor page using Azure Blob Storage to upload download files securely using OAuth and Open ID Connect

https://github.com/damienbod/AspNetCoreEntraIdBlobStorage

### Microsoft OBO with OpenIddict (delegated)

This demo shows how to implement the On-Behalf-Of flow between an Microsoft Entra ID protected API and an API protected using OpenIddict.

https://github.com/damienbod/OnBehalfFlowOidcDownstreamApi

### ASP.NET Core OBO using Microsoft Graph (delegated)

Backend for frontend security using Angular Standalone (nx) and ASP.NET Core backend using Microsoft Graph

https://github.com/damienbod/bff-aspnetcore-angular

## History

- 2025-08-01 Updated packages
- 2025-03-25 Updated packages
- 2025-03-01 Updated packages
- 2025-02-07 Initial version

## Links

https://damienbod.com/2024/02/12/using-blob-storage-from-asp-net-core-with-entra-id-authentication/

https://damienbod.com/2023/01/09/implement-the-oauth-2-0-token-exchange-delegated-flow-between-an-azure-ad-api-and-an-api-protected-using-openiddict/

https://github.com/damienbod/OAuthGrantExchangeOidcDownstreamApi

https://docs.duendesoftware.com/identityserver/v7/tokens/extension_grants/token_exchange/

https://datatracker.ietf.org/doc/html/rfc8693

https://www.youtube.com/watch?v=Ue8HKBGkIJY&t=

https://github.com/damienbod/OnBehalfFlowOidcDownstreamApi

https://www.rfc-editor.org/rfc/rfc6749#section-5.2

https://github.com/blowdart/idunno.Authentication/tree/dev/src/idunno.Authentication.Basic

https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth2-on-behalf-of-flow

## Standards

[JSON Web Token (JWT)](https://datatracker.ietf.org/doc/html/rfc7519)

[Best Current Practice for OAuth 2.0 Security](https://datatracker.ietf.org/doc/rfc9700/)

[The OAuth 2.0 Authorization Framework](https://datatracker.ietf.org/doc/html/rfc6749)

[OAuth 2.0 Demonstrating Proof of Possession DPoP](https://datatracker.ietf.org/doc/html/rfc9449)

[OAuth 2.0 JWT-Secured Authorization Request (JAR) RFC 9101](https://datatracker.ietf.org/doc/rfc9101/)

[OAuth 2.0 Mutual-TLS Client Authentication and Certificate-Bound Access Tokens](https://datatracker.ietf.org/doc/html/rfc8705)

[OpenID Connect 1.0](https://openid.net/specs/openid-connect-core-1_0-final.html)

[Microsoft identity platform and OAuth 2.0 On-Behalf-Of flow](/azure/active-directory/develop/v2-oauth2-on-behalf-of-flow)

[OAuth 2.0 Token Exchange](https://datatracker.ietf.org/doc/html/rfc8693)

[JSON Web Token (JWT) Profile for OAuth 2.0 Access Tokens](https://datatracker.ietf.org/doc/html/rfc9068)

[HTTP Semantics RFC 9110](https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.2)
