// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Tests.Mocks;

public class MockMyModuleService : ISampleModuleService
{
    private readonly List<GetSampleModuleDto> _modules = new();
    private int _nextId = 1;

    public MockMyModuleService()
    {
        _modules.Add(new GetSampleModuleDto
        {
            Id = 1,
            ModuleId = 1,
            Name = "Test Module 1",
            CreatedBy = "Test User",
            CreatedOn = DateTime.Now.AddDays(-10),
            ModifiedBy = "Test User",
            ModifiedOn = DateTime.Now.AddDays(-5)
        });

        _modules.Add(new GetSampleModuleDto
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

    public Task<GetSampleModuleDto> GetAsync(int id, int moduleId)
    {
        var module = _modules.FirstOrDefault(m => m.Id == id && m.ModuleId == moduleId);
        if (module == null)
        {
            throw new InvalidOperationException($"Module with Id {id} and ModuleId {moduleId} not found");
        }
        return Task.FromResult(module);
    }

    public Task<PagedResult<ListSampleModuleDto>> ListAsync(int moduleId, int pageNumber = 1, int pageSize = 10)
    {
        var items = _modules
            .Where(m => m.ModuleId == moduleId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new ListSampleModuleDto
            {
                Id = m.Id,
                Name = m.Name
            })
            .ToList();

        var totalCount = _modules.Count(m => m.ModuleId == moduleId);

        var pagedResult = new PagedResult<ListSampleModuleDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return Task.FromResult(pagedResult);
    }

    public Task<int> CreateAsync(int moduleId, CreateAndUpdateSampleModuleDto dto)
    {
        var newModule = new GetSampleModuleDto
        {
            Id = _nextId++,
            ModuleId = moduleId,
            Name = dto.Name,
            CreatedBy = "Test User",
            CreatedOn = DateTime.Now,
            ModifiedBy = "Test User",
            ModifiedOn = DateTime.Now
        };

        _modules.Add(newModule);
        return Task.FromResult(newModule.Id);
    }

    public Task<int> UpdateAsync(int id, int moduleId, CreateAndUpdateSampleModuleDto dto)
    {
        var module = _modules.FirstOrDefault(m => m.Id == id && m.ModuleId == moduleId);
        if (module == null)
        {
            throw new InvalidOperationException($"Module with Id {id} and ModuleId {moduleId} not found");
        }

        module.Name = dto.Name;
        module.ModifiedBy = "Test User";
        module.ModifiedOn = DateTime.Now;

        return Task.FromResult(module.Id);
    }

    public Task DeleteAsync(int id, int moduleId)
    {
        var module = _modules.FirstOrDefault(m => m.Id == id && m.ModuleId == moduleId);
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

    public void AddTestData(GetSampleModuleDto module)
    {
        _modules.Add(module);
    }

    public int GetModuleCount()
    {
        return _modules.Count;
    }
}
