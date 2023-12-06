using Microsoft.AspNetCore.Mvc;
using WorkshopDemo.Core.Common;

namespace WorkshopDemo.Controllers;

[ApiController]
[Route("api/version")]
public class VersionController(IVersionService versionService) : ControllerBase
{
    [HttpGet]
    public string Get()
    {
        return versionService.GetVersion();
    }
}