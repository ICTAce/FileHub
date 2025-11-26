// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Categories;

[Route("api/ictace/fileHub/categories")]
[ApiController]
public class ICTAceFileHubCategoriesController(
    IMediator mediator,
    ILogManager logger,
    IHttpContextAccessor accessor)
    : ModuleControllerBase(logger, accessor)
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("{id}")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    [ProducesResponseType(typeof(GetCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetCategoryDto>> GetAsync(
        int id,
        [FromQuery] int moduleId,
        CancellationToken cancellationToken = default)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleId))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized FileHub Category Get Attempt Id={Id} in ModuleId={ModuleId}", id, moduleId);
            return Forbid();
        }

        if (id <= 0)
        {
            return BadRequest("Invalid Category ID");
        }

        var query = new GetCategoryRequest
        {
            ModuleId = moduleId,
            Id = id,
        };

        var category = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

        if (category is null)
        {
            _logger.Log(LogLevel.Warning, this, LogFunction.Read,
                "Category Not Found Id={Id} in ModuleId={ModuleId}", id, moduleId);
            return NotFound();
        }

        return Ok(category);
    }

    [HttpGet("")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    [ProducesResponseType(typeof(PagedResult<ListCategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<ListCategoryDto>>> ListAsync(
        [FromQuery] int moduleId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleId))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized Category List Attempt ModuleId={ModuleId}", moduleId);
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

        var query = new ListCategoryRequest
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
        [FromBody] CreateAndUpdateCategoryDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!IsAuthorizedEntityId(EntityNames.Module, moduleId))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized Category Create Attempt ModuleId={ModuleId}", moduleId);
            return Forbid();
        }

        var command = new CreateCategoryRequest
        {
            ModuleId = moduleId,
            Name = dto.Name,
            ViewOrder = dto.ViewOrder,
            ParentId = dto.ParentId,
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
        [FromBody] CreateAndUpdateCategoryDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!IsAuthorizedEntityId(EntityNames.Module, moduleId))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized Category Update Attempt Id={Id} in ModuleId={ModuleId}", id, moduleId);
            return Forbid();
        }

        var command = new UpdateCategoryRequest
        {
            Id = id,
            ModuleId = moduleId,
            Name = dto.Name,
            ViewOrder = dto.ViewOrder,
            ParentId = dto.ParentId,
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
                "Unauthorized Category Delete Attempt Id={Id} in ModuleId={ModuleId}", id, moduleId);
            return Forbid();
        }

        if (id <= 0)
        {
            return BadRequest("Invalid Category ID");
        }

        var command = new DeleteCategoryRequest
        {
            ModuleId = moduleId,
            Id = id,
        };

        await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return NoContent();
    }
}
