using CM.Domain.Seat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Infrastructure.Repositories.SeatRepository.Abstracts
{
    public interface ISeatRepository
    {
        CMSeat GetById(int id);
        IEnumerable<CMSeat> GetSeatsByShowtime(string showtimeId);
        void Update(CMSeat seat);

    }
}
