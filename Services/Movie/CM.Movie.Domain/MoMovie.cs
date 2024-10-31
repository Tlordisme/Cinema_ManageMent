using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Movie.Domain
{
    public class MoMovie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(256)]
        public string Title { get; set; }
        [MaxLength(50)]
        public string Director { get; set; } // Đạo diễn
        [MaxLength(50)]
        public string Country   { get; set; }
        [MaxLength(50)]
        public string Language  { get; set; }
        public virtual ICollection<MoGenre> MovieGenres { get; set; } = new List<MoGenre>();
        public virtual ICollection<MoCast> MovieCasts { get; set; } = new List<MoCast>();
    }
}
