using CM.Domain.Showtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Showtime.Abstracts
{
    public interface IShowtimeRepository
    {
        Task<CMShowtime> GetByIdAsync(string showtimeId);
        Task<List<CMShowtime>> GetAllAsync();
        Task AddAsync(CMShowtime showtime);
        Task UpdateAsync(CMShowtime showtime);
        Task DeleteAsync(CMShowtime showtime);
    }
}
