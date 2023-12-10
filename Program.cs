using Refit;
using RefitSandbox;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddRefitClient<IExampleRefitClient>() // Returns `IHttpClientBuilder`
  .ConfigureHttpClient(httpClient => httpClient.BaseAddress = new Uri("https://example.com/"));
IHttpClientBuilder refitClientBuilder = builder.Services.AddRefitClient<ICatFactsClient>()
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
host.Run();
