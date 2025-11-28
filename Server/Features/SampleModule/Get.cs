// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.SampleModule;

public record GetSampleModuleRequest : RequestBase, IRequest<GetSampleModuleDto>
{
    public int Id { get; set; }
}

public class GetHandler(HandlerServices<ApplicationQueryContext> services)
    : HandlerBase<ApplicationQueryContext>(services), IRequestHandler<GetSampleModuleRequest, GetSampleModuleDto?>
{
    private static readonly GetMapper _mapper = new();

    public async Task<GetSampleModuleDto?> Handle(GetSampleModuleRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.View))
        {
            using var db = CreateDbContext();
            var entity = await db.SampleModule.SingleOrDefaultAsync(m => m.Id == request.Id && m.ModuleId == request.ModuleId, cancellationToken).ConfigureAwait(false);
            if (entity is null)
            {
                Logger.Log(LogLevel.Error, this, LogFunction.Security, "SampleModule not found Id={Id} in ModuleId= {ModuleId}", request.Id, request.ModuleId);
                return null;
            }

            return _mapper.ToGetResponse(entity);
        }

        Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized SampleModule Get Attempt {Id} {ModuleId}", request.Id, request.ModuleId);
        return null;
    }
}

[Mapper]
internal sealed partial class GetMapper
{
    /// <summary>
    /// Maps SampleModule entity to GetSampleModuleResponse DTO
    /// </summary>
    public partial GetSampleModuleDto ToGetResponse(Persistence.Entities.SampleModule sampleModule);
}
