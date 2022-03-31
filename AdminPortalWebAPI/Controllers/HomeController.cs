using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;

namespace MvcMovie.Controllers;

public class HomeController : Controller
{
    // ReSharper disable once NotAccessedField.Local
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // login admin
    [HttpPost]
    public IActionResult Index(string login, string password)
    {

        if (!login.Equals("admin") || !password.Equals("admin"))
        {
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
            return View();
        }

        // Login Admin.
        HttpContext.Session.SetString("adminID", login);

        return RedirectToAction("Index", "Customers");
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
}
