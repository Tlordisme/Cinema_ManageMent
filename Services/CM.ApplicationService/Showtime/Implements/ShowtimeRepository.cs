using CM.ApplicationService.Showtime.Abstracts;
using CM.Domain.Showtime;
using CM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Showtime.Implements
{
    public class ShowtimeRepository : IShowtimeRepository
    {
        private readonly CMDbContext _dbContext;

        public ShowtimeRepository(CMDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CMShowtime> GetByIdAsync(string showtimeId)
        {
            return await _dbContext.Showtimes
                .Include(s => s.Room)
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(s => s.Id == showtimeId);
        }

        public async Task<List<CMShowtime>> GetAllAsync()
        {
            return await _dbContext.Showtimes
                .Include(s => s.Room)
                .Include(s => s.Movie)
                .ToListAsync();
        }

        public async Task AddAsync(CMShowtime showtime)
        {
            _dbContext.Showtimes.Add(showtime);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(CMShowtime showtime)
        {
            _dbContext.Showtimes.Update(showtime);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(CMShowtime showtime)
        {
            _dbContext.Showtimes.Remove(showtime);
            await _dbContext.SaveChangesAsync();
        }
    }
}