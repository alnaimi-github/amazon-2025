using System.Security.Claims;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

public class BuggyController : BaseApiController
{

   [HttpGet("unauthorized")]
    public ActionResult GetUnauthorized()
    {
        return Unauthorized();
    }
    
    [HttpGet("notfound")]
    public ActionResult GetNotFound()
    {
      return NotFound();
    }

    [HttpGet("internalerror")]
    public ActionResult GetServerError()
    {
      throw new Exception("This is a server error")!;
    }

    [HttpGet("badrequest")]
    public ActionResult GetBadRequest()
    {
        return BadRequest("This was not a good request");
    }

    [HttpPost("validationerror")]
    public ActionResult GetValidationError(CreateProductDto product)
    {
        return Ok();
    }

    [Authorize]
    [HttpGet("secret")]
    public IActionResult GetSecret()
    {
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Ok("Hello" + name + "with the id of" + id);
    }
}