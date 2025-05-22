using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QUANLYRAPCHIEUPHHIM.ViewModels
{
    public class HeaderViewModel
    {
        public List<MovieHeaderViewModel> NowShowing { get; set; } = new();
        public List<MovieHeaderViewModel> ComingSoon { get; set; } = new();
        public List<MovieHeaderViewModel> ImaxMovies { get; set; } = new();
        public List<CinemaHeaderViewModel> Cinemas { get; set; } = new();

        [StringLength(100, ErrorMessage = "Tên người dùng không được vượt quá 100 ký tự")]
        public string? UserName { get; set; }
        public string AvatarUrl { get; set; } = "/images/default-avatar.png";
    }
}
