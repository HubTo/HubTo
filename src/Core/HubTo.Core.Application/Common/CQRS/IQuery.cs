using HubTo.Core.Application.Common.Results;
using Mediator;

namespace HubTo.Core.Application.Common.CQRS;

public interface IQuery<TResult> : IRequest<Result<TResult>> { }