using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using QUANLYRAPCHIEUPHHIM.Models;
using QUANLYRAPCHIEUPHHIM.Data;
using System.Security.Cryptography;
using System.Text;
using QUANLYRAPCHIEUPHHIM.ViewModels;
using RegisterViewModel = QUANLYRAPCHIEUPHHIM.Models.RegisterViewModel;

namespace QUANLYRAPCHIEUPHHIM.Controllers
{
    public class AccountController : Controller
    {
        private readonly CinemaDbcontext _context;

        public AccountController(CinemaDbcontext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid email or password");
                    return View(model);
                }

                // Verify password
                var hashedPassword = HashPassword(model.Password);
                if (user.Password != hashedPassword)
                {
                    ModelState.AddModelError("", "Invalid email or password");
                    return View(model);
                }

                // Create claims
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.Username),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new("FullName", user.FullName)
                };

                // Add role claims
                var userRoles = _context.UserRoles.Where(ur => ur.UserId == user.UserId)
                                                .Select(ur => ur.Role.RoleName);
                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : null
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                if (userRoles.Contains("Admin"))
                {
                    return RedirectToAction("dashboard", "Admin");
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (_context.Users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                    return View(model);
                }

                // Check if email already exists
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                    return View(model);
                }

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = HashPassword(model.Password),
                    FullName = model.FullName,
                    Phone = model.Phone,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Add user role
                var userRole = new UserRole
                {
                    UserId = user.UserId,
                    RoleId = 3 // Assuming 3 is the ID for "User" role
                };

                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}