using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkiShop.API.DTOs;
using System.Security.Claims;

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
        public IActionResult GetNotFound()
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
            return Ok();

        }
        [Authorize]
        [HttpGet("secret")]

        public IActionResult GetSecret()
        {
            var name=User.FindFirst(ClaimTypes.Name)?.Value;
            var id =User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok("Hello " + name + " with the id of " + id);

        }
    }
}
