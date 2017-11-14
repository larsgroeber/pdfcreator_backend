using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace API.Controllers
{
    [Route("/api/v1/version")]
    public class VersionController : Controller
    {
        private readonly string _version;

        public VersionController(IConfiguration configuration)
        {
            _version = configuration["Version"];
        }

        public IActionResult Index()
        {
            return new JsonResult(new
            {
                Version = _version
            });
        }
    }
}