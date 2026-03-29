using HubTo.Core.Application.Common.Results;
using Mediator;

namespace HubTo.Core.Application.Common.CQRS;

public interface ICommandHandler<TCommand, TResult> : IRequestHandler<TCommand, Result<TResult>> where TCommand : ICommand<TResult> { }

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Unit> where TCommand : ICommand { }
