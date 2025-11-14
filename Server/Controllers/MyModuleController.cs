// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Controllers;

[Route(ControllerRoutes.ApiRoute)]
public class MyModuleController : ModuleControllerBase
{
    private readonly IMediator _mediator;

    public MyModuleController(IMediator mediator, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = PolicyNames.ViewModule)]
    public async Task<IEnumerable<ListMyModulesResponse>> List(string moduleid)
    {
        if (!int.TryParse(moduleid, out int ModuleId))
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return null;
        }

        var query = new ListMyModulesRequest { ModuleId = ModuleId };
        return await _mediator.Send(query);
    }

    [HttpGet("{id}/{moduleid}")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    public async Task<GetMyModuleResponse> Get(int id, int moduleid)
    {
        if (IsAuthorizedEntityId(EntityNames.Module, moduleid))
        {
            var query = new GetMyModuleRequest
            {
                Id = id,
                ModuleId = moduleid
            };
            var myModule = await _mediator.Send(query).ConfigureAwait(false);

            if (myModule == null)
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Get Attempt {Id} {ModuleId}", id, moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
            return myModule;
        }
        else
        { 
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Get Attempt {Id} {ModuleId}", id, moduleid);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }
    }

    [HttpPost]
    [Authorize(Policy = PolicyNames.EditModule)]
    public async Task<int> Post([FromBody] CreateMyModuleRequest command)
    {
        if (ModelState.IsValid && IsAuthorizedEntityId(EntityNames.Module, command.ModuleId))
        {
            return await _mediator.Send(command).ConfigureAwait(false);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Post Attempt {Command}", command);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return -1;
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    public async Task<int> Put(int id, [FromBody] UpdateMyModuleRequest command)
    {
        if (ModelState.IsValid && command.Id == id && IsAuthorizedEntityId(EntityNames.Module, command.ModuleId))
        {
            return await _mediator.Send(command).ConfigureAwait(false);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Put Attempt {Command}", command);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return -1;
        }
    }

    [HttpDelete("{id}/{moduleid}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    public async Task Delete(int id, int moduleid)
    {
        if (IsAuthorizedEntityId(EntityNames.Module, moduleid))
        {
            var command = new DeleteMyModuleRequest
            {
                Id = id,
                ModuleId = moduleid
            };
            await _mediator.Send(command).ConfigureAwait(false);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Delete Attempt {Id} {ModuleId}", id, moduleid);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        }
    }
}
