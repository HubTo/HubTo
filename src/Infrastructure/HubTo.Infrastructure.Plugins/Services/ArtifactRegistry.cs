using HubTo.Abstraction.Models;
using HubTo.Abstraction.Models.Domain;
using HubTo.Abstraction.Registrars;
using HubTo.Core.Application.Contracts.Persistence.Repositories;
using HubTo.Core.Domain.Entities;

namespace HubTo.Infrastructure.Plugins.Services;

public sealed class ArtifactRegistry : IArtifactRegistry
{
    private readonly IArtifactRepository _artifactRepository;
    private readonly IArtifactVersionRepository _artifactVersionRepository;
    private readonly IStoragePlugin _storagePlugin;

    public string BaseUrl { get; }

    public ArtifactRegistry(
        IArtifactRepository artifactRepository,
        IArtifactVersionRepository artifactVersionRepository,
        IStoragePlugin storagePlugin,
        string baseUrl)
    {
        _artifactRepository = artifactRepository;
        _artifactVersionRepository = artifactVersionRepository;
        _storagePlugin = storagePlugin;
        BaseUrl = baseUrl;
    }

    public async Task<ArtifactInfo?> GetArtifactAsync(Guid namespaceId, string artifactName, CancellationToken cancellationToken = default)
    {
        var entity = await _artifactRepository.GetByPackageAsync(namespaceId, artifactName, cancellationToken);
        if (entity is null) return null;

        return MapToArtifactInfo(entity);
    }

    public async Task<IEnumerable<ArtifactInfo>> GetArtifactsAsync(Guid namespaceId, CancellationToken cancellationToken = default)
    {
        var entities = await _artifactRepository.WhereAsync(x => x.NamespaceId == namespaceId, cancellationToken);
        return entities.Select(MapToArtifactInfo);
    }

    public async Task<IEnumerable<ArtifactInfo>> SearchAsync(Guid namespaceId, string query, int skip = 0, int take = 20, CancellationToken cancellationToken = default)
    {
        var entities = await _artifactRepository.WhereAsync(
            x => x.NamespaceId == namespaceId && x.Name.Contains(query),
            cancellationToken);

        return entities.Skip(skip).Take(take).Select(MapToArtifactInfo);
    }

    public async Task<ArtifactInfo> EnsureArtifactAsync(Guid namespaceId, string artifactName, string registrarType, CancellationToken cancellationToken = default)
    {
        var entity = await _artifactRepository.GetByPackageAsync(namespaceId, artifactName, cancellationToken);

        if (entity is not null)
            return MapToArtifactInfo(entity);

        var newEntity = new ArtifactEntity
        {
            Id = Guid.NewGuid(),
            Name = artifactName,
            RegistrarType = registrarType,
            NamespaceId = namespaceId
        };

        await _artifactRepository.AddAsync(newEntity, cancellationToken);
        return MapToArtifactInfo(newEntity);
    }

    public async Task<IEnumerable<ArtifactVersionInfo>> GetVersionsAsync(Guid artifactId, CancellationToken cancellationToken = default)
    {
        var versions = await _artifactVersionRepository.GetPackageVersionsByArtifactIdAsync(artifactId, cancellationToken);
        return versions.Select(MapToArtifactVersionInfo);
    }

    public async Task<ArtifactVersionInfo?> GetVersionAsync(Guid artifactId, string version, CancellationToken cancellationToken = default)
    {
        var entity = await _artifactVersionRepository.FindAsync(
            x => x.ArtifactId == artifactId && x.Version == version,
            cancellationToken);

        return entity is null ? null : MapToArtifactVersionInfo(entity);
    }

    public async Task<ArtifactVersionInfo> AddVersionAsync(Guid artifactId, Stream stream, ArtifactVersionInfo versionInfo, CancellationToken cancellationToken = default)
    {
        var fileName = $"{artifactId}/{versionInfo.Version}";

        var storageResult = await _storagePlugin.SaveAsync(
            stream,
            fileName,
            cancellationToken: cancellationToken);

        if (!storageResult.IsSuccess)
        {
            var error = storageResult.Errors.FirstOrDefault() ?? "Storage error";
            throw new InvalidOperationException(error);
        }

        var storagePath = storageResult.Value;

        var entity = new ArtifactVersionEntity
        {
            Id = Guid.NewGuid(),
            ArtifactId = artifactId,
            Version = versionInfo.Version,
            StoragePath = storagePath,
            Digest = versionInfo.Digest,
            MediaType = versionInfo.MediaType,
            SizeInBytes = versionInfo.SizeInBytes,
            IsListed = true,
            PublishedAt = DateTime.UtcNow
        };

        await _artifactVersionRepository.AddAsync(entity, cancellationToken);

        return MapToArtifactVersionInfo(entity);
    }

    public async Task<Stream> GetVersionStreamAsync(Guid artifactId, string version, CancellationToken cancellationToken = default)
    {
        var entity = await _artifactVersionRepository.FindAsync(
        x => x.ArtifactId == artifactId && x.Version == version,
        cancellationToken);

        if (entity is null)
            return Stream.Null;

        var result = await _storagePlugin.GetAsync(entity.StoragePath, cancellationToken);

        if (!result.IsSuccess)
            return Stream.Null;

        return result.Value;
    }

    public async Task UnlistVersionAsync(Guid artifactId, string version, CancellationToken cancellationToken = default)
    {
        var entity = await _artifactVersionRepository.FindAsync(
            x => x.ArtifactId == artifactId && x.Version == version,
            cancellationToken);

        if (entity is null) return;

        entity.IsListed = false;
        await _artifactVersionRepository.UpdateAsync(entity, cancellationToken);
    }

    private static ArtifactInfo MapToArtifactInfo(ArtifactEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        RegistrarType = entity.RegistrarType,
        NamespaceId = entity.NamespaceId
    };

    private static ArtifactVersionInfo MapToArtifactVersionInfo(ArtifactVersionEntity entity) => new()
    {
        Id = entity.Id,
        Version = entity.Version,
        StoragePath = entity.StoragePath,
        Digest = entity.Digest,
        MediaType = entity.MediaType,
        SizeInBytes = entity.SizeInBytes,
        IsListed = entity.IsListed,
        PublishedAt = entity.PublishedAt
    };
}

