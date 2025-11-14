using ICTAce.FileHub.Client.Features.MyModules;
using ICTAce.FileHub.Features.MyModules;
using ICTAce.FileHub.Models;
using MediatR;

namespace ICTAce.FileHub.Controllers;

[Route(ControllerRoutes.ApiRoute)]
public class MyModuleController : ModuleControllerBase
{
    private readonly IMediator _mediator;

    public MyModuleController(IMediator mediator, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
    {
        _mediator = mediator;
    }

    // GET: api/<controller>?moduleid=x
    [HttpGet]
    [Authorize(Policy = PolicyNames.ViewModule)]
    public async Task<IEnumerable<ListMyModulesResponse>> Get(string moduleid)
    {
        int ModuleId;
        if (int.TryParse(moduleid, out ModuleId) && IsAuthorizedEntityId(EntityNames.Module, ModuleId))
        {
            var query = new ListMyModulesRequest { ModuleId = ModuleId };
            return await _mediator.Send(query).ConfigureAwait(false);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Get Attempt {ModuleId}", moduleid);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }
    }

    // GET api/<controller>/5
    [HttpGet("{id}/{moduleid}")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    public async Task<Models.MyModule> Get(int id, int moduleid)
    {
        if (IsAuthorizedEntityId(EntityNames.Module, moduleid))
        {
            var query = new GetMyModuleByIdQuery
            {
                MyModuleId = id,
                ModuleId = moduleid
            };
            var myModule = await _mediator.Send(query).ConfigureAwait(false);

            if (myModule == null)
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Get Attempt {MyModuleId} {ModuleId}", id, moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
            return myModule;
        }
        else
        { 
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Get Attempt {MyModuleId} {ModuleId}", id, moduleid);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }
    }

    // POST api/<controller>
    [HttpPost]
    [Authorize(Policy = PolicyNames.EditModule)]
    public async Task<Models.MyModule> Post([FromBody] CreateMyModuleRequest command)
    {
        if (ModelState.IsValid && IsAuthorizedEntityId(EntityNames.Module, command.ModuleId))
        {
            // Map DTO to MediatR request
            var request = new CreateMyModuleRequest
            {
                ModuleId = command.ModuleId,
                Name = command.Name
            };
            var myModule = await _mediator.Send(request).ConfigureAwait(false);
            return myModule;
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Post Attempt {Command}", command);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }
    }

    // PUT api/<controller>/5
    [HttpPut("{id}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    public async Task<Models.MyModule> Put(int id, [FromBody] UpdateMyModuleCommand command)
    {
        if (ModelState.IsValid && command.MyModuleId == id && IsAuthorizedEntityId(EntityNames.Module, command.ModuleId))
        {
            // Map DTO to MediatR request
            var request = new UpdateMyModuleRequest
            {
                MyModuleId = command.MyModuleId,
                ModuleId = command.ModuleId,
                Name = command.Name
            };
            var myModule = await _mediator.Send(request).ConfigureAwait(false);
            return myModule;
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Put Attempt {Command}", command);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }
    }

    // DELETE api/<controller>/5
    [HttpDelete("{id}/{moduleid}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    public async Task Delete(int id, int moduleid)
    {
        if (IsAuthorizedEntityId(EntityNames.Module, moduleid))
        {
            var command = new DeleteMyModuleCommand
            {
                MyModuleId = id,
                ModuleId = moduleid
            };
            await _mediator.Send(command).ConfigureAwait(false);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Delete Attempt {MyModuleId} {ModuleId}", id, moduleid);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        }
    }
}
