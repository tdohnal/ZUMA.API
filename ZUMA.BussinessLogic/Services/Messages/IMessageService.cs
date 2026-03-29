public interface IMessageService
{
    Task<BusinessResult<TSuccess, TFailure>> SendAsync<TRequest, TSuccess, TFailure>(
        TRequest message,
        CancellationToken ct = default)
        where TRequest : class
        where TSuccess : class
        where TFailure : class;
}