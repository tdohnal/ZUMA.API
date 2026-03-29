using MassTransit;

public class MessageService : IMessageService
{
    private readonly IBus _bus;

    public MessageService(IBus bus)
    {
        _bus = bus;
    }

    public async Task<BusinessResult<TSuccess, TFailure>> SendAsync<TRequest, TSuccess, TFailure>(
        TRequest message,
        CancellationToken ct = default)
        where TRequest : class
        where TSuccess : class
        where TFailure : class
    {
        var client = _bus.CreateRequestClient<TRequest>();

        var response = await client.GetResponse<TSuccess, TFailure>(message, ct);

        if (response.Is(out Response<TSuccess> success))
            return BusinessResult<TSuccess, TFailure>.Success(success.Message);

        if (response.Is(out Response<TFailure> failure))
            return BusinessResult<TSuccess, TFailure>.Failure(failure.Message);

        throw new InvalidOperationException("Bus returned unexpected response type.");
    }
}