using QUANLYRAPCHIEUPHHIM.Models;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface IMovieService
    {
        Task<IEnumerable<Movie>> GetAllMoviesAsync();
        Task<Movie> GetMovieByIdAsync(int id);
        Task<IEnumerable<Movie>> GetMoviesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Movie>> GetActiveMoviesAsync();
        Task<IEnumerable<Movie>> SearchMoviesAsync(
            string? title = null,
            string? genre = null,
            string? format = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool? isActive = null
        );
        Task<Movie> CreateMovieAsync(Movie movie);
        Task<Movie> UpdateMovieAsync(Movie movie);
        Task<bool> DeleteMovieAsync(int id);
    }
} 