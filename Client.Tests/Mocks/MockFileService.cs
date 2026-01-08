// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Tests.Mocks;

public class MockFileService : Services.IFileService
{
    private readonly List<GetFileDto> _files = [];
    private int _nextId = 1;

    public MockFileService()
    {
        _files.Add(new GetFileDto
        {
            Id = 1,
            ModuleId = 1,
            Name = "Test File 1",
            FileName = "test-file-1.pdf",
            ImageName = "test-image-1.png",
            Description = "Test file description 1",
            FileSize = "1.5 MB",
            Downloads = 10,
            CategoryIds = [1],
            CreatedBy = "Test User",
            CreatedOn = DateTime.Now.AddDays(-10),
            ModifiedBy = "Test User",
            ModifiedOn = DateTime.Now.AddDays(-5)
        });

        _files.Add(new GetFileDto
        {
            Id = 2,
            ModuleId = 1,
            Name = "Test File 2",
            FileName = "test-file-2.docx",
            ImageName = "test-image-2.jpg",
            Description = "Test file description 2",
            FileSize = "2.3 MB",
            Downloads = 25,
            CategoryIds = [1, 2],
            CreatedBy = "Test User",
            CreatedOn = DateTime.Now.AddDays(-8),
            ModifiedBy = "Test User",
            ModifiedOn = DateTime.Now.AddDays(-3)
        });

        _nextId = 3;
    }

    public Task<GetFileDto> GetAsync(int id, int moduleId)
    {
        var file = _files.FirstOrDefault(f => f.Id == id && f.ModuleId == moduleId);
        if (file == null)
        {
            throw new InvalidOperationException($"File with Id {id} and ModuleId {moduleId} not found");
        }
        return Task.FromResult(file);
    }

    public Task<PagedResult<ListFileDto>> ListAsync(int moduleId, int pageNumber = 1, int pageSize = 10)
    {
        var items = _files
            .Where(f => f.ModuleId == moduleId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new ListFileDto
            {
                Id = f.Id,
                Name = f.Name,
                FileName = f.FileName,
                ImageName = f.ImageName,
                Description = f.Description,
                FileSize = f.FileSize,
                Downloads = f.Downloads,
                CreatedOn = f.CreatedOn
            })
            .ToList();

        var totalCount = _files.Count(f => f.ModuleId == moduleId);

        var pagedResult = new PagedResult<ListFileDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return Task.FromResult(pagedResult);
    }

    public Task<int> CreateAsync(int moduleId, CreateAndUpdateFileDto dto)
    {
        var newFile = new GetFileDto
        {
            Id = _nextId++,
            ModuleId = moduleId,
            Name = dto.Name,
            FileName = dto.FileName,
            ImageName = dto.ImageName,
            Description = dto.Description,
            FileSize = dto.FileSize,
            Downloads = dto.Downloads,
            CategoryIds = dto.CategoryIds ?? [],
            CreatedBy = "Test User",
            CreatedOn = DateTime.Now,
            ModifiedBy = "Test User",
            ModifiedOn = DateTime.Now
        };

        _files.Add(newFile);
        return Task.FromResult(newFile.Id);
    }

    public Task<int> UpdateAsync(int id, int moduleId, CreateAndUpdateFileDto dto)
    {
        var file = _files.FirstOrDefault(f => f.Id == id && f.ModuleId == moduleId);
        if (file == null)
        {
            throw new InvalidOperationException($"File with Id {id} and ModuleId {moduleId} not found");
        }

        file.Name = dto.Name;
        file.FileName = dto.FileName;
        file.ImageName = dto.ImageName;
        file.Description = dto.Description;
        file.FileSize = dto.FileSize;
        file.Downloads = dto.Downloads;
        file.CategoryIds = dto.CategoryIds ?? [];
        file.ModifiedBy = "Test User";
        file.ModifiedOn = DateTime.Now;

        return Task.FromResult(file.Id);
    }

    public Task DeleteAsync(int id, int moduleId)
    {
        var file = _files.FirstOrDefault(f => f.Id == id && f.ModuleId == moduleId);
        if (file != null)
        {
            _files.Remove(file);
        }
        return Task.CompletedTask;
    }

    public Task<string> UploadFileAsync(int moduleId, Stream fileStream, string fileName)
    {
        // Simulate file upload by generating a unique filename
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        return Task.FromResult(uniqueFileName);
    }

    public void ClearData()
    {
        _files.Clear();
        _nextId = 1;
    }

    public void AddTestData(GetFileDto file)
    {
        _files.Add(file);
    }

    public int GetFileCount()
    {
        return _files.Count;
    }

    public List<GetFileDto> GetAllFiles()
    {
        return _files;
    }
}
