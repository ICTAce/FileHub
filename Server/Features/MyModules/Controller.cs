// Licensed to ICTAce under the MIT license.

using ICTAce.FileHub.Client.Services.Common;

namespace ICTAce.FileHub.Server.Features.MyModules;

/// <summary>
/// Vertical slice controller for MyModule operations.
/// Each action method contains its complete request/response logic in one place,
/// following VSA principles while maintaining Oqtane controller conventions.
/// </summary>
[Route(ControllerRoutes.ApiRoute)]
[ApiController]
public class MyModuleController : ModuleControllerBase
{
    private readonly IMediator _mediator;

    public MyModuleController(IMediator mediator, ILogManager logger, IHttpContextAccessor accessor)
        : base(logger, accessor)
    {
        _mediator = mediator;
    }

    #region Get MyModule Slice

    /// <summary>
    /// GET SLICE: Retrieves a specific MyModule by ID
    /// This slice contains all logic for getting a single MyModule.
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    [ProducesResponseType(typeof(GetMyModuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetMyModuleDto>> GetAsync(
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

    #endregion

    #region List MyModules Slice

    /// <summary>
    /// LIST SLICE: Retrieves a paginated list of MyModules for the specified module
    /// This slice contains all logic for listing MyModules in one cohesive unit.
    /// </summary>
    [HttpGet("")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    [ProducesResponseType(typeof(PagedResult<ListMyModuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<ListMyModuleDto>>> ListAsync(
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

        var query = new ListMyModuleRequest
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

    #endregion

    #region Create MyModule Slice

    /// <summary>
    /// CREATE SLICE: Creates a new MyModule
    /// This slice contains all logic for creating a MyModule from request to response.
    /// </summary>
    [HttpPost("")]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<int>> CreateAsync(
        [FromQuery] int moduleId,
        [FromBody] CreateUpdateMyModuleDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var request = new CreateMyModuleRequest
        {
            ModuleId = moduleId,
            Name = dto.Name,
        };

        var id = await _mediator.Send(request, cancellationToken).ConfigureAwait(false);

        return Created(
            Url.Action(nameof(GetAsync), new { id, moduleId = request.ModuleId }) ?? string.Empty,
            id);
    }

    #endregion

    #region Update MyModule Slice

    /// <summary>
    /// UPDATE SLICE: Updates an existing MyModule
    /// This slice contains all logic for updating a MyModule.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<int>> UpdateAsync(
        int id,
        [FromQuery] int moduleId,
        [FromBody] CreateUpdateMyModuleDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var request = new UpdateMyModuleRequest
        {
            Id = id,
            ModuleId = moduleId,
            Name = dto.Name,
        };

        var result = await _mediator.Send(request, cancellationToken).ConfigureAwait(false);

        return Ok(result);
    }

    #endregion

    #region Delete MyModule Slice

    /// <summary>
    /// DELETE SLICE: Deletes a specific MyModule
    /// This slice contains all logic for deleting a MyModule.
    /// </summary>
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

        var command = new DeleteMyModuleRequest
        {
            ModuleId = moduleId,
            Id = id,
        };

        await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return NoContent();
    }

    #endregion
}
