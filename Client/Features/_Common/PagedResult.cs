// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Common;

/// <summary>
/// Represents a paginated result set for RESTful API responses
/// </summary>
/// <typeparam name="T">The type of items in the result set</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// The collection of items for the current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = [];

    /// <summary>
    /// The current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// The number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// The total number of pages
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    /// <summary>
    /// Indicates whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indicates whether there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
