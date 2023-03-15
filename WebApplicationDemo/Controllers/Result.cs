using Microsoft.AspNetCore.Mvc;
using WebApplicationDemo.Models;

namespace WebApplicationDemo.Controllers;

public class Result : Controller
{
    // GET
    public IActionResult Index(ResultModel rm)
    {
        return View(rm);
    }

    public IActionResult FirmaWeb(string url)
    {

        ViewData["url"] = url;

        return View();
    }
}