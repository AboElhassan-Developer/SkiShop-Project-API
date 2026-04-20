using Microsoft.AspNetCore.Mvc;

namespace SkiShop.API.Controllers
{
    public class FallbackController : Controller
    {
        [HttpGet("{*path:nonfile}")]
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot", "index.html"), "text/HTML");
        }
    }
}