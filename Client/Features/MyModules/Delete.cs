namespace ICTAce.FileHub.Client.Features.MyModules;

public class DeleteMyModuleRequest : IRequest<int>
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "ModuleId must be greater than 0")]
    public int ModuleId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
    public int Id { get; set; }
}
