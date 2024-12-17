using CM.Infrastructure;
using CM.ApplicationService.Revenue.Abstracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CM.ApplicationService.Revenue.Implements
{
    public class RevenueService : IRevenueService
    {
        private readonly CMDbContext _dbContext;
        private readonly ILogger<RevenueService> _logger;

        public RevenueService(CMDbContext dbContext, ILogger<RevenueService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // Thống kê doanh thu theo ngày
        public async Task<IEnumerable<dynamic>> GetRevenueByDateAsync(string date)
        {
            try
            {
                var query = _dbContext.Tickets.AsQueryable();

                // Nếu có ngày cụ thể, lọc theo ngày đó
                if (!string.IsNullOrEmpty(date))
                {
                    DateTime selectedDate;
                    if (DateTime.TryParse(date, out selectedDate))
                    {
                        query = query.Where(t => t.BookingDate.Date == selectedDate.Date);
                    }
                }

                var revenueData = await query
                    .GroupBy(t => t.BookingDate.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        TotalRevenue = g.Sum(t => t.TotalPrice)
                    })
                    .ToListAsync();

                _logger.LogInformation($"Revenue data for date {date}: {revenueData.Count} records retrieved.");
                return revenueData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving revenue by date.");
                throw; // Rethrow to be handled by controller
            }
        }

        // Thống kê doanh thu theo bộ phim
        public async Task<IEnumerable<dynamic>> GetRevenueByMovieAsync(int? movieId)
        {
            try
            {
                var query = _dbContext.Tickets.AsQueryable();

                // Nếu có movieId, lọc theo bộ phim đó
                if (movieId.HasValue)
                {
                    query = query.Where(t => t.Showtime.MovieID == movieId.Value);
                }

                var revenueData = await query
                    .GroupBy(t => t.Showtime.MovieID)
                    .Select(g => new
                    {
                        MovieId = g.Key,
                        TotalRevenue = g.Sum(t => t.TotalPrice)
                    })
                    .ToListAsync();

                _logger.LogInformation($"Revenue data for movie ID {movieId}: {revenueData.Count} records retrieved.");
                return revenueData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving revenue by movie.");
                throw; // Rethrow to be handled by controller
            }
        }
    }
}
