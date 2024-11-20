
using CM.Domain.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Auth.ApplicantService.Auth.Abstracts
{
    public interface IJwtService
    {
        public Task<string> GenerateJwtToken(User user);
    }
}