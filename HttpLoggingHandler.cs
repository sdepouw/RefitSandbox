namespace RefitSandbox;

public class HttpLoggingHandler(ILogger<HttpLoggingHandler> logger) : DelegatingHandler
{
  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    Guid id = Guid.NewGuid();
    HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    logger.LogInformation("[{Id}] Request: {Request}", id, request);
    logger.LogInformation("[{Id}] Response: {Response}", id, response);
    return response;
  }
}
