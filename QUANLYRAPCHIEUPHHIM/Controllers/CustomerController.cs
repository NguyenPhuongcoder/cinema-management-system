using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QUANLYRAPCHIEUPHHIM.Models;
using QUANLYRAPCHIEUPHHIM.Data;
using System.Security.Claims;

namespace QUANLYRAPCHIEUPHHIM.Controllers
{
    [Authorize(Policy = "RequireCustomerRole")]
    public class CustomerController : Controller
    {
        private readonly CinemaDbcontext _context;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(CinemaDbcontext context, ILogger<CustomerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                // Lấy ID của user hiện tại
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Index: {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> MyBookings()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var bookings = await _context.Bookings
                    .Include(b => b.Tickets)
                        .ThenInclude(t => t.Showtime)
                            .ThenInclude(s => s.Movie)
                    .Include(b => b.Tickets)
                        .ThenInclude(t => t.Seat)
                    .Include(b => b.BookingBookingStatuses)
                        .ThenInclude(bs => bs.BookingStatus)
                    .Where(b => b.UserId == userId)
                    .OrderByDescending(b => b.BookingDate)
                    .ToListAsync();

                return View(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in MyBookings: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> MyProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _context.Users.FindAsync(userId);
                
                if (user == null)
                {
                    _logger.LogWarning($"User not found: {userId}");
                    return RedirectToAction("Index", "Home");
                }

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in MyProfile: {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(User model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    var user = await _context.Users.FindAsync(userId);

                    if (user == null)
                    {
                        _logger.LogWarning($"User not found during update: {userId}");
                        return RedirectToAction("Index", "Home");
                    }

                    // Chỉ cập nhật các thông tin được phép
                    user.FullName = model.FullName;
                    user.Phone = model.Phone;
                    user.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Profile updated for user: {userId}");

                    TempData["SuccessMessage"] = "Profile updated successfully";
                    return RedirectToAction(nameof(MyProfile));
                }

                return View("MyProfile", model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating profile: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while updating your profile");
                return View("MyProfile", model);
            }
        }
    }
} 