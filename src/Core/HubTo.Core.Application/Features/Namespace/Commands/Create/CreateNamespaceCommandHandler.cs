using HubTo.Core.Application.Common;
using HubTo.Core.Application.Common.CQRS;
using HubTo.Core.Application.Contracts.Persistence.Repositories;
using HubTo.Core.Domain.Entities;

namespace HubTo.Core.Application.Features.Namespace.Commands.Create;

internal sealed class CreateNamespaceCommandHandler : ICommandHandler<CreateNamespaceCommand, CreateNamespaceDto>
{
    private readonly INamespaceRepository _namespaceRepository;

    public CreateNamespaceCommandHandler(
        INamespaceRepository namespaceRepository)
    {
        _namespaceRepository = namespaceRepository;
    }

    public async ValueTask<Result<CreateNamespaceDto>> Handle(CreateNamespaceCommand request, CancellationToken cancellationToken)
    {
        var slug = request.Name.Trim().ToLowerInvariant();

        var nameExist = await _namespaceRepository.ExistsByNameAsync(request.Name, cancellationToken);
        if (nameExist)
            return Result<CreateNamespaceDto>.Fail(new List<string> { "Name is already exist." });

        var slugExist = await _namespaceRepository.ExistsBySlugAsync(slug, cancellationToken);
        if (slugExist)
            return Result<CreateNamespaceDto>.Fail(new List<string> { "Name is already exist." });

        NamespaceEntity entity = new NamespaceEntity()
        {
            Name = request.Name,
            Slug = slug,
            Description = request.Description
        };

        await _namespaceRepository.AddAsync(entity, cancellationToken);

        return Result<CreateNamespaceDto>.Ok(new CreateNamespaceDto(entity.Id));
    }
}