using System.Collections.Generic;
using QUANLYRAPCHIEUPHHIM.Models;

namespace QUANLYRAPCHIEUPHHIM.ViewModels
{
    public class MovieLibraryViewModel
    {
        public List<Movie> Movies { get; set; }
        public List<Genre> Genres { get; set; }
        public List<int> Years { get; set; }
        public string SelectedGenre { get; set; }
        public int? SelectedYear { get; set; }
        public string SelectedStatus { get; set; } // nowshowing, comingsoon
        public string SelectedSort { get; set; } // mostviewed, newest
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }
} 