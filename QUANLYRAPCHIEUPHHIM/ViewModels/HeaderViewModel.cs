namespace QUANLYRAPCHIEUPHHIM.ViewModels
{
    public class HeaderViewModel
    {
        public List<MovieHeaderViewModel> NowShowing { get; set; } = new();
        public List<MovieHeaderViewModel> ComingSoon { get; set; } = new();
        public List<MovieHeaderViewModel> ImaxMovies { get; set; } = new();
        public List<CinemaHeaderViewModel> Cinemas { get; set; } = new();

        public string? UserName { get; set; }
        public string AvatarUrl { get; set; } = "/images/default-avatar.png";
    }
}
