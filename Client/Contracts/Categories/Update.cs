// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Contracts.Categories;

public record UpdateCategoryRequest : RequestBase, IRequest<int>
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    public required string Name { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "ViewOrder must be greater than or equal to 0")]
    public int ViewOrder { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "ParentId must be greater than or equal to 0")]
    public int ParentId { get; set; }
}
