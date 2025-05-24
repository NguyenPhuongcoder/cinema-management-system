using QUANLYRAPCHIEUPHHIM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetRoomsAsync(
            int? cinemaId = null,
            int? formatId = null,
            int page = 1,
            int pageSize = 10);

        Task<int> CountRoomsAsync(
            int? cinemaId = null,
            int? formatId = null);

        Task<Room> GetRoomByIdAsync(int id);
        Task<Room> CreateRoomAsync(Room room);
        Task<Room> UpdateRoomAsync(Room room);
        Task<bool> DeleteRoomAsync(int id);
        Task<IEnumerable<Room>> GetRoomsByCinemaAsync(int cinemaId);
        Task<IEnumerable<Room>> GetRoomsByFormatAsync(int formatId);
    }
} 