using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Auth.ApplicantService.Common.Abstracts
{
    public interface IValidateEmailService
    {      
           bool IsValidEmail(string email);
        }

}