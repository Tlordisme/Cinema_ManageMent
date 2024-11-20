using CM.Domain.Seat;
using CM.Infrastructure;
using CM.Infrastructure.Repositories.SeatRepository.Abstracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Infrastructure.Repositories.SeatRepository.Implements
{
    public class SeatRepository : ISeatRepository
    {
        private readonly CMDbContext _dbContext;

        public SeatRepository(CMDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public CMSeat GetById(int id)
        {
            return _dbContext.Seats.FirstOrDefault(s => s.Id == id);
        }

        public IEnumerable<CMSeat> GetSeatsByShowtime(string showtimeId)
        {
            // Truy vấn ghế dựa trên ShowtimeId qua RoomID
            var seats = _dbContext.Seats
                .Include(s => s.Room) // Bao gồm Room để lấy thông tin liên quan
                .Where(s => s.Room.Id == _dbContext.Showtimes
                    .Where(st => st.Id == showtimeId)
                    .Select(st => st.RoomID)
                    .FirstOrDefault())
                .ToList();

            return seats;
        }

        public void Update(CMSeat seat)
        {
            _dbContext.Seats.Update(seat);
            _dbContext.SaveChanges();
        }
    }
}
