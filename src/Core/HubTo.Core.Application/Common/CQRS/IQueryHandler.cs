using Mediator;

namespace HubTo.Core.Application.Common.CQRS;

public interface IQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, Result<TResult>> where TQuery : IQuery<TResult> { }
