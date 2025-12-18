// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Files;

[Route("api/ictace/fileHub/files")]
[ApiController]
public class ICTAceFileHubFilesController(
    IMediator mediator,
    ILogManager logger,
    IHttpContextAccessor accessor)
    : ModuleControllerBase(logger, accessor)
{
    private readonly IMediator _mediator = mediator;

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
            Downloads = dto.Downloads,
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
}
