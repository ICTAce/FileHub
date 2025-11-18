// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Tests.Mocks;

public class MockMyModuleService : IMyModuleService
{
    private readonly List<GetMyModuleResponse> _modules = new();
    private int _nextId = 1;

    public MockMyModuleService()
    {
        _modules.Add(new GetMyModuleResponse
        {
            Id = 1,
            ModuleId = 1,
            Name = "Test Module 1",
            CreatedBy = "Test User",
            CreatedOn = DateTime.Now.AddDays(-10),
            ModifiedBy = "Test User",
            ModifiedOn = DateTime.Now.AddDays(-5)
        });

        _modules.Add(new GetMyModuleResponse
        {
            Id = 2,
            ModuleId = 1,
            Name = "Test Module 2",
            CreatedBy = "Test User",
            CreatedOn = DateTime.Now.AddDays(-8),
            ModifiedBy = "Test User",
            ModifiedOn = DateTime.Now.AddDays(-3)
        });

        _nextId = 3;
    }

    public Task<GetMyModuleResponse> GetAsync(GetMyModuleRequest request)
    {
        var module = _modules.FirstOrDefault(m => m.Id == request.Id);
        if (module == null)
        {
            throw new InvalidOperationException($"Module with Id {request.Id} not found");
        }
        return Task.FromResult(module);
    }

    public Task<PagedResult<ListMyModulesResponse>> ListAsync(ListMyModulesRequest request)
    {
        var items = _modules
            .Where(m => m.ModuleId == request.ModuleId)
            .Select(m => new ListMyModulesResponse
            {
                Id = m.Id,
                Name = m.Name
            })
            .ToList();

        var pagedResult = new PagedResult<ListMyModulesResponse>
        {
            Items = items,
            TotalCount = items.Count,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        return Task.FromResult(pagedResult);
    }

    public Task<int> CreateAsync(CreateMyModuleRequest request)
    {
        var newModule = new GetMyModuleResponse
        {
            Id = _nextId++,
            ModuleId = request.ModuleId,
            Name = request.Name,
            CreatedBy = "Test User",
            CreatedOn = DateTime.Now,
            ModifiedBy = "Test User",
            ModifiedOn = DateTime.Now
        };

        _modules.Add(newModule);
        return Task.FromResult(newModule.Id);
    }

    public Task<int> UpdateAsync(UpdateMyModuleRequest request)
    {
        var module = _modules.FirstOrDefault(m => m.Id == request.Id);
        if (module == null)
        {
            throw new InvalidOperationException($"Module with Id {request.Id} not found");
        }

        module.Name = request.Name;
        module.ModifiedBy = "Test User";
        module.ModifiedOn = DateTime.Now;

        return Task.FromResult(module.Id);
    }

    public Task DeleteAsync(DeleteMyModuleRequest request)
    {
        var module = _modules.FirstOrDefault(m => m.Id == request.Id);
        if (module != null)
        {
            _modules.Remove(module);
        }
        return Task.CompletedTask;
    }

    public void ClearData()
    {
        _modules.Clear();
        _nextId = 1;
    }

    public void AddTestData(GetMyModuleResponse module)
    {
        _modules.Add(module);
    }

    public int GetModuleCount()
    {
        return _modules.Count;
    }
}
