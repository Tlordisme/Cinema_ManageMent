using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CM.Domain.Auth;

namespace CM.Domain.Movie
{
    public class MoComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(10000)]
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        [Required]
        [Range(1, 10)]
        public int Rating { get; set; }
        public DateTime CreateAt { get; set; }

        public int UserId { get; set; }

        public int MovieId { get; set; }

    }
}