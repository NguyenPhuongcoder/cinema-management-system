using System.ComponentModel.DataAnnotations;

namespace QUANLYRAPCHIEUPHHIM.ViewModels
{
    public class CreateRoomViewModel
    {
        [Required(ErrorMessage = "Tên phòng là bắt buộc")]
        public string RoomName { get; set; } = null!;

        [Required(ErrorMessage = "Sức chứa là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Sức chứa phải lớn hơn 0")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Rạp chiếu là bắt buộc")]
        public int CinemaId { get; set; }

        [Required(ErrorMessage = "Định dạng phòng là bắt buộc")]
        public int FormatId { get; set; }
    }
}
