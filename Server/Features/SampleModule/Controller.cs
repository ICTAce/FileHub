// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.SampleModule;

[Route("api/company/sampleModules")]
[ApiController]
public class CompanySampleModulesController : ModuleControllerBase
{
    private readonly IMediator _mediator;

    public CompanySampleModulesController(IMediator mediator, ILogManager logger, IHttpContextAccessor accessor)
        : base(logger, accessor)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    [ProducesResponseType(typeof(GetSampleModuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetSampleModuleDto>> GetAsync(
        int id,
        [FromQuery] int moduleId,
        CancellationToken cancellationToken = default)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleId))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized MyModule Get Attempt Id={Id} in ModuleId={ModuleId}", id, moduleId);
            return Forbid();
        }

        if (id <= 0)
        {
            return BadRequest("Invalid MyModule ID");
        }

        var query = new GetMyModuleRequest
        {
            ModuleId = moduleId,
            Id = id,
        };

        var myModule = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

        if (myModule is null)
        {
            _logger.Log(LogLevel.Warning, this, LogFunction.Read,
                "MyModule Not Found Id={Id} in ModuleId={ModuleId}", id, moduleId);
            return NotFound();
        }

        return Ok(myModule);
    }

    [HttpGet("")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    [ProducesResponseType(typeof(PagedResult<ListSampleModuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<ListSampleModuleDto>>> ListAsync(
        [FromQuery] int moduleId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleId))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized MyModule List Attempt ModuleId={ModuleId}", moduleId);
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

        var query = new ListSampleModuleRequest
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
        [FromBody] CreateAndUpdateSampleModuleDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var request = new CreateSampleModuleRequest
        {
            ModuleId = moduleId,
            Name = dto.Name,
        };

        var id = await _mediator.Send(request, cancellationToken).ConfigureAwait(false);

        return Created(
            Url.Action(nameof(GetAsync), new { id, moduleId = request.ModuleId }) ?? string.Empty,
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
        [FromBody] CreateAndUpdateSampleModuleDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var request = new UpdateSampleModuleRequest
        {
            Id = id,
            ModuleId = moduleId,
            Name = dto.Name,
        };

        var result = await _mediator.Send(request, cancellationToken).ConfigureAwait(false);

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
                "Unauthorized MyModule Delete Attempt Id={Id} in ModuleId={ModuleId}", id, moduleId);
            return Forbid();
        }

        if (id <= 0)
        {
            return BadRequest("Invalid MyModule ID");
        }

        var command = new DeleteSampleModuleRequest
        {
            ModuleId = moduleId,
            Id = id,
        };

        await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return NoContent();
    }
}
