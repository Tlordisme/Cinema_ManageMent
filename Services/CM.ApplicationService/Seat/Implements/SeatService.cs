using CM.ApplicationService.Common;
using CM.ApplicationService.Seat.Abstracts;
using CM.Domain.Seat;
using CM.Dtos.Seat;
using CM.Infrastructure;
using Microsoft.Extensions.Logging;

namespace CM.ApplicationService.Seat.Implements
{
    public class SeatService : ServiceBase, ISeatService
    {
        private readonly ILogger<SeatService> _logger;

        public SeatService(CMDbContext dbContext, ILogger<SeatService> logger)
            : base(logger, dbContext)
        {
            _logger = logger;
        }

        public void AddSeat(AddSeatDto seatDto)
        {
            try
            {
                var existingSeat = _dbContext.Seats
                    .FirstOrDefault(s => s.X == seatDto.X && s.Y == seatDto.Y && s.RoomID == seatDto.RoomID);
                if (existingSeat != null)
                {
                    _logger.LogWarning("Ghế đã tồn tại tại vị trí {X}, {Y} trong phòng {RoomID}.", seatDto.X, seatDto.Y, seatDto.RoomID);
                    throw new Exception("Đã có ghế tồn tại ở vị trí này.");
                }

                var existingName = _dbContext.Seats
                    .FirstOrDefault(s => s.Name == seatDto.Name && s.RoomID == seatDto.RoomID);
                if (existingName != null)
                {
                    _logger.LogWarning("Ghế trùng tên: {Name} trong phòng {RoomID}.", seatDto.Name, seatDto.RoomID);
                    throw new Exception("Đã có ghế trùng tên.");
                }

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
                        _logger.LogWarning("Ghế cặp không hợp lệ. Double seat ID: {DoubleSeatId} trong phòng {RoomID}.", seatDto.DoubleSeatId, seatDto.RoomID);
                        throw new Exception("Ghế cặp phải thuộc cùng phòng.");
                    }

                    if (!IsDoubleSeatValid(seatDto.X, seatDto.Y, doubleSeat))
                    {
                        _logger.LogWarning("Ghế đôi phải ở cạnh nhau theo hàng. Vị trí ghế: {X}, {Y}, DoubleSeatId: {DoubleSeatId}.", seatDto.X, seatDto.Y, seatDto.DoubleSeatId);
                        throw new Exception("Ghế đôi phải ở cạnh nhau theo hàng.");
                    }

                    seat.DoubleSeatId = doubleSeat.Id;
                    doubleSeat.DoubleSeatId = seat.Id;
                }

                _dbContext.Seats.Add(seat);
                _dbContext.SaveChanges();

                _logger.LogInformation("Thêm ghế thành công. Ghế {Name} tại vị trí {X}, {Y} trong phòng {RoomID}.", seat.Name, seat.X, seat.Y, seat.RoomID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm ghế.");
                throw;
            }
        }

