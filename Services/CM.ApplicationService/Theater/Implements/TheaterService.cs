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
        public TheaterService(
            CMDbContext dbContext,
            ILogger<ServiceBase> logger

        )
            : base(logger, dbContext)
        {
        }

        public string CreateTheater(TheaterDto dto)
        {
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

            return theater.Id;
        }

        public List<CMTheater> GetTheatersByChainId(string chainId)
        {
            return _dbContext.Theaters.Where(t => t.ChainId == chainId).ToList();
        }

        public void DeleteTheater(string theaterId)
        {
            var theater = _dbContext.Theaters.Find(theaterId);

            if (theater == null)
                throw new Exception("Theater không tồn tại.");

            _dbContext.Theaters.Remove(theater);
            _dbContext.SaveChanges();
        }

        public string UpdateTheater(TheaterDto dto)
        {
            var theater = _dbContext.Theaters.Find(dto.Id);

            if (theater == null)
                throw new Exception("Theater không tồn tại.");

            theater.Name = dto.Name;
            theater.Location = dto.Location;
            theater.ChainId = dto.ChainId;

            _dbContext.Theaters.Update(theater);
            _dbContext.SaveChanges();

            return theater.Id;
        }
    }
}
