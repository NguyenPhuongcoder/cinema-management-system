using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QUANLYRAPCHIEUPHHIM.Models;
using QUANLYRAPCHIEUPHHIM.Data;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;


namespace QUANLYRAPCHIEUPHHIM.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly CinemaDbContext _context;

        public AdminController(CinemaDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            // Lấy thống kê cơ bản
            var totalMovies = _context.Movies.Count();
            var totalShows = _context.Showtimes.Count();
            var totalBookings = _context.Bookings.Count();
            var totalUsers = _context.Users.Count();

            ViewBag.TotalMovies = totalMovies;
            ViewBag.TotalShows = totalShows;
            ViewBag.TotalBookings = totalBookings;
            ViewBag.TotalUsers = totalUsers;

            return View();
        }

        public IActionResult Movies()
        {
            var movies = _context.Movies.ToList();
            return View(movies);
        }

        // Create Movie
        public IActionResult CreateMovie()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateMovie(Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Movies.Add(movie);
                _context.SaveChanges();
                return RedirectToAction(nameof(Movies));
            }
            return View(movie);
        }

        // Edit Movie
        public IActionResult EditMovie(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        [HttpPost]
        public IActionResult EditMovie(Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Update(movie);
                _context.SaveChanges();
                return RedirectToAction(nameof(Movies));
            }
            return View(movie);
        }

        // Delete Movie
        public IActionResult DeleteMovie(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        [HttpPost, ActionName("DeleteMovie")]
        public IActionResult DeleteMovieConfirmed(int id)
        {
            var movie = _context.Movies.Find(id);
            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Movies));
        }

        public IActionResult Shows()
        {
            var shows = _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .ToList();
            return View(shows);
        }

        // Create Show
        public IActionResult CreateShow()
        {
            ViewBag.Movies = _context.Movies.Select(m => new { m.MovieId, m.Title, m.Duration }).ToList();
            ViewBag.Rooms = _context.Rooms.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult CreateShow(Showtime show)

        {
            if (ModelState.IsValid)
            {

                var movie = _context.Movies.FirstOrDefault(m => m.MovieId == show.MovieId);
                if (movie == null)
                {
                    ModelState.AddModelError("", "Selected movie does not exist");
                    ViewBag.Movies = _context.Movies.Select(m => new { m.MovieId, m.Title, m.Duration }).ToList();
                    ViewBag.Rooms = _context.Rooms.ToList();
                    return View(show);
                }
                show.EndTime = show.StartTime.AddMinutes(movie.Duration);
                // Check for room availability
                var conflictingShow = _context.Showtimes
                    .Where(s => s.RoomId == show.RoomId &&
                               ((show.StartTime >= s.StartTime && show.StartTime < s.EndTime) ||
                                (show.EndTime > s.StartTime && show.EndTime <= s.EndTime) ||
                                (show.StartTime <= s.StartTime && show.EndTime >= s.EndTime)))
                    .FirstOrDefault();

                if (conflictingShow != null)
                {
                    ModelState.AddModelError("", "The room is already booked for this time period");
                    ViewBag.Movies = _context.Movies.Select(m => new { m.MovieId, m.Title, m.Duration }).ToList();
                    ViewBag.Rooms = _context.Rooms.ToList();
                    return View(show);
                }

                _context.Showtimes.Add(show);
                _context.SaveChanges();
                return RedirectToAction(nameof(Shows));
            }
            ViewBag.Movies = _context.Movies.Select(m => new { m.MovieId, m.Title, m.Duration }).ToList();
            ViewBag.Rooms = _context.Rooms.ToList();
            return View(show);
        }

        // Edit Show
        public IActionResult EditShow(int id)
        {
            var show = _context.Showtimes.Find(id);
            if (show == null)
            {
                return NotFound();
            }
            ViewBag.Movies = _context.Movies.Select(m => new { m.MovieId, m.Title, m.Duration }).ToList();
            ViewBag.Rooms = _context.Rooms.ToList();
            return View(show);
        }

        [HttpPost]
        public IActionResult EditShow(Showtime show)
        {
            if (ModelState.IsValid)
            {
                var movie = _context.Movies.FirstOrDefault(m => m.MovieId == show.MovieId);
                if (movie == null)
                {
                    ModelState.AddModelError("", "Selected movie does not exist");
                    ViewBag.Movies = _context.Movies.Select(m => new { m.MovieId, m.Title, m.Duration }).ToList();
                    ViewBag.Rooms = _context.Rooms.ToList();
                    return View(show);
                }
                show.EndTime = show.StartTime.AddMinutes(movie.Duration);
                // Validate that end time is after start time
                if (show.EndTime <= show.StartTime)
                {
                    ModelState.AddModelError("EndTime", "End time must be after start time");
                    ViewBag.Movies = _context.Movies.Select(m => new { m.MovieId, m.Title, m.Duration }).ToList();
                    ViewBag.Rooms = _context.Rooms.ToList();
                    return View(show);
                }

                // Check for room availability (excluding current show)
                var conflictingShow = _context.Showtimes
                    .Where(s => s.RoomId == show.RoomId &&
                               s.ShowtimeId != show.ShowtimeId &&
                               ((show.StartTime >= s.StartTime && show.StartTime < s.EndTime) ||
                                (show.EndTime > s.StartTime && show.EndTime <= s.EndTime) ||
                                (show.StartTime <= s.StartTime && show.EndTime >= s.EndTime)))
                    .FirstOrDefault();

                if (conflictingShow != null)
                {
                    ModelState.AddModelError("", "The room is already booked for this time period");
                    ViewBag.Movies = _context.Movies.Select(m => new { m.MovieId, m.Title, m.Duration }).ToList();
                    ViewBag.Rooms = _context.Rooms.ToList();
                    return View(show);
                }

                _context.Update(show);
                _context.SaveChanges();
                return RedirectToAction(nameof(Shows));
            }
            ViewBag.Movies = _context.Movies.Select(m => new { m.MovieId, m.Title, m.Duration }).ToList();
            ViewBag.Rooms = _context.Rooms.ToList();
            return View(show);
        }

        // Delete Show
        public IActionResult DeleteShow(int id)
        {
            var show = _context.Showtimes.Find(id);
            if (show == null)
            {
                return NotFound();
            }
            ViewBag.MovieTitle = _context.Movies.Find(show.MovieId)?.Title;
            ViewBag.RoomName = _context.Rooms.Find(show.RoomId)?.RoomName;
            return View(show);
        }

        [HttpPost, ActionName("DeleteShow")]
        public IActionResult DeleteShowConfirmed(int id)
        {
            var show = _context.Showtimes.Find(id);
            _context.Showtimes.Remove(show);
            _context.SaveChanges();
            return RedirectToAction(nameof(Shows));
        }

        public IActionResult Bookings()
        {
            var bookings = _context.Bookings.ToList();
            var usernames = _context.Bookings
              .Join(_context.Users,
              booking => booking.UserId,
              user => user.UserId,
              (booking, user) => new { booking.UserId, user.Username })
               .ToDictionary(bu => bu.UserId, bu => bu.Username);
            var bookingStatuses = _context.BookingBookingStatuses
                .Include(b => b.BookingStatus)
                .ToList() // Thực thi query tại đây
                .GroupBy(b => b.BookingId)
                .Select(g => g
                .OrderByDescending(e => e.BookingBookingStatusId)
                .FirstOrDefault())
                 .Where(e => e != null && e.BookingStatus != null)
                 .ToList();
            ViewBag.BookingStatuses = bookingStatuses;
            ViewBag.Usernames = usernames;
            return View(bookings);
        }


        // Edit Booking
        public IActionResult EditBooking(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewBag.BookingStatuses = _context.BookingStatuses.ToList();
            return View(booking);
        }

        [HttpPost]
        public IActionResult EditBooking(int id, int BookingStatusId)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                return NotFound();
            }

            // Create new booking status entry
            var bookingStatus = new BookingBookingStatus
            {
                BookingId = id,
                BookingStatusId = BookingStatusId
            };

            _context.BookingBookingStatuses.Add(bookingStatus);
            _context.SaveChanges();
            
            return RedirectToAction(nameof(Bookings));
        }

        // Delete Booking
        public IActionResult DeleteBooking(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewBag.Username = _context.Users.Find(booking.UserId)?.Username;
            // Lấy trạng thái hiện tại (mới nhất) của booking
            var latestStatus = _context.BookingBookingStatuses
                .Where(bbs => bbs.BookingId == booking.BookingId)
                .OrderByDescending(bbs => bbs.BookingBookingStatusId)
                .Select(bbs => bbs.BookingStatus.BookingStatusName)
                .FirstOrDefault();
            ViewBag.BookingStatusName = latestStatus;
            return View(booking);
        }

        [HttpPost, ActionName("DeleteBooking")]
        public IActionResult DeleteBookingConfirmed(int id)
        {
            var booking = _context.Bookings.Find(id);
            _context.Bookings.Remove(booking);
            _context.SaveChanges();
            return RedirectToAction(nameof(Bookings));
        }

        public IActionResult Users()
        {
            var users = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToList();
            return View(users);
        }

        // Create User
        public IActionResult CreateUser()
        {
            ViewBag.Roles = _context.Roles.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult CreateUser(User user, int RoleId)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (_context.Users.Any(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists");
                    ViewBag.Roles = _context.Roles.ToList();
                    return View(user);
                }

                // Check if email already exists
                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    ViewBag.Roles = _context.Roles.ToList();
                    return View(user);
                }

                // Hash password
                user.Password = HashPassword(user.Password);
                user.CreatedAt = DateTime.Now;

                _context.Users.Add(user);
                _context.SaveChanges();

                // Add user role
                var userRole = new UserRole
                {
                    UserId = user.UserId,
                    RoleId = RoleId
                };
                _context.UserRoles.Add(userRole);
                _context.SaveChanges();

                return RedirectToAction(nameof(Users));
            }
            ViewBag.Roles = _context.Roles.ToList();
            return View(user);
        }

        // Edit User
        public IActionResult EditUser(int id)
        {
            var user = _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefault(u => u.UserId == id);
            
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.Roles = _context.Roles.ToList();
            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(User user, int RoleId, string NewPassword)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.Users
                    .Include(u => u.UserRoles)
                    .FirstOrDefault(u => u.UserId == user.UserId);

                if (existingUser == null)
                {
                    return NotFound();
                }

                // Check if username is changed and already exists
                if (existingUser.Username != user.Username && 
                    _context.Users.Any(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists");
                    ViewBag.Roles = _context.Roles.ToList();
                    return View(user);
                }

                // Check if email is changed and already exists
                if (existingUser.Email != user.Email && 
                    _context.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    ViewBag.Roles = _context.Roles.ToList();
                    return View(user);
                }

                // Update user properties
                existingUser.Username = user.Username;
                existingUser.Email = user.Email;
                existingUser.FullName = user.FullName;
                existingUser.Phone = user.Phone;
                existingUser.UpdatedAt = DateTime.Now;

                // Update password if provided
                if (!string.IsNullOrEmpty(NewPassword))
                {
                    existingUser.Password = HashPassword(NewPassword);
                }

                // Update role
                var currentRole = existingUser.UserRoles.FirstOrDefault();
                if (currentRole != null)
                {
                    if (currentRole.RoleId != RoleId)
                    {
                        _context.UserRoles.Remove(currentRole);
                        _context.UserRoles.Add(new UserRole
                        {
                            UserId = user.UserId,
                            RoleId = RoleId
                        });
                    }
                }
                else
                {
                    _context.UserRoles.Add(new UserRole
                    {
                        UserId = user.UserId,
                        RoleId = RoleId
                    });
                }

                _context.SaveChanges();
                return RedirectToAction(nameof(Users));
            }
            ViewBag.Roles = _context.Roles.ToList();
            return View(user);
        }

        // Delete User
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("DeleteUser")]
        public IActionResult DeleteUserConfirmed(int id)
        {
            var user = _context.Users.Find(id);
            _context.Users.Remove(user);
            _context.SaveChanges();
            return RedirectToAction(nameof(Users));
        }

        public IActionResult Rooms()
        {
            var rooms = _context.Rooms.ToList();
            return View(rooms);
        }

        // Create Room
        public IActionResult CreateRoom()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateRoom(Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Rooms.Add(room);
                _context.SaveChanges();
                return RedirectToAction(nameof(Rooms));
            }
            return View(room);
        }

        // Edit Room
        public IActionResult EditRoom(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        [HttpPost]
        public IActionResult EditRoom(Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Update(room);
                _context.SaveChanges();
                return RedirectToAction(nameof(Rooms));
            }
            return View(room);
        }

        // Delete Room
        public IActionResult DeleteRoom(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        [HttpPost, ActionName("DeleteRoom")]
        public IActionResult DeleteRoomConfirmed(int id)
        {
            var room = _context.Rooms.Find(id);
            _context.Rooms.Remove(room);
            _context.SaveChanges();
            return RedirectToAction(nameof(Rooms));
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