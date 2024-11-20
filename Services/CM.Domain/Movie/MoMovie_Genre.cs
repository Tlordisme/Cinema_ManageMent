using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Domain.Movie
{
    public class MoMovie_Genre
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int MovieId { get; set; }
        //[ForeignKey(nameof(MovieId))]

        public MoMovie Movie { get; set; }
        //[ForeignKey(nameof(GenreId))]
        public int GenreId { get; set; }

        public MoGenre Genre { get; set; }
    }
}
