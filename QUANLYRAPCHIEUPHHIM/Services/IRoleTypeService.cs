using QUANLYRAPCHIEUPHHIM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface IRoleTypeService
    {
        Task<IEnumerable<RoleType>> GetRoleTypesAsync(
            string? roleTypeName = null,
            int page = 1,
            int pageSize = 10);

        Task<int> CountRoleTypesAsync(string? roleTypeName = null);

        Task<RoleType> GetRoleTypeByIdAsync(int id);
        Task<RoleType> CreateRoleTypeAsync(RoleType roleType);
        Task<RoleType> UpdateRoleTypeAsync(RoleType roleType);
        Task<bool> DeleteRoleTypeAsync(int id);
    }
} 