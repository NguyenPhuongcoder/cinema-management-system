using QUANLYRAPCHIEUPHHIM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface ICinemaService
    {
        Task<IEnumerable<Cinema>> GetCinemasAsync(int page = 1, int pageSize = 10);
        Task<int> CountCinemasAsync();
        Task<Cinema?> GetCinemaByIdAsync(int id);
        Task<Cinema> CreateCinemaAsync(Cinema cinema);
        Task<Cinema> UpdateCinemaAsync(Cinema cinema);
        Task DeleteCinemaAsync(int id);
        Task<IEnumerable<Cinema>> GetCinemasByCityAsync(int cityId);
        Task<IEnumerable<Cinema>> GetCinemasAsync();
        Task<bool> CinemaExistsAsync(int id);
    }
} 