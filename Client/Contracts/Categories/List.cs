// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Contracts.Categories;

public record ListCategoryRequest : RequestBase, IRequest<PagedResult<ListCategoryResponse>>
{
    /// <summary>
    /// Page number (1-based). Defaults to 1 if not specified.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page. Defaults to 10 if not specified.
    /// </summary>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; set; } = 10;
}

public record ListCategoryResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int ViewOrder { get; set; }
    public int ParentId { get; set; }
}
