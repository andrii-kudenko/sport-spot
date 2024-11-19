// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using SportSpot.Entities.Models;
using SportSpot.Services.Interfaces;

namespace SportSpot.Operations.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserInterface _userInterface;

        public AuthController(IUserInterface userInterface)
        {
            _userInterface = userInterface;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Event");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            Console.WriteLine($"Login attempt started for email: {email}");

            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Email or password is empty");
                    ModelState.AddModelError("", "Email and password are required");
                    return View();
                }

                var user = await _userInterface.GetUserByEmailAsync(email);
                Console.WriteLine($"User found: {user != null}");

                if (user == null)
                {
                    Console.WriteLine("User not found");
                    ModelState.AddModelError("", "Invalid email or password");
                    return View();
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
                Console.WriteLine($"Password verification result: {isPasswordValid}");

                if (isPasswordValid)
                {
                    Console.WriteLine("Password verified, setting session");
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("UserName", user.Name);
                    Console.WriteLine($"Session set - UserId: {user.Id}, UserName: {user.Name}");

                    return RedirectToAction("Index", "Event");
                }

                Console.WriteLine("Invalid password");
                ModelState.AddModelError("", "Invalid email or password");
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", "An error occurred during login.");
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user, string password)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingUser = await _userInterface.GetUserByEmailAsync(user.Email);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("", "Email already registered");
                        return View(user);
                    }

                    // Hash password before saving
                    user.Password = HashPassword(password);
                    await _userInterface.CreateUserAsync(user);

                    return RedirectToAction(nameof(Login));
                }
                return View(user);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred during registration.");
                return View(user);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(User user, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, user.Password);
        }
    }
}