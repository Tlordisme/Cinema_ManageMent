using CM.Domain.Theater;
using CM.Dtos.Theater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Theater.Abstracts
{
    public interface IRoomService
    {
        string CreateRoom(RoomDto dto); 
        List<CMRoom> GetRoomsByTheaterId(string theaterId);
        void DeleteRoom(string roomId);
        void UpdateRoom(RoomDto dto);
    }
}
