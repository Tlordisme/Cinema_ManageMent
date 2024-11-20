//using CM.ApplicationService.Auth.Common;
//using CM.ApplicationService.AuthModule.Implements;
//using CM.ApplicationService.Common;
//using CM.ApplicationService.Seat.Abstracts;
//using CM.Auth.ApplicantService.Auth.Implements;
//using CM.Domain.Auth;
//using CM.Domain.Seat;
//using CM.Dtos.Seat;
//using CM.Infrastructure;
//using CM.Infrastructure.Repositories.SeatRepository.Abstracts;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Logging;
//using Share.ApplicationService;

//namespace CM.ApplicationService.Seat.Implements
//{
//    public class SeatService : ServiceBase, ISeatService
//    {
//        private readonly ISeatRepository _seatRepository;
//        public SeatService(
//            CMDbContext dbContext,
//            ILogger<AuthService> logger,
//            ISeatRepository seatRepository
           
//        )
//            : base(logger, dbContext)
//        {        
//           _seatRepository = seatRepository;
//        }

//        public CMSeat GetSeatById(int seatId)
//        {
//            var seat = _seatRepository.GetById(seatId);
//            if (seat == null)
//            {
//                throw new KeyNotFoundException($"Seat with ID {seatId} not found.");
//            }
//            return seat;
//        }

//        public List<CMSeat> GetSeatsByShowtimeId(string showtimeId)
//        {
//            var seats = _seatRepository.GetSeatsByShowtime(showtimeId);
//            if (!seats.Any())
//            {
//                throw new KeyNotFoundException($"No seats found for Showtime ID {showtimeId}.");
//            }
//            return seats.ToList();
//        }

//        public void UpdateSeat(CMSeat seat)
//        {
//            var existingSeat = _seatRepository.GetById(seat.Id);
//            if (existingSeat == null)
//            {
//                throw new KeyNotFoundException($"Seat with ID {seat.Id} not found.");
//            }

//            existingSeat.Status = seat.Status;
//            existingSeat.Price = seat.Price;
//            existingSeat.SeatType = seat.SeatType;

//            _seatRepository.Update(existingSeat);
//        }
//    }
//}
