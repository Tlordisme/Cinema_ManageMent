using CM.Dtos.Showtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Showtime.Abstracts
{
    public interface IShowtimeService
    {
        Task<ShowtimeDto> CreateShowtimeAsync(CreateShowtimeDto createShowtimeDto);
        Task<List<ShowtimeDto>> GetAllShowtimesAsync();
     
        Task<ShowtimeDto> GetShowtimeByIdAsync(string showtimeId);
        Task<bool> DeleteShowtimeAsync(string showtimeId);
        Task<bool> UpdateShowtimeAsync(UpdateShowtimeDto updateShowtimeDto);
    }
    
}
