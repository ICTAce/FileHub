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

    #region List MyModules Slice
    
    /// <summary>
    /// LIST SLICE: Retrieves a paginated list of MyModules for the specified module
    /// This slice contains all logic for listing MyModules in one cohesive unit.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = PolicyNames.ViewModule)]
    [ProducesResponseType(typeof(PagedResult<ListMyModuleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<ListMyModuleResponse>>> ListAsync(
        [FromQuery] int moduleid,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        // Authorization - this is the entry point for this slice
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleid))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, 
                "Unauthorized MyModule List Attempt {ModuleId}", moduleid);
            return Forbid();
        }

        // Validation - part of this slice's responsibility
        if (pageSize > 100) pageSize = 100;
        if (pageNumber < 1) pageNumber = 1;

        // Business logic request
        var query = new ListMyModuleRequest
        {
            ModuleId = moduleid,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        // Response handling - completing the slice
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
    
    #endregion

    #region Get MyModule Slice
    
    /// <summary>
    /// GET SLICE: Retrieves a specific MyModule by ID
    /// This slice contains all logic for getting a single MyModule.
    /// </summary>
    [HttpGet("{id}/{moduleid}")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    [ProducesResponseType(typeof(GetMyModuleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetMyModuleResponse>> GetAsync(int id, int moduleid)
    {
        // Authorization - entry point validation
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleid))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, 
                "Unauthorized MyModule Get Attempt {Id} {ModuleId}", id, moduleid);
            return Forbid();
        }

        // Input validation - part of slice responsibility
        if (id <= 0)
        {
            return BadRequest("Invalid MyModule ID");
        }

        // Business logic request
        var query = new GetMyModuleRequest
        {
            Id = id,
            ModuleId = moduleid
        };

        var myModule = await _mediator.Send(query).ConfigureAwait(false);

        // Response handling - completing the slice
        if (myModule is null)
        {
            _logger.Log(LogLevel.Warning, this, LogFunction.Read, 
                "MyModule Not Found {Id} {ModuleId}", id, moduleid);
            return NotFound();
        }

        return Ok(myModule);
    }
    
    #endregion

    #region Create MyModule Slice
    
    /// <summary>
    /// CREATE SLICE: Creates a new MyModule
    /// This slice contains all logic for creating a MyModule from request to response.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<int>> CreateAsync([FromBody] CreateMyModuleRequest command)
    {
        // Input validation - slice entry point
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Authorization - part of slice responsibility
        if (!IsAuthorizedEntityId(EntityNames.Module, command.ModuleId))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, 
                "Unauthorized MyModule Create Attempt {Command}", command);
            return Forbid();
        }

        // Additional business validation within this slice
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return BadRequest("MyModule name is required");
        }

        // Business logic execution
        var id = await _mediator.Send(command).ConfigureAwait(false);

        // Response generation - completing the slice
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
    [HttpPut("{id}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<int>> UpdateAsync(int id, [FromBody] UpdateMyModuleRequest command)
    {
        // Input validation - slice entry point
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Route/body consistency validation
        if (command.Id != id)
        {
            return BadRequest("ID mismatch between route and body");
        }

        // Authorization - part of slice responsibility
        if (!IsAuthorizedEntityId(EntityNames.Module, command.ModuleId))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, 
                "Unauthorized MyModule Update Attempt {Command}", command);
            return Forbid();
        }

        // Additional business validation
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return BadRequest("MyModule name is required");
        }

        // Business logic execution
        var result = await _mediator.Send(command).ConfigureAwait(false);

        // Response - completing the slice
        return Ok(result);
    }
    
    #endregion

    #region Delete MyModule Slice
    
    /// <summary>
    /// DELETE SLICE: Deletes a specific MyModule
    /// This slice contains all logic for deleting a MyModule.
    /// </summary>
    [HttpDelete("{id}/{moduleid}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(int id, int moduleid)
    {
        // Authorization - slice entry point
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleid))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, 
                "Unauthorized MyModule Delete Attempt {Id} {ModuleId}", id, moduleid);
            return Forbid();
        }

        // Input validation
        if (id <= 0)
        {
            return BadRequest("Invalid MyModule ID");
        }

        // Check if resource exists before attempting delete
        var checkQuery = new GetMyModuleRequest { Id = id, ModuleId = moduleid };
        var exists = await _mediator.Send(checkQuery);
        
        if (exists is null)
        {
            _logger.Log(LogLevel.Warning, this, LogFunction.Delete, 
                "Attempted to delete non-existent MyModule {Id} {ModuleId}", id, moduleid);
            return NotFound();
        }

        // Business logic execution
        var command = new DeleteMyModuleRequest
        {
            Id = id,
            ModuleId = moduleid
        };

        await _mediator.Send(command).ConfigureAwait(false);

        // Successful deletion response
        return NoContent();
    }
    
    #endregion
}