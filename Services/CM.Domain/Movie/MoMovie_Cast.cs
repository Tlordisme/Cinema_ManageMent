using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Domain.Movie
{
    public class MoMovie_Cast
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public MoMovie Movie { get; set; }
        //[ForeignKey(nameof(MovieId))]
        public int MovieId { get; set; }
        //[ForeignKey(nameof(CastId))]

        public MoCast Cast { get; set; }
        public int CastId { get; set; }
    }
}
