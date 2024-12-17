using CM.ApplicationService.Common;
using CM.ApplicationService.Theater.Abstracts;
using CM.Domain.Theater;
using CM.Dtos.Theater;
using CM.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Theater.Implements
{
    public class TheaterService : ServiceBase, ITheaterService
    {
        private readonly ILogger<TheaterService> _logger;

        public TheaterService(
            CMDbContext dbContext,
            ILogger<TheaterService> logger
        )
            : base(logger, dbContext)
        {
            _logger = logger;
        }

        public string CreateTheater(TheaterDto dto)
        {
            try
            {
                _logger.LogInformation("Starting to create theater with name {TheaterName}.", dto.Name);

                var theaterChain = _dbContext.TheaterChains.Find(dto.ChainId);
                if (theaterChain == null)
                    throw new Exception("TheaterChain không tồn tại.");
                if (theaterChain.Theaters == null)
                    theaterChain.Theaters = new List<CMTheater>();

                var theater = new CMTheater
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Location = dto.Location,
                    ChainId = dto.ChainId
                };
                theaterChain.Theaters.Add(theater);

                _dbContext.Theaters.Add(theater);
                _dbContext.SaveChanges();

                _logger.LogInformation("Theater created successfully with ID {TheaterId}.", theater.Id);
                return theater.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating theater.");
                throw;
            }
        }

        public List<CMTheater> GetTheatersByChainId(string chainId)
        {
            try
            {
                _logger.LogInformation("Retrieving theaters for chain ID {ChainId}.", chainId);

                var theaters = _dbContext.Theaters.Where(t => t.ChainId == chainId).ToList();
                _logger.LogInformation("Retrieved {TheaterCount} theaters for chain {ChainId}.", theaters.Count(), chainId);
                return theaters;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving theaters for chain {ChainId}.", chainId);
                throw;
            }
        }

        public void DeleteTheater(string theaterId)
        {
            try
            {
                _logger.LogInformation("Attempting to delete theater with ID {TheaterId}.", theaterId);

                var theater = _dbContext.Theaters.Find(theaterId);
                if (theater == null)
                    throw new Exception("Theater không tồn tại.");

                _dbContext.Theaters.Remove(theater);
                _dbContext.SaveChanges();

                _logger.LogInformation("Theater with ID {TheaterId} deleted successfully.", theaterId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting theater with ID {TheaterId}.", theaterId);
                throw;
            }
        }

        public string UpdateTheater(TheaterDto dto)
        {
            try
            {
                _logger.LogInformation("Updating theater with ID {TheaterId}.", dto.Id);

                var theater = _dbContext.Theaters.Find(dto.Id);
                if (theater == null)
                    throw new Exception("Theater không tồn tại.");

                theater.Name = dto.Name;
                theater.Location = dto.Location;
                theater.ChainId = dto.ChainId;

                _dbContext.Theaters.Update(theater);
                _dbContext.SaveChanges();

                _logger.LogInformation("Theater with ID {TheaterId} updated successfully.", dto.Id);
                return theater.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating theater with ID {TheaterId}.", dto.Id);
                throw;
            }
        }
    }
}
