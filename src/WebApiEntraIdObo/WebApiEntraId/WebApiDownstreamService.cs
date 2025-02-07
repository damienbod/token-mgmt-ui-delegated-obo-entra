using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace WebApiEntraIdObo.WebApiEntraId;

public class WebApiDownstreamService
{
    private readonly IOptions<WebApiDownstreamConfig> _webApiDuendeConfig;
    private readonly IHttpClientFactory _clientFactory;
    private readonly ApiTokenCacheClient _apiTokenClient;

    public WebApiDownstreamService(
        IOptions<WebApiDownstreamConfig> webApiDuendeConfig,
        IHttpClientFactory clientFactory,
        ApiTokenCacheClient apiTokenClient)
    {
        _webApiDuendeConfig = webApiDuendeConfig;
        _clientFactory = clientFactory;
        _apiTokenClient = apiTokenClient;
    }

    public async Task<string> GetWebApiDuendeDataAsync(string entraIdAccessToken)
    {
        try
        {
            var client = _clientFactory.CreateClient();

            client.BaseAddress = new Uri(_webApiDuendeConfig.Value.ApiBaseAddress);

            var accessToken = await _apiTokenClient.GetApiTokenOauthGrantTokenExchange
            (
                _webApiDuendeConfig.Value.ClientId,
                _webApiDuendeConfig.Value.Audience,
                _webApiDuendeConfig.Value.ScopeForAccessToken,
                _webApiDuendeConfig.Value.ClientSecret,
                entraIdAccessToken
            );

            client.SetBearerToken(accessToken);

            var response = await client.GetAsync("api/profiles/photo");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();

                if (data != null)
                {
                    return data;
                }

                return string.Empty;
            }

            throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
        }
        catch (Exception e)
        {
            throw new ApplicationException($"Exception {e}");
        }
    }
}