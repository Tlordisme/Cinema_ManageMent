using CM.ApplicationService.Common;
using CM.ApplicationService.Showtime.Abstracts;
using CM.Domain.Showtime;
using CM.Dtos.Showtime;
using CM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Showtime.Implements
{
    public class ShowtimeService : ServiceBase, IShowtimeService
    {
        public ShowtimeService(CMDbContext dbContext, ILogger<ServiceBase> logger)
            : base(logger, dbContext) { }

        public async Task<ShowtimeDto> CreateShowtimeAsync(CreateShowtimeDto createShowtimeDto)
        {
            _logger.LogInformation($"User is creating a new showtime.");

            var room = await _dbContext.Rooms.FindAsync(createShowtimeDto.RoomId);
            if (room == null)
                throw new Exception("Room not found");

            var movie = await _dbContext.Movies.FindAsync(createShowtimeDto.MovieId);
            if (movie == null)
                throw new Exception("Movie not found");

            var showtime = new CMShowtime
            {
                StartTime = createShowtimeDto.StartTime,
                EndTime = createShowtimeDto.EndTime,
                RoomID = createShowtimeDto.RoomId,
                MovieID = createShowtimeDto.MovieId,
            };

            _dbContext.Showtimes.Add(showtime);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Showtime with ID {showtime.Id} created successfully.");

            return new ShowtimeDto
            {
                Id = showtime.Id,
                StartTime = showtime.StartTime,
                EndTime = showtime.EndTime,
                RoomId = showtime.RoomID,
                MovieId = showtime.MovieID,
            };
        }

        public async Task<bool> DeleteShowtimeAsync(string showtimeId)
        {
            _logger.LogInformation($"User is attempting to delete showtime with ID {showtimeId}.");

            var showtime = await _dbContext.Showtimes.FindAsync(showtimeId);
            if (showtime == null)
                return false;

            _dbContext.Showtimes.Remove(showtime);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Showtime with ID {showtimeId} deleted successfully.");
            return true;
        }

        public async Task<List<ShowtimeDto>> GetAllShowtimesAsync()
        {
            _logger.LogInformation($"User is viewing all showtimes.");

            var showtimes = await _dbContext
                .Showtimes
                .Include(s => s.Room)
                .Include(s => s.Movie)
                .ToListAsync();

            return showtimes.Select(s => new ShowtimeDto
            {
                Id = s.Id,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                RoomId = s.RoomID,
                RoomName = s.Room?.Name,
                MovieId = s.MovieID,
                MovieTitle = s.Movie?.Title,
            }).ToList();
        }

        public async Task<ShowtimeDto> GetShowtimeByIdAsync(string showtimeId)
        {
            _logger.LogInformation($"User is viewing showtime with ID {showtimeId}.");

            var showtime = await _dbContext
                .Showtimes
                .Include(s => s.Room)
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(s => s.Id == showtimeId);

            if (showtime == null)
                throw new Exception("Showtime not found");

            return new ShowtimeDto
            {
                Id = showtime.Id,
                StartTime = showtime.StartTime,
                EndTime = showtime.EndTime,
                RoomId = showtime.RoomID,
                RoomName = showtime.Room.Name,
                MovieId = showtime.MovieID,
                MovieTitle = showtime.Movie.Title,
            };
        }

        public async Task<bool> UpdateShowtimeAsync(UpdateShowtimeDto updateShowtimeDto)
        {
            _logger.LogInformation($"User is updating showtime with ID {updateShowtimeDto.Id}.");

            var showtime = await _dbContext.Showtimes.FindAsync(updateShowtimeDto.Id);
            if (showtime == null)
                throw new Exception("Showtime not found");

            var room = await _dbContext.Rooms.FindAsync(updateShowtimeDto.RoomId);
            if (room == null)
                throw new Exception("Room not found");

            var movie = await _dbContext.Movies.FindAsync(updateShowtimeDto.MovieId);
            if (movie == null)
                throw new Exception("Movie not found");

            showtime.StartTime = updateShowtimeDto.StartTime;
            showtime.EndTime = updateShowtimeDto.EndTime;
            showtime.RoomID = updateShowtimeDto.RoomId;
            showtime.MovieID = updateShowtimeDto.MovieId;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Showtime with ID {updateShowtimeDto.Id} updated successfully.");
            return true;
        }
    }
}