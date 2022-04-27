using Microsoft.AspNetCore.Mvc;
using WebBankingApp.Data;
using WebBankingApp.Models;
using SimpleHashing;

namespace WebBankingApp.Controllers;

// Bonus Material: Implement global authorisation check.
//[AllowAnonymous]
[Route("/Mcba/SecureLogin")]
public class LoginController : Controller
{
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string loginID, string password)
    {
        if(!loginID.Equals("admin") || !password.Equals("admin"))
        {
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
            return View();
        }

        HttpContext.Session.SetInt32("admin", 100);
        HttpContext.Session.SetString("admin", "admin");

        return RedirectToAction("Index", "Customer");
    }

    [Route("LogoutNow")]
    public IActionResult Logout()
    {
        // Logout customer.
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }
}
