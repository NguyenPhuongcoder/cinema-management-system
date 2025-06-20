using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QUANLYRAPCHIEUPHHIM.Data;
using QUANLYRAPCHIEUPHHIM.Models;
using QUANLYRAPCHIEUPHHIM.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace QUANLYRAPCHIEUPHHIM.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly CinemaDbcontext _context;

        public ProfileController(CinemaDbcontext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            // Lấy lịch sử đặt vé
            var bookings = await _context.Bookings
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Showtime)
                        .ThenInclude(s => s.Movie)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Showtime)
                        .ThenInclude(s => s.Room)
                            .ThenInclude(r => r.Cinema)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Seat)
                .Include(b => b.Discount)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            ViewBag.Bookings = bookings;
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string fullName, string email, string phone)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.FullName = fullName;
            user.Email = email;
            user.Phone = phone;

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật thông tin thành công!";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật thông tin.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> BookingDetails(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var booking = await _context.Bookings
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Showtime)
                        .ThenInclude(s => s.Movie)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Showtime)
                        .ThenInclude(s => s.Room)
                            .ThenInclude(r => r.Cinema)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Seat)
                .Include(b => b.Discount)
                .FirstOrDefaultAsync(b => b.BookingId == id && b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Profile/ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(int.Parse(userId));

            if (user == null)
            {
                return NotFound();
            }

            // Verify current password
            var currentPasswordHash = HashPassword(model.CurrentPassword);
            if (user.Password != currentPasswordHash)
            {
                ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không đúng");
                return View(model);
            }

            // Update password
            user.Password = HashPassword(model.NewPassword);
            user.UpdatedAt = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra. Vui lòng thử lại sau.";
                return View(model);
            }
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