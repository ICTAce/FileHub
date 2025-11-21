// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Contracts.Categories;

public record GetCategoryRequest : RequestBase, IRequest<GetCategoryResponse>
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
    public int Id { get; set; }
}

public record GetCategoryResponse
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public required string Name { get; set; }
    public int ViewOrder { get; set; }
    public int ParentId { get; set; }

    public required string CreatedBy { get; set; }
    public required DateTime CreatedOn { get; set; }
    public required string ModifiedBy { get; set; }
    public required DateTime ModifiedOn { get; set; }
}
