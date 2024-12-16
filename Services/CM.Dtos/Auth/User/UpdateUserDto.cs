using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CM.Domain.Auth;
using Swashbuckle.AspNetCore.Annotations;

namespace CM.Dtos.User
{
    public class UpdateUserDto
    {
        public int Id { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public GenderType Gender { get; set; }

        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
    }
}
