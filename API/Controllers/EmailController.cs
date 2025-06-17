using Microsoft.AspNetCore.Mvc;

namespace PropertyManagementAPI.API.Controllers
{
    public class EmailController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
