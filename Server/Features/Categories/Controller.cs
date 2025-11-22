// Licensed to ICTAce under the MIT license.

using ICTAce.FileHub.Client.Services.Common;

namespace ICTAce.FileHub.Features.Categories;

/// <summary>
/// DTO for creating or updating a Category
/// </summary>
public record CreateUpdateCategoryDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "ViewOrder must be greater than or equal to 0")]
    public int ViewOrder { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "ParentId must be greater than or equal to 0")]
    public int ParentId { get; set; }
}

/// <summary>
/// Vertical slice controller for FileHub Category operations.
/// Each action method contains its complete request/response logic in one place,
/// following VSA principles while maintaining Oqtane controller conventions.
/// </summary>
[Route(ControllerRoutes.ApiRoute)]
[ApiController]
public class FileHubCategoryController : ModuleControllerBase
{
    private readonly IMediator _mediator;

    public FileHubCategoryController(IMediator mediator, ILogManager logger, IHttpContextAccessor accessor)
        : base(logger, accessor)
    {
        _mediator = mediator;
    }

    #region Get Category Slice

    /// <summary>
    /// GET SLICE: Retrieves a specific Category by ID
    /// This slice contains all logic for getting a single Category.
    /// </summary>
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
                "Unauthorized Category Get Attempt Id={Id} in ModuleId={ModuleId}", id, moduleId);
            return Forbid();
        }

        if (id <= 0)
        {
            return BadRequest("Invalid Category ID");
        }

        var query = new GetCategoryRequest
        {
            ModuleId = moduleId,
            Id = id
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

    #endregion

    #region List Categories Slice

    /// <summary>
    /// LIST SLICE: Retrieves a paginated list of Categories for the specified module
    /// This slice contains all logic for listing Categories in one cohesive unit.
    /// </summary>
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

    #endregion

    #region Create Category Slice

    /// <summary>
    /// CREATE SLICE: Creates a new Category
    /// This slice contains all logic for creating a Category from request to response.
    /// </summary>
    [HttpPost("")]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<int>> CreateAsync(
        [FromQuery] int moduleId,
        [FromBody] CreateUpdateCategoryDto dto,
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
            ParentId = dto.ParentId
        };

        var id = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return Created(
            Url.Action(nameof(GetAsync), new { id, moduleId = command.ModuleId }) ?? string.Empty,
            id);
    }

    #endregion

    #region Update Category Slice

    /// <summary>
    /// UPDATE SLICE: Updates an existing Category
    /// This slice contains all logic for updating a Category.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<int>> UpdateAsync(
        int id,
        [FromQuery] int moduleId,
        [FromBody] CreateUpdateCategoryDto dto,
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
            ParentId = dto.ParentId
        };

        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return Ok(result);
    }

    #endregion

    #region Delete Category Slice

    /// <summary>
    /// DELETE SLICE: Deletes a specific Category
    /// This slice contains all logic for deleting a Category.
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
            Id = id
        };

        await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return NoContent();
    }

    #endregion
}
