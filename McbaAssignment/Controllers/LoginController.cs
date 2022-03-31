using Microsoft.AspNetCore.Mvc;
using McbaExample.Data;
using McbaExample.Models;
using McbaExampleWithLogin.Filters;
using SimpleHashing;
using Microsoft.EntityFrameworkCore;

namespace McbaExample.Controllers;

// Bonus Material: Implement global authorisation check.
//[AllowAnonymous]
[Route("/Mcba/SecureLogin")]
public class LoginController : Controller
{
    private readonly McbaContext _context;

    public LoginController(McbaContext context) => _context = context;

    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string loginID, string password)
    {
        var login = await _context.Logins.FindAsync(loginID);
        if(login == null || string.IsNullOrEmpty(password) || !PBKDF2.Verify(login.PasswordHash, password))
        { 
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
            return View(new Login { LoginID = loginID });
        }

        if (login.LoginStatus == LoginStatus.Locked)
        {
            ModelState.AddModelError("LoginFailed", "Login failed, this account has been locked.");
            return View(new Login { LoginID = loginID });
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

    [Route("ChangePassword")]
    [AuthorizeCustomer]
    public async Task<IActionResult> ChangePassword(string id)
    {
        return View(await _context.Logins.FindAsync(id));
    }

    [Route("ChangePassword")]
    [AuthorizeCustomer]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(string loginID, string password)
    {
        var hashedPassword = PBKDF2.Hash(password);
        var login = _context.Logins.Find(loginID);
        if (ModelState.IsValid)
        {
            try
            {
                login.PasswordHash = hashedPassword;
                _context.Update(login);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoginExists(login.LoginID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("MyProfile", "Customer");
        }
        return View(login);
    }

    // method to check a login exists
    private bool LoginExists(string id)
    {
        return _context.Logins.Any(e => e.LoginID == id);
    }
}
