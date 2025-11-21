// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Contracts.Categories;

public record DeleteCategoryRequest : RequestBase, IRequest<int>
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
    public int Id { get; set; }
}
