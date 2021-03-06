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
    private readonly WebBankingAppContext _context;

    public LoginController(WebBankingAppContext context) => _context = context;

    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string loginID, string password)
    {
        var login = await _context.Logins.FindAsync(loginID);
        if(login == null || string.IsNullOrEmpty(password) || !PBKDF2.Verify(login.PasswordHash, password))
        {
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
            return View();
        }
        if(login.Auth == Authorization.Unauthorized)
        {
            ModelState.AddModelError("LoginFailed", "Your account has been locked, please contact the admin");
            return View();
        }

        // Login customer.
        HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
        HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);

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
