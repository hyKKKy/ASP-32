using System.Diagnostics;
using ASP_32.Models;
using ASP_32.Services.Kdf;
using Microsoft.AspNetCore.Mvc;

namespace ASP_32.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IKdfService _kdfService;

        public HomeController(ILogger<HomeController> logger, IKdfService kdfService)
        {
            _logger = logger;
            _kdfService = kdfService;
        }

        public IActionResult Index()
        {
            ViewData["dk"] = _kdfService.Dk("Admin", "4506C746-8FDD-4586-9BF4-95D6933C3B4F");
            return View();
        }

        public IActionResult Privacy()
        {
            
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
