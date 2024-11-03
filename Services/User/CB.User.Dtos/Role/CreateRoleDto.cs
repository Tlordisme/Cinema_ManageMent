using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Auth.Dtos.Role
{
    public class CreateRoleDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
