using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CM.Domain.Seat;
using CM.Dtos.Seat;

namespace CM.ApplicationService.Seat.Abstracts
{
    public interface ISeatService
    {
        List<SeatDto> GetSeatsByRoomId(string roomId);

        void LinkDoubleSeat(int seatId, int doubleSeatId);
        void AddSeat(AddSeatDto seatDto);
        void UpdateSeat(UpdateSeatDto seatDto);
        void DeleteSeat(int seatId);
        bool IsDoubleSeatValid(int x, int y, CMSeat doubleSeat);
        bool IsDoubleSeat(string seatType);
    }
}