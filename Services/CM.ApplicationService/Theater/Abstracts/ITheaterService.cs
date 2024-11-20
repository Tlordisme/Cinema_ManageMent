using CM.Domain.Theater;
using CM.Dtos.Theater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Theater.Abstracts
{
    public interface  ITheaterService
    {
        string CreateTheater(TheaterDto dto); 
        List<CMTheater> GetTheatersByChainId(string chainId);
    }
}
