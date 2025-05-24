using QUANLYRAPCHIEUPHHIM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface IMoviePersonService
    {
        Task<IEnumerable<MoviePerson>> GetMoviePersonsAsync(
            string? fullName = null,
            string? nationality = null,
            int page = 1,
            int pageSize = 10);

        Task<int> CountMoviePersonsAsync(
            string? fullName = null,
            string? nationality = null);

        Task<MoviePerson> GetMoviePersonByIdAsync(int id);

        Task<MoviePerson> CreateMoviePersonAsync(MoviePerson person);

        Task<MoviePerson> UpdateMoviePersonAsync(MoviePerson person);

        Task<bool> DeleteMoviePersonAsync(int id);
    }
} 