// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Tests.Mocks;

public class MockCategoryService : ICategoryService
{
    private readonly List<GetCategoryDto> _categories = [];
    private int _nextId = 1;

    public MockCategoryService()
    {
        _categories.Add(new GetCategoryDto
        {
            Id = 1,
            ModuleId = 1,
            Name = "Test Category 1",
            ViewOrder = 0,
            ParentId = null,
            CreatedBy = "Test User",
            CreatedOn = DateTime.Now.AddDays(-10),
            ModifiedBy = "Test User",
            ModifiedOn = DateTime.Now.AddDays(-5)
        });

        _categories.Add(new GetCategoryDto
        {
            Id = 2,
            ModuleId = 1,
            Name = "Test Category 2",
            ViewOrder = 1,
            ParentId = null,
            CreatedBy = "Test User",
            CreatedOn = DateTime.Now.AddDays(-8),
            ModifiedBy = "Test User",
            ModifiedOn = DateTime.Now.AddDays(-3)
        });

        _categories.Add(new GetCategoryDto
        {
            Id = 3,
            ModuleId = 1,
            Name = "Test Category 1.1",
            ViewOrder = 0,
            ParentId = 1,
            CreatedBy = "Test User",
            CreatedOn = DateTime.Now.AddDays(-7),
            ModifiedBy = "Test User",
            ModifiedOn = DateTime.Now.AddDays(-2)
        });

        _nextId = 4;
    }

    public Task<GetCategoryDto> GetAsync(int id, int moduleId)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id && c.ModuleId == moduleId);
        if (category == null)
        {
            throw new InvalidOperationException($"Category with Id {id} and ModuleId {moduleId} not found");
        }
        return Task.FromResult(category);
    }

    public Task<PagedResult<ListCategoryDto>> ListAsync(int moduleId, int pageNumber = 1, int pageSize = 10)
    {
        var items = _categories
            .Where(c => c.ModuleId == moduleId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new ListCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                ViewOrder = c.ViewOrder,
                ParentId = c.ParentId,
                Children = []
            })
            .ToList();

        var totalCount = _categories.Count(c => c.ModuleId == moduleId);

        var pagedResult = new PagedResult<ListCategoryDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return Task.FromResult(pagedResult);
    }

    public Task<int> CreateAsync(int moduleId, CreateAndUpdateCategoryDto dto)
    {
        var newCategory = new GetCategoryDto
        {
            Id = _nextId++,
            ModuleId = moduleId,
            Name = dto.Name,
            ViewOrder = dto.ViewOrder,
            ParentId = dto.ParentId,
            CreatedBy = "Test User",
            CreatedOn = DateTime.Now,
            ModifiedBy = "Test User",
            ModifiedOn = DateTime.Now
        };

        _categories.Add(newCategory);
        return Task.FromResult(newCategory.Id);
    }

    public Task<int> UpdateAsync(int id, int moduleId, CreateAndUpdateCategoryDto dto)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id && c.ModuleId == moduleId);
        if (category == null)
        {
            throw new InvalidOperationException($"Category with Id {id} and ModuleId {moduleId} not found");
        }

        category.Name = dto.Name;
        category.ViewOrder = dto.ViewOrder;
        category.ParentId = dto.ParentId;
        category.ModifiedBy = "Test User";
        category.ModifiedOn = DateTime.Now;

        return Task.FromResult(category.Id);
    }

    public Task DeleteAsync(int id, int moduleId)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id && c.ModuleId == moduleId);
        if (category != null)
        {
            _categories.Remove(category);
        }
        return Task.CompletedTask;
    }

    public Task<int> MoveUpAsync(int id, int moduleId)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id && c.ModuleId == moduleId);
        if (category == null)
        {
            return Task.FromResult(-1);
        }

        var siblings = _categories
            .Where(c => c.ModuleId == moduleId && c.ParentId == category.ParentId)
            .OrderBy(c => c.ViewOrder)
            .ToList();

        var currentIndex = siblings.IndexOf(category);
        if (currentIndex > 0)
        {
            var previous = siblings[currentIndex - 1];
            (category.ViewOrder, previous.ViewOrder) = (previous.ViewOrder, category.ViewOrder);
        }

        return Task.FromResult(category.Id);
    }

    public Task<int> MoveDownAsync(int id, int moduleId)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id && c.ModuleId == moduleId);
        if (category == null)
        {
            return Task.FromResult(-1);
        }

        var siblings = _categories
            .Where(c => c.ModuleId == moduleId && c.ParentId == category.ParentId)
            .OrderBy(c => c.ViewOrder)
            .ToList();

        var currentIndex = siblings.IndexOf(category);
        if (currentIndex >= 0 && currentIndex < siblings.Count - 1)
        {
            var next = siblings[currentIndex + 1];
            (category.ViewOrder, next.ViewOrder) = (next.ViewOrder, category.ViewOrder);
        }

        return Task.FromResult(category.Id);
    }

    public void ClearData()
    {
        _categories.Clear();
        _nextId = 1;
    }

    public void AddTestData(GetCategoryDto category)
    {
        _categories.Add(category);
    }

    public int GetCategoryCount()
    {
        return _categories.Count;
    }

    public List<GetCategoryDto> GetAllCategories()
    {
        return _categories;
    }
}
