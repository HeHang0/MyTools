using Microsoft.AspNetCore.Mvc;

namespace CoreWebDemo.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}