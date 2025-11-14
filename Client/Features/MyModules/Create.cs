// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ICTAce.FileHub.Client.Features.MyModules;

public class CreateMyModuleRequest : IRequest<int>
{
    public int ModuleId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateMyModuleValidator : AbstractValidator<CreateMyModuleRequest>
{
    public CreateMyModuleValidator()
    {
        // ModuleId validation
        RuleFor(x => x.ModuleId)
            .GreaterThan(0)
            .WithMessage("ModuleId must be greater than 0");

        // Name validation
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(1, 100)
            .WithMessage("Name must be between 1 and 100 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$")
            .WithMessage("Name can only contain letters, numbers, spaces, hyphens, and underscores");

        // Example: Async validation (e.g., check if name already exists)
        // RuleFor(x => x.Name)
        //     .MustAsync(async (name, cancellation) => await IsNameUnique(name))
        //     .WithMessage("A module with this name already exists");
    }

    // Example async validation method
    // private async Task<bool> IsNameUnique(string name)
    // {
    //     // Call service to check database
    //     return true;
    // }
}
