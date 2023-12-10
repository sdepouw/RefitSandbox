using Refit;

namespace RefitSandbox;

public interface IExampleRefitClient
{
  [Get("/path/to/endpoint?id={id}")]
  public Task<ApiResponse<ExampleResponse>> ExampleApiResponseAsync(int id, CancellationToken cancellationToken);

  [Post("/path/to/create")]
  public Task<ExampleResponse?> ExamplePostEndpointAsync(ExampleCreateRequestBody request, CancellationToken cancellationToken);
}