        public void LinkDoubleSeat(int seatId, int doubleSeatId)
        {
            try
            {
                var seat = _dbContext.Seats.Find(seatId);
                var doubleSeat = _dbContext.Seats.Find(doubleSeatId);

                if (seat == null || doubleSeat == null)
                {
                    _logger.LogWarning("Ghế hoặc ghế cặp không tồn tại. SeatId: {SeatId}, DoubleSeatId: {DoubleSeatId}.", seatId, doubleSeatId);
                    throw new Exception("Ghế hoặc ghế cặp không tồn tại.");
                }

                if (seat.RoomID != doubleSeat.RoomID)
                {
                    _logger.LogWarning("Ghế và ghế cặp phải thuộc cùng phòng. SeatId: {SeatId}, DoubleSeatId: {DoubleSeatId}.", seatId, doubleSeatId);
                    throw new Exception("Ghế và ghế cặp phải thuộc cùng phòng.");
                }

                if (!(IsDoubleSeat(seat.SeatType) && IsDoubleSeat(doubleSeat.SeatType)))
                {
                    _logger.LogWarning("Một hoặc cả hai ghế không phải ghế đôi. SeatId: {SeatId}, DoubleSeatId: {DoubleSeatId}.", seatId, doubleSeatId);
                    throw new Exception("Hai ghế này ko phải ghế đôi.");
                }

                if (!IsDoubleSeatValid(seat.X, seat.Y, doubleSeat))
                {
                    _logger.LogWarning("Ghế đôi phải ở cạnh nhau theo hàng. Vị trí ghế: {X}, {Y}, DoubleSeatId: {DoubleSeatId}.", seat.X, seat.Y, doubleSeatId);
                    throw new Exception("Ghế đôi phải ở cạnh nhau theo hàng.");
                }

                seat.DoubleSeatId = doubleSeatId;
                doubleSeat.DoubleSeatId = seatId;

                _dbContext.SaveChanges();

                _logger.LogInformation("Liên kết ghế đôi thành công. SeatId: {SeatId}, DoubleSeatId: {DoubleSeatId}.", seatId, doubleSeatId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi liên kết ghế đôi.");
                throw;
            }
        }

        public bool IsDoubleSeatValid(int x, int y, CMSeat doubleSeat)
        {
            return y == doubleSeat.Y && Math.Abs(x - doubleSeat.X) == 1;
        }

        public void DeleteSeat(int seatId)
        {
            try
            {
                var seat = _dbContext.Seats.FirstOrDefault(s => s.Id == seatId);
                if (seat == null)
                {
                    _logger.LogWarning("Ghế không tồn tại. SeatId: {SeatId}.", seatId);
                    throw new Exception("Ghế không tồn tại.");
                }

                _dbContext.Seats.Remove(seat);
                _dbContext.SaveChanges();

                _logger.LogInformation("Xóa ghế thành công. SeatId: {SeatId}.", seatId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa ghế.");
                throw;
            }
        }

        public List<SeatDto> GetSeatsByRoomId(string roomId)
        {
            try
            {
                var seats = _dbContext.Seats
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

                _logger.LogInformation("Lấy danh sách ghế thành công. RoomID: {RoomID}, Tổng số ghế: {SeatCount}.", roomId, seats.Count);
                return seats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách ghế.");
                throw;
            }
        }

        public bool IsDoubleSeat(string seatType)
        {
            return seatType.Equals("Double", StringComparison.OrdinalIgnoreCase);
        }

        public void UpdateSeat(UpdateSeatDto seatDto)
        {
            try
            {
                var seat = _dbContext.Seats.FirstOrDefault(s => s.Id == seatDto.Id);
                if (seat == null)
                {
                    _logger.LogWarning("Ghế không tồn tại. SeatId: {SeatId}.", seatDto.Id);
                    throw new Exception("Ghế không tồn tại.");
                }

                if (seat.X != seatDto.X || seat.Y != seatDto.Y)
                {
                    var existingSeat = _dbContext.Seats
                        .FirstOrDefault(s => s.X == seatDto.X && s.Y == seatDto.Y && s.RoomID == seatDto.RoomId);
                    if (existingSeat != null)
                    {
                        _logger.LogWarning("Ghế đã tồn tại tại vị trí {X}, {Y} trong phòng {RoomID}.", seatDto.X, seatDto.Y, seatDto.RoomId);
                        throw new Exception("Đã có ghế tại vị trí này.");
                    }
                }

                seat.Name = seatDto.Name;
                seat.X = seatDto.X;
                seat.Y = seatDto.Y;
                seat.SeatType = seatDto.SeatType;
                seat.Status = seatDto.Status;

                _dbContext.SaveChanges();

                _logger.LogInformation("Cập nhật ghế thành công. Ghế {Name} tại vị trí {X}, {Y} trong phòng {RoomID}.", seat.Name, seat.X, seat.Y, seat.RoomID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật ghế.");
                throw;
            }
        }
    }
}
