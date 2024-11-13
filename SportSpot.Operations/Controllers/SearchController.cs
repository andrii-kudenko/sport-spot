using Microsoft.AspNetCore.Mvc;

namespace SportSpot.Operations.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
