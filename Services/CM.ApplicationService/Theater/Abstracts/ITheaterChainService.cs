using CM.Domain.Theater;
using CM.Dtos.Theater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Theater.Abstracts
{
    public interface ITheaterChainService
    {
        string CreateTheaterChain(TheaterChainDto dto);
        List<CMTheaterChain> GetAllTheaterChains();
        void DeleteTheaterChain(string theaterChainId);
        string UpdateTheaterChain(TheaterChainDto dto);
    }
}