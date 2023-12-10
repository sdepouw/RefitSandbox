using Refit;

namespace RefitSandbox;

public class Worker(ILogger<Worker> logger, ICatFactsClient catFactsClient)
  : BackgroundService
{
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    try
    {
      await UseCatFactsClientAsync(stoppingToken);
    }
    catch (Exception e)
    {
      logger.LogError(e, "Exception thrown when using Refit client");
    }
  }

  private async Task UseCatFactsClientAsync(CancellationToken stoppingToken)
  {
    ApiResponse<List<CatFact>> catFactsResponse = await catFactsClient.GetTheFactsAsync(stoppingToken);
    await catFactsResponse.EnsureSuccessStatusCodeAsync();
    List<CatFact>? catFacts = catFactsResponse.Content;
    if (catFacts is null)
    {
      logger.LogWarning("CatFacts request returned null");
      return;
    }

    logger.LogInformation("Found Cat Facts!");
    foreach (CatFact fact in catFacts.Take(3))
    {
      logger.LogInformation("Fact created at {CreatedAt}: {FactText}", fact.CreatedAt.ToString("yyyy-MM-dd"), fact.Text);
    }
  }
}
