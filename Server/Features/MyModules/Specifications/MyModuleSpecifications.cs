using System.Linq.Expressions;
using ICTAce.FileHub.Features.Common.Specifications;

namespace ICTAce.FileHub.Features.MyModules.Specifications;

/// <summary>
/// Specification for MyModules belonging to a specific module
/// </summary>
public class MyModulesByModuleIdSpec : Specification<Entities.MyModule>
{
    private readonly int _moduleId;

    public MyModulesByModuleIdSpec(int moduleId)
    {
        _moduleId = moduleId;
    }

    public override Expression<Func<Entities.MyModule, bool>> ToExpression()
    {
        return myModule => myModule.ModuleId == _moduleId;
    }
}

/// <summary>
/// Specification for MyModules with name containing search term
/// </summary>
public class MyModulesByNameSpec : Specification<Entities.MyModule>
{
    private readonly string _searchTerm;

    public MyModulesByNameSpec(string searchTerm)
    {
        _searchTerm = searchTerm?.ToLower() ?? string.Empty;
    }

    public override Expression<Func<Entities.MyModule, bool>> ToExpression()
    {
        return myModule => myModule.Name.ToLower().Contains(_searchTerm);
    }
}

/// <summary>
/// Specification for active MyModules (example of business rule)
/// </summary>
public class ActiveMyModulesSpec : Specification<Entities.MyModule>
{
    public override Expression<Func<Entities.MyModule, bool>> ToExpression()
    {
        // Example: MyModules created within last 30 days are considered "active"
        var cutoffDate = DateTime.UtcNow.AddDays(-30);
        return myModule => myModule.CreatedOn >= cutoffDate;
    }
}
