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

            var theater = new CMTheater
            {
                Id = dto.Id,
                Name = dto.Name,
                Location = dto.Location,
                ChainId = dto.ChainId
            };

            _dbContext.Theaters.Add(theater);
            _dbContext.SaveChanges();

            return theater.Id;
        }

        public List<CMTheater> GetTheatersByChainId(string chainId)
        {
            return _dbContext.Theaters.Where(t => t.ChainId == chainId).ToList();
        }
    }
}
