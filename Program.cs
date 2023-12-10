using Microsoft.Extensions.Options;
using Refit;
using RefitSandbox;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddRefitClient<IExampleRefitClient>() // Returns `IHttpClientBuilder`
  .ConfigureHttpClient(httpClient => httpClient.BaseAddress = new Uri("https://example.com/"));

RefitSettings refitSettings = new()
{
  // Tell Refit to use AuthBearerTokenFactory.GetBearerTokenAsync() whenever it needs an OAuth token string.
  // This is a lambda, so it won't be called until a request is made.
  AuthorizationHeaderValueGetter = (_, cancellationToken) => AuthBearerTokenFactory.GetBearerTokenAsync(cancellationToken)
};

IHttpClientBuilder refitClientBuilder = builder.Services.AddRefitClient<ICatFactsClient>(refitSettings) // Now passing refitSettings to Refit
  .ConfigureHttpClient(httpClient =>
  {
    httpClient.BaseAddress = new Uri("https://cat-fact.herokuapp.com/");
    // Will throw `TaskCanceledException` if the request goes longer than 3 seconds.
    httpClient.Timeout = TimeSpan.FromSeconds(3);
  });

// Adding our new handler here
refitClientBuilder.AddHttpMessageHandler(serviceProvider
  => new HttpLoggingHandler(serviceProvider.GetRequiredService<ILogger<HttpLoggingHandler>>()));
refitClientBuilder.Services.AddSingleton<HttpLoggingHandler>();

var host = builder.Build();
// Define what gets called when Refit requires an OAuth token string
AuthBearerTokenFactory.SetBearerTokenGetterFunc(cancellationToken =>
{
  // Get our application settings and an instance of ICatFactsClient via the host's ServiceProvider.
  RefitSandboxSettings settings = host.Services.GetRequiredService<IOptions<RefitSandboxSettings>>().Value;
  ICatFactsClient client = host.Services.GetRequiredService<ICatFactsClient>();
  return GetTheToken(client, settings, cancellationToken);
});
host.Run();
return;

// Could wrap this in its own dependency, e.g. ICatFactsClientService,
// but for brevity we'll just define a local function here in Program.cs
async Task<string> GetTheToken(ICatFactsClient client, RefitSandboxSettings settings, CancellationToken cancellationToken)
{
  // In reality we'd log this error, but for our demo we'll return a fake token, since
  // our OAuth endpoint is always going to 404.
  const string defaultToken = "default-token-value";
  try
  {
    OAuthRequest request = new() { ClientId = settings.ClientId, ClientSecret = settings.ClientSecret };
    ApiResponse<string?> result = await client.GetBearerTokenAsync(request, cancellationToken);
    // Taking advantage of ApiResponse<T> by checking the status before returning
    await result.EnsureSuccessStatusCodeAsync();
    // Any caching or other logic regarding the full token would be implemented here.
    return result.Content ?? defaultToken;
  }
  catch (Exception)
  {
    return defaultToken;
  }
}
