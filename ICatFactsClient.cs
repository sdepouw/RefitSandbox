using Refit;

namespace RefitSandbox;

public interface ICatFactsClient
{
  [Post("/oauth")]
  public Task<ApiResponse<string?>> GetBearerTokenAsync(OAuthRequest request, CancellationToken cancellationToken);

  [Get("/facts")]
  [Headers("Authorization: Bearer")] // Adding the header to each request for facts
  Task<ApiResponse<List<CatFact>>> GetTheFactsAsync(CancellationToken cancellationToken);
}
