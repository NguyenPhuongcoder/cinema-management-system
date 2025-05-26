using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QUANLYRAPCHIEUPHHIM.Models;
using QUANLYRAPCHIEUPHHIM.Services;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Controllers
{
    // [Authorize(Roles = "Staff")]
    public class StaffTicketController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly ILogger<StaffTicketController> _logger;

        public StaffTicketController(ITicketService ticketService, ILogger<StaffTicketController> logger)
        {
            _ticketService = ticketService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTicketDetails(int ticketId)
        {
            try
            {
                var ticket = await _ticketService.GetTicketByIdAsync(ticketId);
                if (ticket == null)
                {
                    return NotFound("Không tìm thấy vé");
                }

                return PartialView("TicketDetails", ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin chi tiết vé");
                return StatusCode(500, "Có lỗi xảy ra khi lấy thông tin vé");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmTicket(int ticketId)
        {
            try
            {
                var ticket = await _ticketService.ConfirmTicketAsync(ticketId);
                return Json(new { success = true, ticket = ticket });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xác nhận vé");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi xác nhận vé" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelTicket(int ticketId)
        {
            try
            {
                var ticket = await _ticketService.CancelTicketAsync(ticketId);
                return Json(new { success = true, ticket = ticket });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hủy vé");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi hủy vé" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTicketByCode(string code)
        {
            try
            {
                var ticket = await _ticketService.GetTicketByCodeAsync(code);
                if (ticket == null)
                {
                    return NotFound("Không tìm thấy vé");
                }

                return Json(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm vé theo mã");
                return StatusCode(500, "Có lỗi xảy ra khi tìm vé");
            }
        }

        public IActionResult ManageTicket()
        {
            return View();
        }
    }
} 