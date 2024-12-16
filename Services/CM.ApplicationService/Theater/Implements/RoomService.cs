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
        public RoomService(CMDbContext dbContext, ILogger<ServiceBase> logger)
            : base(logger, dbContext) { }

        public string CreateRoom(RoomDto dto)
        {
            var theater = _dbContext.Theaters.Find(dto.TheaterId);
            if (theater == null)
                throw new Exception("Theater không tồn tại.");
            if (theater.Rooms == null)
                theater.Rooms = new List<CMRoom>();
            var room = new CMRoom
            {
                Id = dto.Id,
                Name = dto.Name,
                TheaterId = dto.TheaterId,
                Type = dto.Type
            };
            theater.Rooms.Add(room);

            _dbContext.Rooms.Add(room);
            _dbContext.SaveChanges();

            return room.Id;
        }

        public List<CMRoom> GetRoomsByTheaterId(string theaterId)
        {
            return _dbContext.Rooms.Where(r => r.TheaterId == theaterId).ToList();
        }

        public void DeleteRoom(string roomId)
        {
            var room = _dbContext.Rooms.Find(roomId);

            if (room == null)
                throw new Exception("Room không tồn tại.");

            var theater = _dbContext.Theaters.Find(room.TheaterId);
            if (theater != null && theater.Rooms != null)
            {
                theater.Rooms.Remove(room);
            }

            _dbContext.Rooms.Remove(room);
            _dbContext.SaveChanges();
        }

        public void UpdateRoom(RoomDto dto)
        {
            var room = _dbContext.Rooms.Find(dto.Id);
            if (room == null)
                throw new Exception("Room không tồn tại.");

            var theater = _dbContext.Theaters.Find(dto.TheaterId);
            if (theater == null)
                throw new Exception("Theater không tồn tại.");

            room.Name = dto.Name;
            room.Type = dto.Type;
            room.TheaterId = dto.TheaterId;

            _dbContext.SaveChanges();
        }
    }
}
