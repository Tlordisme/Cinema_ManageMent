using CM.ApplicationService.Revenue.Abstracts;
using CM.Dtos.Revenue;
using CM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Revenue.Implements
{
    public class RevenueService : IRevenueService
    {
        private readonly CMDbContext _dbContext;

        public RevenueService(CMDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Thống kê doanh thu theo ngày
        public async Task<List<RevenueDto>> GetRevenueByDateAsync()
        {
            var revenueData = await _dbContext.Tickets
                .Include(t => t.Showtime)  
                .GroupBy(t => t.Showtime.StartTime.Date)  
                .Select(g => new RevenueDto
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    TotalRevenue = g.Sum(t => t.TotalPrice)
                })
                .ToListAsync();

            return revenueData;
        }

        public async Task<List<MovieRevenueDto>> GetRevenueByMovieAsync()
        {
            var movieRevenueData = await _dbContext.Tickets
                .Include(t => t.Showtime)  
                .ThenInclude(s => s.Movie) 
                .GroupBy(t => t.Showtime.MovieID) 
                .Select(g => new MovieRevenueDto
                {
                    MovieTitle = g.FirstOrDefault().Showtime.Movie.Title, 
                    TotalRevenue = g.Sum(t => t.TotalPrice)
                })
                .ToListAsync();

            return movieRevenueData;
        }
    }

}
