using QUANLYRAPCHIEUPHHIM.Models;

namespace QUANLYRAPCHIEUPHHIM.Models.ViewModels
{
    public class HomePageViewModel
    {
        public List<Movie> NowShowing { get; set; }
        public List<Movie> ComingSoon { get; set; }
        public List<Movie> ImaxMovies { get; set; }
        public List<string> Banners { get; set; }
    }
}
