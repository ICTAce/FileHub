// Licensed to ICTAce under the MIT license.

using Riok.Mapperly.Abstractions;
using MyModuleEntity = ICTAce.FileHub.Entities.MyModule;

namespace ICTAce.FileHub.Features.MyModules;

/// <summary>
/// Mapper for MyModule entities to DTOs using Mapperly source generation
/// </summary>
[Mapper]
public partial class Mapper
{
    /// <summary>
    /// Maps MyModule entity to ListMyModuleResponse DTO
    /// </summary>
    public partial ListMyModuleResponse ToListResponse(MyModuleEntity myModule);

    /// <summary>
    /// Maps MyModule entity to GetMyModuleResponse DTO
    /// </summary>
    public partial GetMyModuleResponse ToGetResponse(MyModuleEntity myModule);
}
