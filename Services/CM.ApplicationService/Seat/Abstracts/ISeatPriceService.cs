using CM.Dtos.Seat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Seat.Abstracts
{
    public interface ISeatPriceService
    {
        void AddSeatPrice(AddSeatPriceDto seatPriceDto);
        void UpdateSeatPrice(UpdateSeatPriceDto seatPriceDto);
        void DeleteSeatPrice(int seatPriceId);
        List<SeatPriceDto> GetSeatPricesByRoomId(string roomId);
        SeatPriceDto GetSeatPrice(int seatPriceId);
    }
}