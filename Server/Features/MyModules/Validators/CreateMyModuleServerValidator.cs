using ICTAce.FileHub.Client.Features.MyModules;

namespace ICTAce.FileHub.Features.MyModules.Validators;

/// <summary>
/// Server-side validator for CreateMyModuleRequest
/// Can include database checks and complex business rules
/// </summary>
public class CreateMyModuleServerValidator : AbstractValidator<CreateMyModuleRequest>
{
    private readonly IDbContextFactory<Context> _contextFactory;

    public CreateMyModuleServerValidator(IDbContextFactory<Context> contextFactory)
    {
        _contextFactory = contextFactory;

        // Basic validations
        RuleFor(x => x.ModuleId)
            .GreaterThan(0)
            .WithMessage("ModuleId must be greater than 0");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(1, 100)
            .WithMessage("Name must be between 1 and 100 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$")
            .WithMessage("Name can only contain letters, numbers, spaces, hyphens, and underscores");

        // Database validation: Check if name already exists in module
        RuleFor(x => x)
            .MustAsync(async (request, cancellation) => 
                await IsNameUniqueInModule(request.Name, request.ModuleId, cancellation))
            .WithMessage("A module with this name already exists in this module")
            .WithName("Name");
    }

    private async Task<bool> IsNameUniqueInModule(string name, int moduleId, CancellationToken cancellationToken)
    {
        using var db = _contextFactory.CreateDbContext();
        return !await db.MyModule
            .AnyAsync(m => m.ModuleId == moduleId && m.Name == name, cancellationToken);
    }

}
