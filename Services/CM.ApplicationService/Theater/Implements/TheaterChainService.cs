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
                Id = dto.Id,
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

        public void DeleteTheaterChain(string theaterChainId)
        {
            // Tìm rạp chiếu theo Id
            var theaterChain = _dbContext.TheaterChains.Find(theaterChainId);

            if (theaterChain == null)
                throw new Exception("Theater chain không tồn tại.");

            // Xóa rạp chiếu khỏi database
            _dbContext.TheaterChains.Remove(theaterChain);
            _dbContext.SaveChanges();
        }

        public string UpdateTheaterChain(TheaterChainDto dto)
        {
            // Tìm rạp chiếu theo Id
            var theaterChain = _dbContext.TheaterChains.Find(dto.Id);

            if (theaterChain == null)
                throw new Exception("Theater chain không tồn tại.");

            // Cập nhật thông tin rạp chiếu
            theaterChain.Name = dto.Name;

            _dbContext.TheaterChains.Update(theaterChain);
            _dbContext.SaveChanges();

            return theaterChain.Id;
        }
    }
}
