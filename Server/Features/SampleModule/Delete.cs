// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.SampleModule;

public record DeleteSampleModuleRequest : RequestBase, IRequest<int>
{
    public int Id { get; set; }
}

public class DeleteHandler(HandlerServices<ApplicationCommandContext> services)
    : HandlerBase<ApplicationCommandContext>(services), IRequestHandler<DeleteSampleModuleRequest, int>
{
    public async Task<int> Handle(DeleteSampleModuleRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        // Enforce authorization at the aggregate boundary
        if (!IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized SampleModule Delete Attempt {Id} {ModuleId}", request.Id, request.ModuleId);
            return -1;
        }

        using var db = CreateDbContext();
        var rowsAffected = await db.SampleModule
            .Where(m => m.Id == request.Id && m.ModuleId == request.ModuleId)
            .ExecuteDeleteAsync(cancellationToken)
            .ConfigureAwait(false);

        if (rowsAffected > 0)
        {
            Logger.Log(LogLevel.Information, this, LogFunction.Delete, "SampleModule Deleted {Id}", request.Id);
            return request.Id;
        }
        else
        {
            Logger.Log(LogLevel.Warning, this, LogFunction.Delete, "SampleModule Not Found {Id}", request.Id);
            return -1;
        }
    }
}
