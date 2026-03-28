using HubTo.Core.Application.Common.CQRS;
using HubTo.Core.Application.Contracts.Persistence;
using Mediator;
using Microsoft.Extensions.Logging;

namespace HubTo.Core.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IMessage
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        if (message is not ICommandBase)
        {
            return await next(message, cancellationToken);
        }

        var requestName = typeof(TRequest).Name;

        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async (ct) =>
            {
                _logger.LogInformation("--- Begin transaction for {RequestName}", requestName);

                var response = await next(message, ct);

                _logger.LogInformation("--- Commit transaction for {RequestName}", requestName);

                return response;
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "--- Rollback transaction executed for {RequestName}", requestName);
            throw;
        }
    }
}