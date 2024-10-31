using CM.Auth.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Auth.Dtos
{
    public class RegisterUserDto
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public GenderType Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

}
