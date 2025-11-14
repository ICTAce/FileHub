namespace ICTAce.FileHub.Client.Features.MyModules;

public class UpdateMyModuleRequest : IRequest<int>
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "ModuleId must be greater than 0")]
    public int ModuleId { get; set; }

    public int Id { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]

    public string Name { get; set; }
}
