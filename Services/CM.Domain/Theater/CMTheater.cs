using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Domain.Theater
{
    public class CMTheater
    {
        [Key]
        public string Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(256)]
        public string Location { get; set; }
        [Required] 
        
        public string ChainId { get; set; } 
        public CMTheaterChain TheaterChain { get; set; }
        public ICollection<CMRoom> Rooms { get; set; }
        
    }
}
