// Licensed to ICTAce under the MIT license.

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
    [HttpGet("{moduleId}/{id}")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    [ProducesResponseType(typeof(GetMyModuleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetMyModuleResponse>> GetAsync(int moduleId, int id, CancellationToken cancellationToken = default)
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
    [HttpGet("{moduleId}")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    [ProducesResponseType(typeof(PagedResult<ListMyModuleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<ListMyModuleResponse>>> ListAsync(int moduleId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
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
    [HttpPost("{moduleId}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<int>> CreateAsync(int moduleId, [FromBody] CreateMyModuleRequest command, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!IsAuthorizedEntityId(EntityNames.Module, command.ModuleId))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized MyModule Create Attempt ModuleId={ModuleId}", command);
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return BadRequest("MyModule name is required");
        }

        var id = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return Created(
            Url.Action(nameof(GetAsync), new { id, moduleid = command.ModuleId }) ?? string.Empty,
            id);
    }

    #endregion

    #region Update MyModule Slice

    /// <summary>
    /// UPDATE SLICE: Updates an existing MyModule
    /// This slice contains all logic for updating a MyModule.
    /// </summary>
    [HttpPut("{moduleId}/{id}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<int>> UpdateAsync(int moduleId, int id, [FromBody] UpdateMyModuleRequest command, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (command.Id != id)
        {
            return BadRequest("ID mismatch between route and body");
        }

        if (!IsAuthorizedEntityId(EntityNames.Module, command.ModuleId))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized MyModule Update Attempt ModuleId={ModuleId}", command);
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return BadRequest("MyModule name is required");
        }

        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return Ok(result);
    }

    #endregion

    #region Delete MyModule Slice

    /// <summary>
    /// DELETE SLICE: Deletes a specific MyModule
    /// This slice contains all logic for deleting a MyModule.
    /// </summary>
    [HttpDelete("{moduleId}/{id}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(int moduleId, int id, CancellationToken cancellationToken = default)
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

        var checkQuery = new GetMyModuleRequest { Id = id, ModuleId = moduleId };
        var exists = await _mediator.Send(checkQuery, cancellationToken).ConfigureAwait(false);

        if (exists is null)
        {
            _logger.Log(LogLevel.Warning, this, LogFunction.Delete,
                "Attempted to delete non-existent MyModule Id={Id} in ModuleId={ModuleId}", id, moduleId);
            return NotFound();
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
