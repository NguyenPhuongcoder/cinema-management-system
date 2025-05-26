using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QUANLYRAPCHIEUPHHIM.Models;
using QUANLYRAPCHIEUPHHIM.Data;
using QUANLYRAPCHIEUPHHIM.Services;
using QUANLYRAPCHIEUPHHIM.ViewModels;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;
using X.PagedList;
using X.PagedList.Extensions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace QUANLYRAPCHIEUPHHIM.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly CinemaDbcontext _context;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly SeatService _seatService;
        private const int PageSize = 10; // Number of items per page

        public AdminController(CinemaDbcontext context, ICloudinaryService cloudinaryService, SeatService seatService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _seatService = seatService;
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

        public IActionResult Movies(int? page)
        {
            var pageNumber = page ?? 1;
            var movies = _context.Movies.ToPagedList(pageNumber, PageSize);
            return View(movies);
        }

        // Create Movie
        public IActionResult CreateMovie()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMovie(Movie movie, IFormFile posterFile)
        {
            if (ModelState.IsValid)
            {
                // Validate base price
                if (movie.BasePrice < 0)
                {
                    ModelState.AddModelError("BasePrice", "Base price cannot be negative");
                    return View(movie);
                }

                // Validate duration
                if (movie.Duration <= 0)
                {
                    ModelState.AddModelError("Duration", "Duration must be greater than 0");
                    return View(movie);
                }

                // Validate release date
                if (movie.ReleaseDate < DateOnly.FromDateTime(DateTime.Now))
                {
                    ModelState.AddModelError("ReleaseDate", "Release date cannot be in the past");
                    return View(movie);
                }

                // Upload poster image if provided
                if (posterFile != null && posterFile.Length > 0)
                {
                    movie.PosterUrl = await _cloudinaryService.UploadImageAsync(posterFile);
                }

                movie.CreatedAt = DateTime.Now;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMovie(Movie movie, IFormFile posterFile)
        {
            if (ModelState.IsValid)
            {
                var existingMovie = _context.Movies.Find(movie.MovieId);
                if (existingMovie == null)
                {
                    return NotFound();
                }

                // Validate base price
                if (movie.BasePrice < 0)
                {
                    ModelState.AddModelError("BasePrice", "Base price cannot be negative");
                    return View(movie);
                }

                // Validate duration
                if (movie.Duration <= 0)
                {
                    ModelState.AddModelError("Duration", "Duration must be greater than 0");
                    return View(movie);
                }

                // Validate release date
                if (movie.ReleaseDate < DateOnly.FromDateTime(DateTime.Now))
                {
                    ModelState.AddModelError("ReleaseDate", "Release date cannot be in the past");
                    return View(movie);
                }

                // Upload new poster image if provided
                if (posterFile != null && posterFile.Length > 0)
                {
                    // Delete old poster from Cloudinary if exists
                    if (!string.IsNullOrEmpty(existingMovie.PosterUrl))
                    {
                        var publicId = existingMovie.PosterUrl.Split('/').Last().Split('.')[0];
                        await _cloudinaryService.DeleteImageAsync(publicId);
                    }

                    // Upload new poster
                    movie.PosterUrl = await _cloudinaryService.UploadImageAsync(posterFile);
                }
                else
                {
                    // Keep existing poster URL
                    movie.PosterUrl = existingMovie.PosterUrl;
                }

                movie.UpdatedAt = DateTime.Now;
                _context.Entry(existingMovie).CurrentValues.SetValues(movie);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMovieConfirmed(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie == null)
            {
                return NotFound();
            }

            // Check if movie has any associated showtimes
            var hasShowtimes = _context.Showtimes.Any(s => s.MovieId == id);
            if (hasShowtimes)
            {
                TempData["Error"] = "Cannot delete movie because it has associated showtimes.";
                return RedirectToAction(nameof(Movies));
            }

            // Delete poster from Cloudinary if exists
            if (!string.IsNullOrEmpty(movie.PosterUrl))
            {
                var publicId = movie.PosterUrl.Split('/').Last().Split('.')[0];
                await _cloudinaryService.DeleteImageAsync(publicId);
            }

            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Movies));
        }

        public IActionResult Shows(int? page)
        {
            var pageNumber = page ?? 1;
            var shows = _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .ToPagedList(pageNumber, PageSize);
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

        public IActionResult Bookings(int? page)
        {
            var pageNumber = page ?? 1;
            var bookings = _context.Bookings.ToPagedList(pageNumber, PageSize);
            var usernames = _context.Bookings
        .Join(_context.Users,
        booking => booking.UserId,
        user => user.UserId,
        (booking, user) => new { booking.UserId, user.Username })
        .Distinct() // Thêm dòng này để loại bỏ trùng lặp
        .ToDictionary(bu => bu.UserId, bu => bu.Username);
            var bookingStatuses = _context.BookingBookingStatuses
                .Include(b => b.BookingStatus)
                .ToList()
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

            // Xoá hết các dòng trạng thái cũ của booking này
            var oldStatuses = _context.BookingBookingStatuses
                                      .Where(b => b.BookingId == id)
                                      .ToList();
            _context.BookingBookingStatuses.RemoveRange(oldStatuses);

            // Thêm dòng trạng thái mới
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

        public IActionResult Users(int? page)
        {
            var pageNumber = page ?? 1;
            var users = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToPagedList(pageNumber, PageSize);
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
        public IActionResult EditUser(User user, int RoleId, string? NewPassword)
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

        public IActionResult Rooms(int? page)
        {
            var pageNumber = page ?? 1;
            var rooms = _context.Rooms
                .Include(r => r.Format)
                .Include(r => r.Cinema)
                .ToPagedList(pageNumber, PageSize);
            return View(rooms);
        }

        // Create Room
        public IActionResult CreateRoom()
        {
            ViewBag.RoomFormats = _context.RoomFormats.ToList();
            ViewBag.Cinema = _context.Cinemas.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateRoom(CreateRoomViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Validate room name uniqueness
                if (_context.Rooms.Any(r => r.RoomName == viewModel.RoomName))
                {
                    ModelState.AddModelError("RoomName", "Tên phòng đã tồn tại");
                    ViewBag.RoomFormats = _context.RoomFormats.ToList();
                    return View(viewModel);
                }

                // Map ViewModel to Room
                var room = new Room
                {
                    RoomName = viewModel.RoomName,
                    Capacity = viewModel.Capacity,
                    CinemaId = viewModel.CinemaId,
                    FormatId = viewModel.FormatId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Rooms.Add(room);
                _context.SaveChanges();
                return RedirectToAction(nameof(Rooms));
            }

            ViewBag.RoomFormats = _context.RoomFormats.ToList();
            return View(viewModel);
        }

        // Edit Room
        public IActionResult EditRoom(int id)
        {
            var room = _context.Rooms.Find(id);
          
            if (room == null)
            {
                return NotFound();
            }
            ViewBag.RoomFormats = _context.RoomFormats.ToList();
            ViewBag.Cinema = _context.Cinemas.ToList();
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditRoom(Room room)
        {

            if (ModelState.IsValid)
            {
                // Validate capacity
                if (room.Capacity <= 0)
                {
                    ModelState.AddModelError("Capacity", "Capacity must be greater than 0");
                    ViewBag.RoomFormats = _context.RoomFormats.ToList();
                    return View(room);
                }

                // Validate room name uniqueness (excluding current room)
                if (_context.Rooms.Any(r => r.RoomName == room.RoomName && r.RoomId != room.RoomId))
                {
                    ModelState.AddModelError("RoomName", "Room name already exists");
                    ViewBag.RoomFormats = _context.RoomFormats.ToList();
                    return View(room);
                }

                _context.Update(room);
                _context.SaveChanges();
                return RedirectToAction(nameof(Rooms));
            }
            ViewBag.RoomFormats = _context.RoomFormats.ToList();
            ViewBag.Cinema = _context.Cinemas.ToList();
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
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRoomConfirmed(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room == null)
            {
                return NotFound();
            }

            // Check if room has any associated showtimes
            var hasShowtimes = _context.Showtimes.Any(s => s.RoomId == id);
            if (hasShowtimes)
            {
                TempData["Error"] = "Cannot delete room because it has associated showtimes.";
                return RedirectToAction(nameof(Rooms));
            }

            _context.Rooms.Remove(room);
            _context.SaveChanges();
            return RedirectToAction(nameof(Rooms));
        }

        // Room Details
        public IActionResult RoomDetails(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        // Movie Details
        public IActionResult MovieDetails(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        public IActionResult Tickets(int? page)
        {
            var pageNumber = page ?? 1;
            var tickets = _context.Tickets
                .Include(t => t.Booking)
                .Include(t => t.Showtime)
                    .ThenInclude(s => s.Movie)
                .Include(t => t.Seat)
                .ToPagedList(pageNumber, PageSize);

            return View(tickets);
        }

        // Edit Ticket
        public IActionResult EditTicket(int id)
        {
            var ticket = _context.Tickets
                .FirstOrDefault(t => t.TicketId == id);

            if (ticket == null)
            {
                return NotFound();
            }

            ViewBag.Bookings = _context.Bookings.ToList();
            ViewBag.Showtimes = _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .ToList();
            ViewBag.Seats = _context.Seats.ToList();
            ViewBag.TicketStatuses = new List<string> { "Active", "Used", "Cancelled", "Expired" };

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTicket(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var existingTicket = _context.Tickets
                    .FirstOrDefault(t => t.TicketId == ticket.TicketId);

                if (existingTicket == null)
                {
                    return NotFound();
                }

                // Check if seat is already booked for this showtime (excluding current ticket)
                var seatBooked = _context.Tickets
                    .Any(t => t.ShowtimeId == ticket.ShowtimeId && 
                             t.SeatId == ticket.SeatId && 
                             t.TicketId != ticket.TicketId);
                
                if (seatBooked)
                {
                    ModelState.AddModelError("", "This seat is already booked for this showtime");
                    ViewBag.Bookings = _context.Bookings.ToList();
                    ViewBag.Showtimes = _context.Showtimes
                        .Include(s => s.Movie)
                        .Include(s => s.Room)
                        .ToList();
                    ViewBag.Seats = _context.Seats.ToList();
                    ViewBag.TicketStatuses = new List<string> { "Active", "Used", "Cancelled", "Expired" };
                    return View(ticket);
                }

                // Update ticket properties
                existingTicket.BookingId = ticket.BookingId;
                existingTicket.ShowtimeId = ticket.ShowtimeId;
                existingTicket.SeatId = ticket.SeatId;
                existingTicket.Price = ticket.Price;
                existingTicket.TicketStatus = ticket.TicketStatus;
                existingTicket.ScanDatetime = ticket.ScanDatetime;
                existingTicket.UpdatedAt = DateTime.Now;

                _context.Update(existingTicket);
                _context.SaveChanges();
                return RedirectToAction(nameof(Tickets));
            }
            ViewBag.Bookings = _context.Bookings.ToList();
            ViewBag.Showtimes = _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .ToList();
            ViewBag.Seats = _context.Seats.ToList();
            ViewBag.TicketStatuses = new List<string> { "Active", "Used", "Cancelled", "Expired" };
            return View(ticket);
        }

        // Delete Ticket
        public IActionResult DeleteTicket(int id)
        {
            var ticket = _context.Tickets
                .Include(t => t.Booking)
                .Include(t => t.Showtime)
                    .ThenInclude(s => s.Movie)
                .Include(t => t.Seat)
                .FirstOrDefault(t => t.TicketId == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        [HttpPost, ActionName("DeleteTicket")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTicketConfirmed(int id)
        {
            var ticket = _context.Tickets.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }

            _context.Tickets.Remove(ticket);
            _context.SaveChanges();
            return RedirectToAction(nameof(Tickets));
        }

        private string GenerateTicketCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var code = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            // Ensure uniqueness
            while (_context.Tickets.Any(t => t.TicketCode == code))
            {
                code = new string(Enumerable.Repeat(chars, 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }

            return code;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Seat Management
        public async Task<IActionResult> ManageSeats(int roomId)
        {
            var room = await _context.Rooms
                .Include(r => r.Seats)
                .ThenInclude(s => s.SeatType)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            if (room == null)
            {
                return NotFound();
            }

            var viewModel = new SeatViewModel
            {
                RoomId = room.RoomId,
                RoomName = room.RoomName,
                Capacity = room.Capacity,
                Seats = room.Seats.ToList(),
                AvailableSeatTypes = await _context.SeatTypes.ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateSeats(int roomId)
        {
            var result = await _seatService.GenerateSeatsForRoom(roomId);
            if (!result)
            {
                TempData["Error"] = "Không thể tạo ghế. Phòng có thể không tồn tại hoặc đã có ghế.";
                return RedirectToAction(nameof(ManageSeats), new { roomId });
            }

            TempData["Success"] = "Đã tạo ghế thành công.";
            return RedirectToAction(nameof(ManageSeats), new { roomId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSeatType(int seatId, int seatTypeId)
        {
            var result = await _seatService.UpdateSeatType(seatId, seatTypeId);
            if (!result)
            {
                TempData["Error"] = "Không thể cập nhật loại ghế. Ghế hoặc loại ghế có thể không tồn tại.";
                return RedirectToAction(nameof(ManageSeats), new { roomId = await GetRoomIdFromSeat(seatId) });
            }

            TempData["Success"] = "Đã cập nhật loại ghế thành công.";
            return RedirectToAction(nameof(ManageSeats), new { roomId = await GetRoomIdFromSeat(seatId) });
        }

        private async Task<int> GetRoomIdFromSeat(int seatId)
        {
            var seat = await _context.Seats.FindAsync(seatId);
            return seat?.RoomId ?? 0;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSeats(int roomId, string rowLetter, int seatsToAdd)
        {
            var room = await _context.Rooms
                .Include(r => r.Seats)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            if (room == null)
            {
                TempData["Error"] = "Không tìm thấy phòng.";
                return RedirectToAction(nameof(Rooms));
            }

            // Validate input
            if (string.IsNullOrEmpty(rowLetter) || rowLetter.Length != 1)
            {
                TempData["Error"] = "Hàng không hợp lệ.";
                return RedirectToAction(nameof(ManageSeats), new { roomId });
            }

            if (seatsToAdd <= 0 || seatsToAdd > 10)
            {
                TempData["Error"] = "Số ghế phải từ 1 đến 10.";
                return RedirectToAction(nameof(ManageSeats), new { roomId });
            }

            // Get default seat type
            var defaultSeatType = await _context.SeatTypes.FirstOrDefaultAsync();
            if (defaultSeatType == null)
            {
                TempData["Error"] = "Không tìm thấy loại ghế mặc định.";
                return RedirectToAction(nameof(ManageSeats), new { roomId });
            }

            // Get current seat count
            var currentSeatCount = room.Seats.Count;
            if (currentSeatCount + seatsToAdd > room.Capacity)
            {
                TempData["Error"] = $"Không thể thêm {seatsToAdd} ghế mới. Phòng chỉ còn {room.Capacity - currentSeatCount} chỗ trống.";
                return RedirectToAction(nameof(ManageSeats), new { roomId });
            }

            // Get existing seats in the selected row
            var existingSeatsInRow = room.Seats
                .Where(s => s.RowLetter == rowLetter)
                .OrderBy(s => s.SeatNumber)
                .ToList();

            // Calculate the next seat number
            int nextSeatNumber = 1;
            if (existingSeatsInRow.Any())
            {
                nextSeatNumber = existingSeatsInRow.Max(s => s.SeatNumber) + 1;
            }

            var newSeats = new List<Seat>();
            for (int i = 0; i < seatsToAdd; i++)
            {
                newSeats.Add(new Seat
                {
                    RoomId = roomId,
                    SeatTypeId = defaultSeatType.SeatTypeId,
                    RowLetter = rowLetter,
                    SeatNumber = nextSeatNumber + i,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await _context.Seats.AddRangeAsync(newSeats);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã thêm {seatsToAdd} ghế mới vào hàng {rowLetter}.";
            return RedirectToAction(nameof(ManageSeats), new { roomId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSeat(int seatId)
        {
            var seat = await _context.Seats
                .Include(s => s.Tickets)
                .FirstOrDefaultAsync(s => s.SeatId == seatId);

            if (seat == null)
            {
                TempData["Error"] = "Không tìm thấy ghế.";
                return RedirectToAction(nameof(ManageSeats), new { roomId = await GetRoomIdFromSeat(seatId) });
            }

            // Check if seat has any associated tickets
            if (seat.Tickets.Any())
            {
                TempData["Error"] = "Không thể xóa ghế vì đã có vé được đặt.";
                return RedirectToAction(nameof(ManageSeats), new { roomId = seat.RoomId });
            }

            _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xóa ghế thành công.";
            return RedirectToAction(nameof(ManageSeats), new { roomId = seat.RoomId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRow(int roomId, string rowLetter)
        {
            var room = await _context.Rooms
                .Include(r => r.Seats)
                .ThenInclude(s => s.Tickets)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            if (room == null)
            {
                TempData["Error"] = "Không tìm thấy phòng.";
                return RedirectToAction(nameof(Rooms));
            }

            var seatsInRow = room.Seats.Where(s => s.RowLetter == rowLetter).ToList();
            if (!seatsInRow.Any())
            {
                TempData["Error"] = "Không tìm thấy hàng ghế.";
                return RedirectToAction(nameof(ManageSeats), new { roomId });
            }

            // Check if any seat in the row has tickets
            var hasTickets = seatsInRow.Any(s => s.Tickets.Any());
            if (hasTickets)
            {
                TempData["Error"] = "Không thể xóa hàng vì có ghế đã được đặt vé.";
                return RedirectToAction(nameof(ManageSeats), new { roomId });
            }

            _context.Seats.RemoveRange(seatsInRow);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã xóa toàn bộ hàng {rowLetter} thành công.";
            return RedirectToAction(nameof(ManageSeats), new { roomId });
        }
    }
} 