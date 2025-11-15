// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Controllers;

/// <summary>
/// Controller for managing MyModule resources within a module context
/// </summary>
[Route(ControllerRoutes.ApiRoute)]
[ApiController]
public class MyModuleController : ModuleControllerBase
{
    private readonly IMediator _mediator;

    public MyModuleController(IMediator mediator, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a paginated list of MyModules for the specified module
    /// </summary>
    /// <param name="moduleid">The module identifier</param>
    /// <param name="pageNumber">Page number (1-based, default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 100)</param>
    /// <returns>Paginated collection of MyModules</returns>
    [HttpGet]
    [Authorize(Policy = PolicyNames.ViewModule)]
    [ProducesResponseType(typeof(PagedResult<ListMyModulesResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<ListMyModulesResponse>>> GetAll(
        [FromQuery] int moduleid,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleid))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule List Attempt {ModuleId}", moduleid);
            return Forbid();
        }

        var query = new ListMyModulesRequest
        {
            ModuleId = moduleid,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific MyModule by ID
    /// </summary>
    /// <param name="id">The MyModule identifier</param>
    /// <param name="moduleid">The module identifier</param>
    /// <returns>The requested MyModule</returns>
    [HttpGet("{id}/{moduleid}")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    [ProducesResponseType(typeof(GetMyModuleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetMyModuleResponse>> Get(int id, int moduleid)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleid))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Get Attempt {Id} {ModuleId}", id, moduleid);
            return Forbid();
        }

        var query = new GetMyModuleRequest
        {
            Id = id,
            ModuleId = moduleid
        };

        var myModule = await _mediator.Send(query).ConfigureAwait(false);

        if (myModule is null)
        {
            _logger.Log(LogLevel.Warning, this, LogFunction.Read, "MyModule Not Found {Id} {ModuleId}", id, moduleid);
            return NotFound();
        }

        return Ok(myModule);
    }

    /// <summary>
    /// Creates a new MyModule
    /// </summary>
    /// <param name="command">The creation request containing MyModule details</param>
    /// <returns>The ID of the created MyModule</returns>
    [HttpPost]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<int>> Create([FromBody] CreateMyModuleRequest command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!IsAuthorizedEntityId(EntityNames.Module, command.ModuleId))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Create Attempt {Command}", command);
            return Forbid();
        }

        var id = await _mediator.Send(command).ConfigureAwait(false);

        return CreatedAtAction(
            nameof(Get),
            new { id, moduleid = command.ModuleId },
            id);
    }

    /// <summary>
    /// Updates an existing MyModule
    /// </summary>
    /// <param name="id">The MyModule identifier</param>
    /// <param name="command">The update request containing modified MyModule details</param>
    /// <returns>The updated MyModule ID</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<int>> Update(int id, [FromBody] UpdateMyModuleRequest command)
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
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Update Attempt {Command}", command);
            return Forbid();
        }

        var result = await _mediator.Send(command).ConfigureAwait(false);

        return Ok(result);
    }

    /// <summary>
    /// Deletes a specific MyModule
    /// </summary>
    /// <param name="id">The MyModule identifier</param>
    /// <param name="moduleid">The module identifier</param>
    [HttpDelete("{id}/{moduleid}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id, int moduleid)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleid))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Delete Attempt {Id} {ModuleId}", id, moduleid);
            return Forbid();
        }

        var command = new DeleteMyModuleRequest
        {
            Id = id,
            ModuleId = moduleid
        };

        await _mediator.Send(command).ConfigureAwait(false);

        return NoContent();
    }
}
