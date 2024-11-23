using CM.ApplicationService.Auth.Common;
using CM.ApplicationService.AuthModule.Implements;
using CM.ApplicationService.Common;
using CM.ApplicationService.Seat.Abstracts;
using CM.Auth.ApplicantService.Auth.Implements;
using CM.Domain.Auth;
using CM.Domain.Seat;
using CM.Dtos.Seat;
using CM.Infrastructure;
using CM.Infrastructure.Repositories.SeatRepository.Abstracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Share.ApplicationService;

namespace CM.ApplicationService.Seat.Implements
{
    public class SeatService : ServiceBase, ISeatService
    {
        public SeatService(CMDbContext dbContext, ILogger<ServiceBase> logger)
            : base(logger, dbContext)
        {
        }

        public void AddSeat(AddSeatDto seatDto)
        {
            // Kiểm tra xem vị trí X, Y đã có ghế chưa
            var existingSeat = _dbContext.Seats
                .FirstOrDefault(s => s.X == seatDto.X && s.Y == seatDto.Y && s.RoomID == seatDto.RoomID);
            if (existingSeat != null)
            {
                throw new Exception("Đã có ghế tại vị trí này.");
            }

            // Nếu ghế đôi, kiểm tra phải có ghế đối diện và chúng phải nằm cạnh nhau theo hàng
            if (seatDto.IsDoubleSeat)
            {
                if (seatDto.DoubleSeatId == null)
                {
                    throw new Exception("Ghế đôi phải có ghế đối diện.");
                }

                // Lấy ghế đối diện để kiểm tra
                var doubleSeat = _dbContext.Seats.Find(seatDto.DoubleSeatId);
                if (doubleSeat == null || doubleSeat.RoomID != seatDto.RoomID)
                {
                    throw new Exception("Ghế đối diện phải thuộc cùng phòng.");
                }

                // Kiểm tra ghế đôi có nằm cạnh nhau trong cùng hàng hay không
                if (!IsDoubleSeatValid(seatDto.X, seatDto.Y, seatDto.DoubleSeatId))
                {
                    throw new Exception("Ghế đôi phải ở cạnh nhau theo hàng.");
                }
            }

            // Tạo ghế mới
            var seat = new CMSeat
            {
                Name = seatDto.Name,
                X = seatDto.X,
                Y = seatDto.Y,
                SeatType = seatDto.SeatType,
                RoomID = seatDto.RoomID,
                Status = seatDto.Status,
                IsDoubleSeat = seatDto.IsDoubleSeat,
                DoubleSeatId = seatDto.DoubleSeatId
            };

            // Thêm ghế vào cơ sở dữ liệu
            _dbContext.Seats.Add(seat);
            _dbContext.SaveChanges();
        }

        public void DeleteSeat(int seatId)
        {
            var seat = _dbContext.Seats.FirstOrDefault(s => s.Id == seatId);
            if (seat == null)
            {
                throw new Exception("Ghế không tồn tại.");
            }

            _dbContext.Seats.Remove(seat);
            _dbContext.SaveChanges();
        }

        public List<SeatDto> GetSeatsByRoomId(string roomId)
        {
            return _dbContext.Seats
                            .Where(s => s.RoomID == roomId)
                            .Select(s => new SeatDto
                            {
                                Id = s.Id,
                                Name = s.Name,
                                X = s.X,
                                Y = s.Y,
                                SeatType = s.SeatType,
                                Status = s.Status,
                                IsDoubleSeat = s.IsDoubleSeat,
                                DoubleSeatId = s.DoubleSeatId
                            })
                            .ToList();
        }

        public bool IsDoubleSeatValid(int seatX, int seatY, int? doubleSeatId)
        {
            var seat = _dbContext.Seats.FirstOrDefault(s => s.X == seatX && s.Y == seatY);
            var doubleSeat = _dbContext.Seats.Find(doubleSeatId);

            // Kiểm tra nếu ghế đôi có trong cùng phòng và phải nằm cạnh nhau theo hàng
            return seat != null && doubleSeat != null &&
                   seat.RoomID == doubleSeat.RoomID &&
                   Math.Abs(seat.X - doubleSeat.X) == 1 && seat.Y == doubleSeat.Y;
        }

        public void UpdateSeat(UpdateSeatDto seatDto)
        {
            var seat = _dbContext.Seats.FirstOrDefault(s => s.Id == seatDto.Id);
            if (seat == null)
            {
                throw new Exception("Ghế không tồn tại.");
            }

            // Kiểm tra vị trí X, Y đã có ghế chưa nếu thay đổi
            if (seat.X != seatDto.X || seat.Y != seatDto.Y)
            {
                var existingSeat = _dbContext.Seats
                    .FirstOrDefault(s => s.X == seatDto.X && s.Y == seatDto.Y && s.RoomID == seatDto.RoomId);
                if (existingSeat != null)
                {
                    throw new Exception("Đã có ghế tại vị trí này.");
                }
            }

            // Nếu ghế đôi, kiểm tra phải có ghế đối diện và chúng phải nằm cạnh nhau theo hàng
            if (seatDto.IsDoubleSeat)
            {
                if (seatDto.DoubleSeatId == null)
                {
                    throw new Exception("Ghế đôi phải có ghế đối diện.");
                }

                // Lấy ghế đối diện để kiểm tra
                var doubleSeat = _dbContext.Seats.Find(seatDto.DoubleSeatId);
                if (doubleSeat == null || doubleSeat.RoomID != seatDto.RoomId)
                {
                    throw new Exception("Ghế đối diện phải thuộc cùng phòng.");
                }

                // Kiểm tra ghế đôi có nằm cạnh nhau trong cùng hàng hay không
                if (!IsDoubleSeatValid(seatDto.X, seatDto.Y, seatDto.DoubleSeatId))
                {
                    throw new Exception("Ghế đôi phải ở cạnh nhau theo hàng.");
                }
            }

            seat.Name = seatDto.Name;
            seat.X = seatDto.X;
            seat.Y = seatDto.Y;
            seat.SeatType = seatDto.SeatType;
            seat.Status = seatDto.Status;
            seat.IsDoubleSeat = seatDto.IsDoubleSeat;
            seat.DoubleSeatId = seatDto.DoubleSeatId;

            // Cập nhật vào cơ sở dữ liệu
            _dbContext.SaveChanges();
        }
    }
}
