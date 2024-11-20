
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Domain.Theater
{
    public class CMRoom
    {
        [Key]
        public string Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public string TheaterId { get; set; }
        public CMTheater Theater { get; set; }
        [MaxLength(50)]
        public string Type { get; set; }

    }
}
