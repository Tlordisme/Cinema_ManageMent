using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Domain.Theater
{
    public class CMTheaterChain
    {
        [Key]
        public string Id {  get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public ICollection<CMTheater> Theaters { get; set; }
    }
}
