using MediatR;
using ICTAce.FileHub.Features.MyModules;

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
    public async Task<IEnumerable<Models.MyModule>> Get(string moduleid)
    {
        int ModuleId;
        if (int.TryParse(moduleid, out ModuleId) && IsAuthorizedEntityId(EntityNames.Module, ModuleId))
        {
            var query = new GetMyModulesQuery { ModuleId = ModuleId };
            return await _mediator.Send(query);
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
            var myModule = await _mediator.Send(query);
            
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
    public async Task<Models.MyModule> Post([FromBody] Models.MyModule MyModule)
    {
        if (!ModelState.IsValid && IsAuthorizedEntityId(EntityNames.Module, MyModule.ModuleId))
        {
            var command = new AddMyModuleCommand { MyModule = MyModule };
            MyModule = await _mediator.Send(command);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Post Attempt {MyModule}", MyModule);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            MyModule = null;
        }
        return MyModule;
    }

    // PUT api/<controller>/5
    [HttpPut("{id}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    public async Task<Models.MyModule> Put(int id, [FromBody] Models.MyModule MyModule)
    {
        if (ModelState.IsValid && MyModule.MyModuleId == id && IsAuthorizedEntityId(EntityNames.Module, MyModule.ModuleId))
        {
            var command = new UpdateMyModuleCommand { MyModule = MyModule };
            MyModule = await _mediator.Send(command);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Put Attempt {MyModule}", MyModule);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            MyModule = null;
        }
        return MyModule;
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
            await _mediator.Send(command);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Delete Attempt {MyModuleId} {ModuleId}", id, moduleid);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        }
    }
}
