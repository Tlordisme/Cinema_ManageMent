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
        Task<List<RevenueDto>> GetRevenueByDateAsync();
        Task<List<MovieRevenueDto>> GetRevenueByMovieAsync();
    }
}
