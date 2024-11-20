using CloudinaryDotNet.Actions;
using CM.Dtos.Movie;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Cloudinary.Abstracts
{
    public interface ICloudService
    {
        Task<string> UploadImageAsync(IFormFile image, string folder);
    }
}