using Microsoft.Identity.Web;
using System.Net.Http.Headers;
using System.Text.Json;

namespace WebApiEntraIdObo.WebApiEntraId;

public class WebApiDownstreamService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IConfiguration _configuration;

    public WebApiDownstreamService(IHttpClientFactory clientFactory,
        ITokenAcquisition tokenAcquisition,
        IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _tokenAcquisition = tokenAcquisition;
        _configuration = configuration;
    }

    public async Task<string?> GetApiDataAsync()
    {
        var client = _clientFactory.CreateClient();

        // user_impersonation access_as_user access_as_application .default
        var scope = _configuration["WebApiEntraIdObo:ScopeForAccessToken"];
        if (scope == null) throw new ArgumentNullException(nameof(scope));

        var uri = _configuration["WebApiEntraIdObo:ApiBaseAddress"];
        if (uri == null) throw new ArgumentNullException(nameof(uri));

        var accessToken = await _tokenAcquisition
            .GetAccessTokenForUserAsync([scope]);

        client.DefaultRequestHeaders.Authorization
            = new AuthenticationHeaderValue("Bearer", accessToken);

        client.BaseAddress = new Uri(uri);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync("api/profiles/photo");
        if (response.IsSuccessStatusCode)
        {
            var data = await JsonSerializer.DeserializeAsync<string>(
                await response.Content.ReadAsStreamAsync());

            return data;
        }

        throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
    }
}