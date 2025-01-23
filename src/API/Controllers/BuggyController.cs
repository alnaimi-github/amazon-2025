using API.DTOs;

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
      throw new Exception("This is a server error");
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
}