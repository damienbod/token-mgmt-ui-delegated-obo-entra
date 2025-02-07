using IdentityModel.Client;
using IdentityModel;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace WebApiEntraIdObo.WebApiEntraId;

public class ApiTokenCacheClient
{
    private readonly ILogger<ApiTokenCacheClient> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<WebApiDownstreamConfig> _webApiDuendeConfig;

    private static readonly object _lock = new();
    private readonly IDistributedCache _cache;

    private const int cacheExpirationInDays = 1;

    private class AccessTokenItem
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpiresIn { get; set; }
    }

    public ApiTokenCacheClient(
        IOptions<WebApiDownstreamConfig> webApiDuendeConfig,
        IHttpClientFactory httpClientFactory,
        ILoggerFactory loggerFactory,
        IDistributedCache cache)
    {
        _webApiDuendeConfig = webApiDuendeConfig;
        _httpClientFactory = httpClientFactory;
        _logger = loggerFactory.CreateLogger<ApiTokenCacheClient>();
        _cache = cache;
    }

    public async Task<string> GetApiTokenOauthGrantTokenExchange(
        string clientId, 
        string audience,
        string scope, 
        string clientSecret, 
        string aadAccessToken)
    {
        var accessToken = GetFromCache(clientId);

        if (accessToken != null)
        {
            if (accessToken.ExpiresIn > DateTime.UtcNow)
            {
                return accessToken.AccessToken;
            }
        }

        _logger.LogDebug("GetApiToken new from STS for {api_name}", clientId);

        // add
        var newAccessToken = await GetApiTokenOauthGrantTokenExchangeAad(
            clientId, audience, scope, clientSecret, aadAccessToken);

        AddToCache(clientId, newAccessToken);

        return newAccessToken.AccessToken;
    }

    private async Task<AccessTokenItem> GetApiTokenOauthGrantTokenExchangeAad(string clientId,
        string audience,
        string scope,
        string clientSecret,
        string entraIdAccessToken)
    {
        var tokenExchangeHttpClient = _httpClientFactory.CreateClient();
        tokenExchangeHttpClient.BaseAddress = new Uri(_webApiDuendeConfig.Value.IdentityProviderUrl);

        var cache = new DiscoveryCache(_webApiDuendeConfig.Value.IdentityProviderUrl);
        var disco = await cache.GetAsync();

        var tokenExchangeSuccessResponse = await tokenExchangeHttpClient
            .RequestTokenExchangeTokenAsync(new TokenExchangeTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = clientId,
            ClientSecret = clientSecret,

            Audience = audience,
            SubjectToken = entraIdAccessToken,
            SubjectTokenType = OidcConstants.TokenTypeIdentifiers.AccessToken,
            Scope = scope, 

            Parameters =
            {
                { "exchange_style", "delegation" }
            }
        });

        if (tokenExchangeSuccessResponse != null)
        {
            return new AccessTokenItem
            {
                ExpiresIn = DateTime.UtcNow.AddSeconds(tokenExchangeSuccessResponse.ExpiresIn),
                AccessToken = tokenExchangeSuccessResponse.AccessToken!
            };
        }

        _logger.LogError("no success response from oauth token exchange access token request");
        throw new ApplicationException("no success response from oauth token exchange access token request");
    }

    private void AddToCache(string key, AccessTokenItem accessTokenItem)
    {
        var options = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromDays(cacheExpirationInDays));

        lock (_lock)
        {
            _cache.SetString(key, JsonSerializer.Serialize(accessTokenItem), options);
        }
    }

    private AccessTokenItem? GetFromCache(string key)
    {
        var item = _cache.GetString(key);
        if (item != null)
        {
            return JsonSerializer.Deserialize<AccessTokenItem>(item);
        }

        return null;
    }
}