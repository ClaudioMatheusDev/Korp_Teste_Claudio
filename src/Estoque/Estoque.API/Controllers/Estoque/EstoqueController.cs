using Microsoft.AspNetCore.Mvc;

namespace Estoque.API.Controllers.Estoque
{
    public class EstoqueController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
