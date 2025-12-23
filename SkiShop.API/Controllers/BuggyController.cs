using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkiShop.API.DTOs;

namespace SkiShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController : BaseApiController
    {
        [HttpGet("unauthorized")]
        public IActionResult GetUnauthorized()
        {
            return Unauthorized();

        }
        [HttpGet("badrequest")]
        public IActionResult GetBadRequest()
        {
           return BadRequest("This is a bad request example.");

        }
        [HttpGet("notFound")]
        public IActionResult  GetNotFound()
        {
            return NotFound();
        }
        
        
        [HttpGet("internalerror")]
        public IActionResult GetEnternalError()
        {
           throw new Exception("This is an internal server error example.");
        }
        [HttpPost("validationerror")]
        public IActionResult GetValidationError(CreateProductDto products)
        {
           return Ok ();

        }
       
    }
}
