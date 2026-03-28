using FluentValidation;

namespace HubTo.Core.Application.Features.Namespace.Commands.Create;

public sealed class CreateNamespaceCommandValidator : AbstractValidator<CreateNamespaceCommand>
{
    public CreateNamespaceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name is too short.")
            .MaximumLength(64).WithMessage("Name is too long.")
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Name can only contain letters, numbers, underscores, and hyphens.");

        RuleFor(x => x.Description)
            .MaximumLength(850).WithMessage("The description cannot exceed 850 characters.");
    }
}
