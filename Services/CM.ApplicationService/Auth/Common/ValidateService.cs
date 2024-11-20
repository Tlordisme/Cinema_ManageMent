using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CM.ApplicationService.Common;
using CM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace CM.ApplicationService.Auth.Common
{
    public class ValidateService : ServiceBase
    {


        public ValidateService(CMDbContext dbcontext, ILogger<ValidateService> logger)
            : base(logger, dbcontext)
        {
        }

        // Kiểm tra định dạng email và trùng lặp email
        public async Task ValidateEmailAsync(string email)
        {
            if (!IsValidEmail(email))
            {
                _logger.LogWarning($"Invalid email format for {email}.");
                throw new ArgumentException("Email invalid.");
            }

            bool emailExists = await _dbContext.Users.AnyAsync(u =>
                u.Email.ToLower() == email.ToLower()
            );
            if (emailExists)
            {
                _logger.LogWarning($"Email {email} already exists.");
                throw new InvalidOperationException("Email exists.");
            }
            _logger.LogInformation($"Email {email} is valid and not duplicated.");
        }

        // Kiểm tra trùng lặp tên người dùng
        public async Task ValidateUserNameAsync(string userName)
        {
            bool userNameExists = await _dbContext.Users.AnyAsync(u =>
                u.UserName.ToLower() == userName.ToLower()
            );
            if (userNameExists)
            {
                _logger.LogWarning($"Username {userName} already exists.");
                throw new InvalidOperationException("User exists.");
            }
        }

        // Kiểm tra hợp lệ của FullName
        public void ValidateFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > 60)
            {
                _logger.LogWarning($"Invalid full name: {fullName}. It is empty or too long.");
                throw new ArgumentException("Fullname is not suitable.");
            }
        }

        // Kiểm tra định dạng email (ví dụ đơn giản, có thể thay đổi cho phù hợp)
        private bool IsValidEmail(string email)
        {
            return email.Contains("@") && email.Contains(".");
        }

        public void ValidateDateOfBirth(DateTime dateOfBirth, int minimumAge = 14)
        {
            var today = DateTime.Now.Date;

            var age = today.Year - dateOfBirth.Year;

            if (today < dateOfBirth.AddYears(age))
            {
                age--;
            }

            if (age < minimumAge)
            {
                _logger.LogWarning($"User's age ({age}) is below the minimum required ({minimumAge}).");
                throw new ArgumentException($"User must be at least {minimumAge} years old.");
            }

            _logger.LogInformation(
                $"Date of Birth {dateOfBirth:yyyy-MM-dd} is valid and user is {age} years old."
            );
        }
    }
}