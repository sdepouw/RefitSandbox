namespace RefitSandbox;

public static class AuthBearerTokenFactory
{
  private static Func<CancellationToken, Task<string>>? _getBearerTokenAsyncFunc;

  /// <summary>
  /// Provide a delegate that returns a bearer token to use for authorization
  /// </summary>
  public static void SetBearerTokenGetterFunc(Func<CancellationToken, Task<string>> getBearerTokenAsyncFunc)
    => _getBearerTokenAsyncFunc = getBearerTokenAsyncFunc;

  public static Task<string> GetBearerTokenAsync(CancellationToken cancellationToken)
  {
    if (_getBearerTokenAsyncFunc is null) throw new InvalidOperationException("Must set Bearer Token Func before using it!");
    return _getBearerTokenAsyncFunc!(cancellationToken);
  }
}
