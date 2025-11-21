// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Features.Categories;

[Route(ControllerRoutes.ApiRoute)]
[ApiController]
public class CategoryController(
    IMediator mediator,
    ILogManager logger,
    IHttpContextAccessor accessor)
    : ModuleControllerBase(logger, accessor)
{
    private readonly IMediator _mediator = mediator;

    #region List Categories Slice

    [HttpGet]
    [Authorize(Policy = PolicyNames.ViewModule)]
    [ProducesResponseType(typeof(PagedResult<ListCategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<ListCategoryResponse>>> ListAsync(
        [FromQuery] int moduleid,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleid))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized Category List Attempt {ModuleId}", moduleid);
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
            ModuleId = moduleid,
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

    #region Get Category Slice

    [HttpGet("{id}/{moduleid}")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    [ProducesResponseType(typeof(GetCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetCategoryResponse>> GetAsync(int id, int moduleid, CancellationToken cancellationToken = default)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleid))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized Category Get Attempt {Id} {ModuleId}", id, moduleid);
            return Forbid();
        }

        if (id <= 0)
        {
            return BadRequest("Invalid Category ID");
        }

        var query = new GetCategoryRequest
        {
            Id = id,
            ModuleId = moduleid
        };

        var category = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

        if (category is null)
        {
            _logger.Log(LogLevel.Warning, this, LogFunction.Read,
                "Category Not Found {Id} {ModuleId}", id, moduleid);
            return NotFound();
        }

        return Ok(category);
    }

    #endregion

    #region Create Category Slice

    [HttpPost]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<int>> CreateAsync([FromBody] CreateCategoryRequest command, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!IsAuthorizedEntityId(EntityNames.Module, command.ModuleId))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized Category Create Attempt {Command}", command);
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return BadRequest("Category name is required");
        }

        var id = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return Created(
            Url.Action(nameof(GetAsync), new { id, moduleid = command.ModuleId }) ?? string.Empty,
            id);
    }

    #endregion

    #region Update Category Slice

    [HttpPut("{id}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<int>> UpdateAsync(int id, [FromBody] UpdateCategoryRequest command, CancellationToken cancellationToken = default)
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
                "Unauthorized Category Update Attempt {Command}", command);
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return BadRequest("Category name is required");
        }

        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return Ok(result);
    }

    #endregion

    #region Delete Category Slice

    [HttpDelete("{id}/{moduleid}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(int id, int moduleid, CancellationToken cancellationToken = default)
    {
        if (!IsAuthorizedEntityId(EntityNames.Module, moduleid))
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized Category Delete Attempt {Id} {ModuleId}", id, moduleid);
            return Forbid();
        }

        if (id <= 0)
        {
            return BadRequest("Invalid Category ID");
        }

        var checkQuery = new GetCategoryRequest { Id = id, ModuleId = moduleid };
        var exists = await _mediator.Send(checkQuery, cancellationToken).ConfigureAwait(false);

        if (exists is null)
        {
            _logger.Log(LogLevel.Warning, this, LogFunction.Delete,
                "Attempted to delete non-existent Category {Id} {ModuleId}", id, moduleid);
            return NotFound();
        }

        var command = new DeleteCategoryRequest
        {
            Id = id,
            ModuleId = moduleid
        };

        await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return NoContent();
    }

    #endregion
}
