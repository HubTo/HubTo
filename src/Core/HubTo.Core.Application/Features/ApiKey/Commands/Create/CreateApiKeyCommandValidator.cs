using FluentValidation;

namespace HubTo.Core.Application.Features.ApiKey.Commands.Create;

internal sealed class CreateApiKeyCommandValidator : AbstractValidator<CreateApiKeyCommand>
{
    public CreateApiKeyCommandValidator()
    {
        RuleFor(x => x.Label)
            .NotEmpty().WithMessage("Label is required.")
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Label can only contain letters, numbers, underscores, and hyphens.");

        RuleFor(x => x.NamespaceId)
            .NotEmpty().WithMessage("Namespace is required.");

        RuleFor(x => x.Permission)
            .NotEmpty().WithMessage("Permission is required.");
    }
}
