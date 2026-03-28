namespace HubTo.Core.Application.Contracts;

public interface IClientContext
{
    Guid? UserId { get; }
    string TraceId { get; }
}