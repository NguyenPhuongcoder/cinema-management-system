using QUANLYRAPCHIEUPHHIM.Models;
namespace QUANLYRAPCHIEUPHHIM.ViewModels;
using System.Collections.Generic;
public class HomePageViewModel
{
    public List<string> Banners { get; set; }
    public List<Movie> Movies { get; set; }
    public List<Province> Provinces { get; set; }
}