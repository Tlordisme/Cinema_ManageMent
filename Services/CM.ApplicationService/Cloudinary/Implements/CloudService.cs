using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CM.ApplicationService.Cloudinary.Abstracts;
using dotenv.net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;



namespace CM.ApplicationService.Cloudinary.Implements
{
    public class CloudService : ICloudService
    {
        public readonly IConfiguration _configuration;

        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public CloudService(IConfiguration configuration, CloudinaryDotNet.Cloudinary cloudinary)
        {
            _configuration = configuration;
            _cloudinary = cloudinary;

        }

        public async Task<string> UploadImageAsync(IFormFile image, string folder)
        {
            if (image == null || image.Length == 0)
            {
                throw new ArgumentException("No file provided.");
            }



            using var stream = image.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(image.FileName, stream),
                Folder = folder
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult == null)
            {
                throw new Exception("Failed to upload image.");
            }

            return uploadResult.SecureUrl.ToString();
        }
    }
}