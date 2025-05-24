using QUANLYRAPCHIEUPHHIM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface IMovieCastService
    {
        Task<IEnumerable<MovieCast>> GetMovieCastsAsync(
            int? movieId = null,
            int? personId = null,
            string? characterName = null,
            int page = 1,
            int pageSize = 10);

        Task<int> CountMovieCastsAsync(
            int? movieId = null,
            int? personId = null,
            string? characterName = null);

        Task<MovieCast> GetMovieCastByIdAsync(int id);
        Task<MovieCast> CreateMovieCastAsync(MovieCast movieCast);
        Task<MovieCast> UpdateMovieCastAsync(MovieCast movieCast);
        Task<bool> DeleteMovieCastAsync(int id);
        Task<IEnumerable<MovieCast>> GetMovieCastsByMovieAsync(int movieId);
        Task<IEnumerable<MovieCast>> GetMovieCastsByPersonAsync(int personId);
    }
} 