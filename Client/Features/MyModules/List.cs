// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.MyModules;

public class ListMyModulesRequest : RequestBase, IRequest<PagedResult<ListMyModulesResponse>>
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

public class ListMyModulesResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
}
