using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CM.Domain.Movie
{
    public class MoMovie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(256)]
        [Required]
        public string Title { get; set; }

        [MaxLength(50)]
        public string Director { get; set; } // Đạo diễn

        [MaxLength(50)]
        public string Nation { get; set; }

        public int LimitAge { get; set; }

        List<String> Language {  get; set; }

        [Required]
        public TimeSpan Duration { get; set; }
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
      
        public DateTime PublicTime { get; set; }
        public virtual ICollection<MoMovie_Genre> MovieGenres { get; set; }
        public virtual ICollection<MoMovie_Cast> MovieCasts { get; set; }
     
    }
}
