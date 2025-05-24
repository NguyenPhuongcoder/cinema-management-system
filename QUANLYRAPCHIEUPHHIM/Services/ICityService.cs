using QUANLYRAPCHIEUPHHIM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface ICityService
    {
        Task<IEnumerable<City>> GetCitiesAsync(
            string? cityName = null,
            int? provinceId = null,
            int page = 1,
            int pageSize = 10);

        Task<int> CountCitiesAsync(
            string? cityName = null,
            int? provinceId = null);

        Task<City> GetCityByIdAsync(int id);
        Task<City> CreateCityAsync(City city);
        Task<City> UpdateCityAsync(City city);
        Task<bool> DeleteCityAsync(int id);
        Task<IEnumerable<City>> GetCitiesByProvinceAsync(int provinceId);
    }
} 