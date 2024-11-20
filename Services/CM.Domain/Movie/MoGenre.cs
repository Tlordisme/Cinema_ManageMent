using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Domain.Movie
{
    public class MoGenre
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 
        [MaxLength(256)]
        [Required]
        public string Name { get; set; } 

        public ICollection<MoMovie_Genre> MovieGenres { get; set; }

    }
}
