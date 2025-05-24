using QUANLYRAPCHIEUPHHIM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface IGenreService
    {
        Task<IEnumerable<Genre>> GetGenresAsync(
            string? genreName = null,
            int page = 1,
            int pageSize = 10);

        Task<int> CountGenresAsync(string? genreName = null);

        Task<Genre> GetGenreByIdAsync(int id);

        Task<Genre> CreateGenreAsync(Genre genre);

        Task<Genre> UpdateGenreAsync(Genre genre);

        Task<bool> DeleteGenreAsync(int id);
    }
} 