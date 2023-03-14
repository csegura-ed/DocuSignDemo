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
}