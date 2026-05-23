using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using prova_02.Models;
using Microsoft.AspNetCore.Authorization; 

namespace prova_02.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
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

    [Authorize] // 👈 só entra se estiver logado!
    public class Home : Controller
    {
        public IActionResult Index() => View();
    }
    
}
