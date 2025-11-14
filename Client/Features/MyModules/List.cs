// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.MyModules;

public class ListMyModulesRequest : RequestBase, IRequest<List<ListMyModulesResponse>>;

public class ListMyModulesResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
}
