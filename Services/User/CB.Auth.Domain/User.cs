using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Auth.Domain
{
    [Table(nameof(User))]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [MaxLength(256)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string UserName { get; set; }

        [MinLength(8)]
        public string Password { get; set; }

        [MaxLength(256)]
        public string FullName { get; set; }

        public GenderType Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

        //public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
