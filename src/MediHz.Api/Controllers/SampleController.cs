using MediHz.Application.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace MediHz.Api.Controllers;

// Example Controller to reference.
// TODO: Delete once not needed.
[Route("api/[controller]")]
[ApiController]
public class SampleController(ISampleHandler sampleHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var request = new SampleRequest("SampleName");
        return Ok(await sampleHandler.Handle(request));
    }
}
