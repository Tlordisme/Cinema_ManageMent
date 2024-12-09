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
                .FirstOrDefault(s => s.X == seatDto.X && s.Y == seatDto.Y && s.RoomID == seatDto.RoomID );
            if (existingSeat != null)
            {
                throw new Exception("Đã có ghế tồn tại ở vị trí này.");
            }
            var existingName = _dbContext.Seats
                    .FirstOrDefault(s => s.Name == seatDto.Name && s.RoomID == seatDto.RoomID);
            if (existingName != null)
            {
                throw new Exception("Đã có ghế trùng tên .");
            }


            // Tạo ghế mới
            var seat = new CMSeat
            {
                Name = seatDto.Name,
                X = seatDto.X,
                Y = seatDto.Y,
                SeatType = seatDto.SeatType,
                RoomID = seatDto.RoomID,
                Status = seatDto.Status
            };

            
            if (IsDoubleSeat(seat.SeatType) && seatDto.DoubleSeatId != null)
            {
               
                var doubleSeat = _dbContext.Seats.Find(seatDto.DoubleSeatId);
                if (doubleSeat == null || doubleSeat.RoomID != seatDto.RoomID)
                {
                    throw new Exception("Ghế cặp phải thuộc cùng phòng.");
                }

                
                if (!IsDoubleSeatValid(seatDto.X, seatDto.Y, doubleSeat))
                {
                    throw new Exception("Ghế đôi phải ở cạnh nhau theo hàng.");
                }

                seat.DoubleSeatId = doubleSeat.Id;
                doubleSeat.DoubleSeatId = seat.Id; 
            }

            
            _dbContext.Seats.Add(seat);
            _dbContext.SaveChanges();
        }

        public void LinkDoubleSeat(int seatId, int doubleSeatId)
        {
            var seat = _dbContext.Seats.Find( seatId );
            var doubleSeat = _dbContext.Seats.Find(doubleSeatId);
            

            if (seat == null || doubleSeat == null)
            {
                throw new Exception("Ghế hoặc ghế cặp không tồn tại.");
            }

            if (seat.RoomID != doubleSeat.RoomID)
            {
                throw new Exception("Ghế và ghế cặp phải thuộc cùng phòng.");
            }
            if (!(IsDoubleSeat(seat.SeatType) && IsDoubleSeat(doubleSeat.SeatType))) { 
                throw new Exception("Hai ghế này ko phải ghế đôi");
            }

            if (!IsDoubleSeatValid(seat.X, seat.Y, doubleSeat))
            {
                throw new Exception("Ghế đôi phải ở cạnh nhau theo hàng.");
            }

            seat.DoubleSeatId = doubleSeatId;   
            doubleSeat.DoubleSeatId = seatId;

            _dbContext.SaveChanges();
        }

        public bool IsDoubleSeatValid(int x, int y, CMSeat doubleSeat)
        {
            // Kiểm tra logic ghế đôi nằm cạnh nhau
            return y == doubleSeat.Y && Math.Abs(x - doubleSeat.X) == 1;
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
                                DoubleSeatId = s.DoubleSeatId
                            })
                            .ToList();
        }

        public bool IsDoubleSeat(string seatType)
        {
            return seatType.Equals("Double", StringComparison.OrdinalIgnoreCase);
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

            // Xử lý ghế đôi dựa trên SeatType
            if (seat.SeatType == "Double" && seatDto.SeatType != "Double")
            {
                // Nếu bỏ trạng thái ghế đôi, xóa liên kết ở ghế cặp
                if (seat.DoubleSeatId != null)
                {
                    var doubleSeat = _dbContext.Seats.Find(seat.DoubleSeatId);
                    if (doubleSeat != null)
                    {
                        doubleSeat.DoubleSeatId = null;
                    }
                }
                seat.DoubleSeatId = null;
            }
            else if (seatDto.SeatType == "Double")
            {
                if (seatDto.DoubleSeatId == null)
                {
                    throw new Exception("Ghế đôi phải có ghế bên cạnh.");
                }

                // Lấy ghế cặp mới để kiểm tra
                var newDoubleSeat = _dbContext.Seats.Find(seatDto.DoubleSeatId);
                if (newDoubleSeat == null || newDoubleSeat.RoomID != seatDto.RoomId)
                {
                    throw new Exception("Ghế cặp phải thuộc cùng phòng.");
                }

                
                if (!IsDoubleSeatValid(seatDto.X, seatDto.Y, newDoubleSeat))
                {
                    throw new Exception("Ghế đôi phải ở cạnh nhau theo hàng.");
                }

                // Cập nhật liên kết ghế 
                if (seat.DoubleSeatId != newDoubleSeat.Id)
                {
                    // Xóa liên kết
                    if (seat.DoubleSeatId != null)
                    {
                        var oldDoubleSeat = _dbContext.Seats.Find(seat.DoubleSeatId);
                        if (oldDoubleSeat != null)
                        {
                            oldDoubleSeat.DoubleSeatId = null;
                        }
                    }

                    
                    seat.DoubleSeatId = newDoubleSeat.Id;
                    newDoubleSeat.DoubleSeatId = seat.Id;
                }
            }

            // Cập nhật thông tin ghế
            seat.Name = seatDto.Name;
            seat.X = seatDto.X;
            seat.Y = seatDto.Y;
            seat.SeatType = seatDto.SeatType;
            seat.Status = seatDto.Status;

            _dbContext.SaveChanges();
        }

    }
}
