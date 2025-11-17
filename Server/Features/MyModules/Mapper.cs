// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Features.MyModules;

/// <summary>
/// Mapper for MyModule entities to DTOs using Mapperly source generation
/// </summary>
[Mapper]
public partial class Mapper
{
    /// <summary>
    /// Maps MyModule entity to ListMyModuleResponse DTO
    /// </summary>
    public partial ListMyModuleResponse ToListResponse(Persistence.Entities.MyModule myModule);

    /// <summary>
    /// Maps MyModule entity to GetMyModuleResponse DTO
    /// </summary>
    public partial GetMyModuleResponse ToGetResponse(Persistence.Entities.MyModule myModule);
}
