// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Files;

[Route("api/ictace/fileHub/files")]
[ApiController]
public class ICTAceFileHubFilesController(
    IMediator mediator,
    ILogManager logger,
    IHttpContextAccessor accessor,
    IWebHostEnvironment environment,
    ITenantManager tenantManager)
    : ModuleControllerBase(logger, accessor)
{
    private readonly IMediator _mediator = mediator;
    private readonly IWebHostEnvironment _environment = environment;
    private readonly ITenantManager _tenantManager = tenantManager;

    [HttpGet("{id}")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    [ProducesResponseType(typeof(GetFileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetFileDto>> GetAsync(
        int id,
        [FromQuery] int moduleId,
        CancellationToken cancellationToken = default)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleId))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized FileHub File Get Attempt Id={Id} in ModuleId={ModuleId}", id, moduleId);
            return Forbid();
        }

        if (id <= 0)
        {
            return BadRequest("Invalid File ID");
        }

        var query = new GetFileRequest
        {
            ModuleId = moduleId,
            Id = id,
        };

        var file = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

        if (file is null)
        {
            _logger.Log(LogLevel.Warning, this, LogFunction.Read,
                "File Not Found Id={Id} in ModuleId={ModuleId}", id, moduleId);
            return NotFound();
        }

        return Ok(file);
    }

    [HttpGet("")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    [ProducesResponseType(typeof(PagedResult<ListFileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<ListFileDto>>> ListAsync(
        [FromQuery] int moduleId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleId))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized File List Attempt ModuleId={ModuleId}", moduleId);
            return Forbid();
        }

        if (pageSize > 100)
        {
            pageSize = 100;
        }

        if (pageNumber < 1)
        {
            pageNumber = 1;
        }

        var query = new ListFileRequest
        {
            ModuleId = moduleId,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("")]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<int>> CreateAsync(
        [FromQuery] int moduleId,
        [FromBody] CreateAndUpdateFileDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!IsAuthorizedEntityId(EntityNames.Module, moduleId))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized File Create Attempt ModuleId={ModuleId}", moduleId);
            return Forbid();
        }

        var command = new CreateFileRequest
        {
            ModuleId = moduleId,
            Name = dto.Name,
            FileName = dto.FileName,
            ImageName = dto.ImageName,
            Description = dto.Description,
            FileSize = dto.FileSize,
            Downloads = dto.Downloads,
            CategoryIds = dto.CategoryIds
        };

        var id = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return Created(
            Url.Action(nameof(GetAsync), new { id, moduleId = command.ModuleId }) ?? string.Empty,
            id);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<int>> UpdateAsync(
        int id,
        [FromQuery] int moduleId,
        [FromBody] CreateAndUpdateFileDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!IsAuthorizedEntityId(EntityNames.Module, moduleId))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized File Update Attempt Id={Id} in ModuleId={ModuleId}", id, moduleId);
            return Forbid();
        }

        var command = new UpdateFileRequest
        {
            Id = id,
            ModuleId = moduleId,
            Name = dto.Name,
            FileName = dto.FileName,
            ImageName = dto.ImageName,
            Description = dto.Description,
            FileSize = dto.FileSize,
            CategoryIds = dto.CategoryIds
        };

        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteAsync(
        int id,
        [FromQuery] int moduleId,
        CancellationToken cancellationToken = default)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleId))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized File Delete Attempt Id={Id} in ModuleId={ModuleId}", id, moduleId);
            return Forbid();
        }

        if (id <= 0)
        {
            return BadRequest("Invalid File ID");
        }

        var command = new DeleteFileRequest
        {
            ModuleId = moduleId,
            Id = id,
        };

        await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return NoContent();
    }

    [HttpPost("upload")]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<string>> UploadFileAsync(
        [FromQuery] int moduleId,
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleId))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized File Upload Attempt ModuleId={ModuleId}", moduleId);
            return Forbid();
        }

        if (file is null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        try
        {
            var alias = _tenantManager.GetAlias();
            var filePath = GetFileStoragePath(alias.TenantId, alias.SiteId, moduleId);
            
            // Ensure directory exists
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            // Generate unique filename to prevent overwrites
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(filePath, fileName);

            // Save the file
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
            }

            _logger.Log(LogLevel.Information, this, LogFunction.Create,
                "File Uploaded FileName={FileName} Size={Size} ModuleId={ModuleId}", 
                fileName, file.Length, moduleId);

            return Ok(fileName);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Create,
                ex, "Error Uploading File ModuleId={ModuleId}", moduleId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Error uploading file");
        }
    }

    [HttpGet("serve/{fileName}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ServeFileAsync(
        string fileName,
        [FromQuery] bool download = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            _logger.Log(LogLevel.Warning, this, LogFunction.Read,
                "ServeFile: Empty filename provided");
            return BadRequest("Filename is required");
        }

        try
        {
            _logger.Log(LogLevel.Information, this, LogFunction.Read,
                "ServeFile: Looking up file FileName={FileName} Download={Download}", fileName, download);

            var fileModuleInfo = await _mediator.Send(
                new GetFileByFileNameRequest { FileName = fileName },
                cancellationToken).ConfigureAwait(false);

            if (fileModuleInfo is null)
            {
                _logger.Log(LogLevel.Warning, this, LogFunction.Read,
                    "ServeFile: File not found in database FileName={FileName}", fileName);
                return NotFound(new { message = "File not found in database", fileName });
            }

            _logger.Log(LogLevel.Information, this, LogFunction.Read,
                "ServeFile: File found in database ModuleId={ModuleId} FileId={FileId} FileName={FileName}", 
                fileModuleInfo.ModuleId, fileModuleInfo.FileId, fileName);

            var alias = _tenantManager.GetAlias();
            var filePath = GetFileStoragePath(alias.TenantId, alias.SiteId, fileModuleInfo.ModuleId);
            var fullPath = Path.Combine(filePath, fileName);

            _logger.Log(LogLevel.Information, this, LogFunction.Read,
                "ServeFile: Checking physical file path Path={Path}", fullPath);

            if (!System.IO.File.Exists(fullPath))
            {
                _logger.Log(LogLevel.Warning, this, LogFunction.Read,
                    "ServeFile: Physical file not found Path={Path}", fullPath);
                return NotFound(new { message = "Physical file not found", path = fullPath });
            }

            // Only increment download counter if this is an actual download (not image display)
            if (download)
            {
                await _mediator.Send(
                    new IncrementDownloadRequest { FileId = fileModuleInfo.FileId, ModuleId = fileModuleInfo.ModuleId },
                    cancellationToken).ConfigureAwait(false);
            }

            var contentType = GetContentType(fileName);
            var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            _logger.Log(LogLevel.Information, this, LogFunction.Read,
                "ServeFile: Successfully serving file FileName={FileName} ModuleId={ModuleId} ContentType={ContentType} Download={Download}", 
                fileName, fileModuleInfo.ModuleId, contentType, download);

            return File(fileStream, contentType, Path.GetFileName(fileName));
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Read,
                ex, "ServeFile: Error serving file FileName={FileName} Error={Error}", fileName, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "Error serving file", error = ex.Message });
        }
    }

    private string GetFileStoragePath(int tenantId, int siteId, int moduleId)
    {
        // Content/Tenants/{TenantId}/Sites/{SiteId}/FileHub/{ModuleId}/
        return Path.Combine(
            _environment.ContentRootPath,
            "Content",
            "Tenants",
            tenantId.ToString(System.Globalization.CultureInfo.InvariantCulture),
            "Sites",
            siteId.ToString(System.Globalization.CultureInfo.InvariantCulture),
            "FileHub",
            moduleId.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            ".json" => "application/json",
            ".xml" => "application/xml",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".svg" => "image/svg+xml",
            ".mp3" => "audio/mpeg",
            ".mp4" => "video/mp4",
            ".avi" => "video/x-msvideo",
            _ => "application/octet-stream"
        };
    }
}
