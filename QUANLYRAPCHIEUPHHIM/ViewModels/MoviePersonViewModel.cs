using QUANLYRAPCHIEUPHHIM.Models;

namespace QUANLYRAPCHIEUPHHIM.ViewModels
{
    public class MoviePersonViewModel
    {
        public List<MoviePerson> People { get; set; } = new List<MoviePerson>();
        public List<string> Nationalities { get; set; } = new List<string>();
        public string SelectedNationality { get; set; }
        public string SelectedSort { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }
} 