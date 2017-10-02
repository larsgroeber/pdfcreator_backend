using System.Linq;
using API.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [Route("/api/users")]
    public class UserController : Controller
    {
        private PDFCreatorContext _context;
        public UserController(PDFCreatorContext pdfCreatorContext)
        {
            _context = pdfCreatorContext;
        }

        // GET
        public IActionResult Index()
        {
            return Json(_context.Users.Include(_ => _.Role).ToList());
        }
    }
}