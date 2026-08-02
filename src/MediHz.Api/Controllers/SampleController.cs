using Microsoft.AspNetCore.Mvc;

namespace MediHz.Api.Controllers;

// Example Controller to reference.
// TODO: Delete once not needed.
[Route("api/[controller]")]
[ApiController]
public class SampleController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { message = "Hello, world!" });
}
