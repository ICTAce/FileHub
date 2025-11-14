namespace ICTAce.FileHub.Client.Features.MyModules;

public class ListMyModulesRequest : IRequest<List<ListMyModulesResponse>>
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "ModuleId must be greater than 0")]
    public int ModuleId { get; set; }
}

public class ListMyModulesResponse
{
    public int MyModuleId { get; set; }
    public string Name { get; set; }
}
