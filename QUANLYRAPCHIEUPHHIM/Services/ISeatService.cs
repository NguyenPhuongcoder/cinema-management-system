using QUANLYRAPCHIEUPHHIM.Models;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface ISeatService
    {
        IEnumerable<Seat> GetSeatsByRoom(int roomId);
        IEnumerable<Seat> GetSeatsByIds(List<int> seatIds);
    }
} 