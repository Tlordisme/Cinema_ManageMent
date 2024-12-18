using CM.Dtos.Revenue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Revenue.Abstracts
{
    public interface IRevenueService
    {
        Task<IEnumerable<dynamic>> GetRevenueByDateAsync(string date);

        Task<IEnumerable<dynamic>> GetRevenueByMovieAsync(int? movieId);
    }
}