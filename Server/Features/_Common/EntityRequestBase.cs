// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Common;

/// <summary>
/// Base class for requests that operate on a specific entity by Id
/// </summary>
public abstract record EntityRequestBase : RequestBase
{
    [Required(ErrorMessage = "Id is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
    public int Id { get; set; }
}
