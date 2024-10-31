using CM.Auth.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Auth.ApplicantService.AuthModule.Abstracts
{
    public interface IJwtService
    {
        string GenerateToken(User user, List<string> roles, List<string> permissions);
    }
}
