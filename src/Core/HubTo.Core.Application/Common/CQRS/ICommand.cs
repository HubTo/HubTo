using Mediator;

namespace HubTo.Core.Application.Common.CQRS;

public interface ICommandBase : IMessage { }

public interface ICommand<TResult> : IRequest<Result<TResult>>, ICommandBase { }

public interface ICommand : IRequest<Unit>, ICommandBase { }

