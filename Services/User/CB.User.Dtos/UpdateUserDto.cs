using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CM.Auth.Domain;

namespace CM.Auth.Dtos
{
    public class UpdateUserDto
    {
        public int Id { get; set; } // Bạn có thể giữ ID để xác định người dùng cần cập nhật

        public string Email { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }
        public GenderType? Gender { get; set; } // Nullable nếu không muốn yêu cầu
        public DateTime? DateOfBirth { get; set; }
    }
}
