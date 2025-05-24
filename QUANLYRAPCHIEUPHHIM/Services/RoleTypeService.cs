using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QUANLYRAPCHIEUPHHIM.Data;
using QUANLYRAPCHIEUPHHIM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public class RoleTypeService : IRoleTypeService
    {
        private readonly CinemaDbContext _context;
        private readonly ILogger<RoleTypeService> _logger;

        public RoleTypeService(CinemaDbContext context, ILogger<RoleTypeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<RoleType>> GetRoleTypesAsync(
            string? roleTypeName = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.RoleTypes
                    .Include(rt => rt.MovieCastRoleTypes)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(roleTypeName))
                {
                    query = query.Where(rt => rt.RoleTypeName.Contains(roleTypeName));
                }

                return await query
                    .OrderBy(rt => rt.RoleTypeName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role types");
                throw;
            }
        }

        public async Task<int> CountRoleTypesAsync(string? roleTypeName = null)
        {
            try
            {
                var query = _context.RoleTypes.AsQueryable();

                if (!string.IsNullOrEmpty(roleTypeName))
                {
                    query = query.Where(rt => rt.RoleTypeName.Contains(roleTypeName));
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting role types");
                throw;
            }
        }

        public async Task<RoleType> GetRoleTypeByIdAsync(int id)
        {
            try
            {
                return await _context.RoleTypes
                    .Include(rt => rt.MovieCastRoleTypes)
                    .FirstOrDefaultAsync(rt => rt.RoleTypeId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role type by id {RoleTypeId}", id);
                throw;
            }
        }

        public async Task<RoleType> CreateRoleTypeAsync(RoleType roleType)
        {
            try
            {
                // Validate role type data
                if (string.IsNullOrEmpty(roleType.RoleTypeName))
                    throw new ArgumentException("Role type name is required");

                // Check if role type name is unique
                var existingRoleType = await _context.RoleTypes
                    .FirstOrDefaultAsync(rt => rt.RoleTypeName == roleType.RoleTypeName);

                if (existingRoleType != null)
                    throw new ArgumentException("Role type with this name already exists");

                _context.RoleTypes.Add(roleType);
                await _context.SaveChangesAsync();
                return roleType;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role type");
                throw;
            }
        }

        public async Task<RoleType> UpdateRoleTypeAsync(RoleType roleType)
        {
            try
            {
                // Validate role type data
                if (string.IsNullOrEmpty(roleType.RoleTypeName))
                    throw new ArgumentException("Role type name is required");

                // Check if role type exists
                var existingRoleType = await _context.RoleTypes
                    .FirstOrDefaultAsync(rt => rt.RoleTypeId == roleType.RoleTypeId);

                if (existingRoleType == null)
                    throw new ArgumentException("Role type not found");

                // Check if role type name is unique
                var duplicateRoleType = await _context.RoleTypes
                    .FirstOrDefaultAsync(rt => rt.RoleTypeName == roleType.RoleTypeName && 
                                             rt.RoleTypeId != roleType.RoleTypeId);

                if (duplicateRoleType != null)
                    throw new ArgumentException("Role type with this name already exists");

                existingRoleType.RoleTypeName = roleType.RoleTypeName;
                await _context.SaveChangesAsync();
                return existingRoleType;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role type");
                throw;
            }
        }

        public async Task<bool> DeleteRoleTypeAsync(int id)
        {
            try
            {
                var roleType = await _context.RoleTypes
                    .Include(rt => rt.MovieCastRoleTypes)
                    .FirstOrDefaultAsync(rt => rt.RoleTypeId == id);

                if (roleType == null)
                    return false;

                // Check if role type is used by any movie cast
                if (roleType.MovieCastRoleTypes.Any())
                    throw new InvalidOperationException("Cannot delete role type that is used by movie casts");

                _context.RoleTypes.Remove(roleType);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role type");
                throw;
            }
        }
    }
} 