using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CM.ApplicationService.Auth.Common;
using CM.ApplicationService.AuthModule.Implements;
using CM.ApplicationService.Common;
using CM.ApplicationService.Theater.Abstracts;
using CM.Auth.ApplicantService.Auth.Implements;
using CM.Domain.Auth;
using CM.Domain.Theater;
using CM.Dtos.Theater;
using CM.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CM.ApplicationService.Theater.Implements
{
    public class TheaterChainService : ServiceBase, ITheaterChainService
    {
        public TheaterChainService(CMDbContext dbContext, ILogger<ServiceBase> logger)
            : base(logger, dbContext) { }

        public string CreateTheaterChain(TheaterChainDto dto)
        {
            var theaterChain = new CMTheaterChain
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
            };

            _dbContext.TheaterChains.Add(theaterChain);
            _dbContext.SaveChanges();

            return theaterChain.Id;
        }

        public List<CMTheaterChain> GetAllTheaterChains()
        {
            return _dbContext.TheaterChains.ToList();
        }
    }
}
