﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CM.Domain.Movie;
using Swashbuckle.AspNetCore.Annotations;

namespace CM.Domain.Auth
{
    [Table(nameof(User))]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [MaxLength(256)]
        [Required]
        public string Email { get; set; }

        [MaxLength(50)]
        [Required]
        public string UserName { get; set; }

        [MaxLength(15)]
        [Required]
        public string PhoneNumber { get; set; }

        [MinLength(8)]
        [Required]
        public string Password { get; set; }

        [MaxLength(256)]
        [Required]
        public string FullName { get; set; }

        public GenderType Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        //public bool IsActive {  get; set; }

        //public virtual ICollection<MoComment> Comments { get; set; }

        //public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
