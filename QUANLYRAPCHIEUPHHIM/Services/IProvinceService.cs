using QUANLYRAPCHIEUPHHIM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface IProvinceService
    {
        Task<IEnumerable<Province>> GetProvincesAsync(
            string? provinceName = null,
            int page = 1,
            int pageSize = 10);

        Task<int> CountProvincesAsync(string? provinceName = null);

        Task<Province> GetProvinceByIdAsync(int id);
        Task<Province> CreateProvinceAsync(Province province);
        Task<Province> UpdateProvinceAsync(Province province);
        Task<bool> DeleteProvinceAsync(int id);
    }
} 