using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CM.ApplicationService.Common;
using CM.ApplicationService.Theater.Abstracts;
using CM.Domain.Theater;
using CM.Dtos.Theater;
using CM.Infrastructure;
using Microsoft.Extensions.Logging;

namespace CM.ApplicationService.Theater.Implements
{
    public class RoomService : ServiceBase, IRoomService
    {
        public RoomService(CMDbContext dbContext, Logger<ServiceBase> logger)
            : base(logger, dbContext) { }

        public string CreateRoom(RoomDto dto)
        {
            var theater = _dbContext.Theaters.Find(dto.TheaterId);
            if (theater == null)
                throw new Exception("Theater không tồn tại.");

            var room = new CMRoom
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                TheaterId = dto.TheaterId,
                Type = dto.Type
            };

            _dbContext.Rooms.Add(room);
            _dbContext.SaveChanges();

            return room.Id;
        }

        public List<CMRoom> GetRoomsByTheaterId(string theaterId)
        {
            return _dbContext.Rooms.Where(r => r.TheaterId == theaterId).ToList();
        }
    }
}
