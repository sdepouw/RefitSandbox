using Refit;

namespace RefitSandbox;

public interface ICatFactsClient
{
    [Get("/facts")]
    Task<ApiResponse<List<CatFact>>> GetTheFactsAsync(CancellationToken cancellationToken);
}