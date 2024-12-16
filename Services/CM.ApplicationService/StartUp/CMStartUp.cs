using System.Text;
using Catel.Services;
using CloudinaryDotNet;
using CM.ApplicantService.Auth.Permission.Abstracts;
using CM.Application.Ticket.Services;
using CM.ApplicationService.Auth.Common;
using CM.ApplicationService.AuthModule.Abstracts;
using CM.ApplicationService.AuthModule.Implements;
using CM.ApplicationService.Cloudinary.Abstracts;
using CM.ApplicationService.Cloudinary.Implements;
using CM.ApplicationService.Movie.Abstracts;
using CM.ApplicationService.Movie.Implements;
using CM.ApplicationService.Notification.Abstracts;
using CM.ApplicationService.Notification.Implements;
using CM.ApplicationService.Payment.Abstracts;
using CM.ApplicationService.Payment.Implements;
using CM.ApplicationService.Revenue.Abstracts;
using CM.ApplicationService.Revenue.Implements;
using CM.ApplicationService.RoleModule.Abstracts;
using CM.ApplicationService.RoleModule.Implements;
using CM.ApplicationService.Seat.Abstracts;
using CM.ApplicationService.Seat.Implements;
using CM.ApplicationService.Showtime.Abstracts;
using CM.ApplicationService.Showtime.Implements;
using CM.ApplicationService.Theater.Abstracts;
using CM.ApplicationService.Theater.Implements;
using CM.ApplicationService.Ticket.Abstracts;
using CM.ApplicationService.Ticket.Implements;
using CM.ApplicationService.UserModule.Abstracts;
using CM.ApplicationService.UserModule.Implements;
using CM.Auth.ApplicantService.Auth.Abstracts;
using CM.Auth.ApplicantService.Auth.Implements;
using CM.Auth.ApplicantService.Permission.Implements;
using CM.Domain.Auth;
using CM.Infrastructure;
using dotenv.net;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Share.Constant.Database;

namespace CM.ApplicationService.StartUp
{
    public static class CMStartUp
    {
        public static void ConfigureService(
            this WebApplicationBuilder builder,
            string? assemblyName
        )
        {
            //AuthDbContext
            builder.Services.AddDbContext<CMDbContext>(
                options =>
                {
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString("Default"),
                        options =>
                        {
                            options.MigrationsAssembly(assemblyName);
                            options.MigrationsHistoryTable(
                                DbSchema.TableMigrationsHistory,
                                DbSchema.Cinema
                            );
                        }
                    );
                    options.EnableSensitiveDataLogging();
                },
                ServiceLifetime.Scoped
            );

            //Auth
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<ValidateService, ValidateService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IPermissionService, PermissionService>();
            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<PermissionService>();
            //Movie
            builder.Services.AddScoped<IMovieService, MovieService>();
            //Theater
            builder.Services.AddScoped<ITheaterChainService, TheaterChainService>();
            builder.Services.AddScoped<ITheaterService, TheaterService>();
            builder.Services.AddScoped<IRoomService, RoomService>();
            //Seat
            builder.Services.AddScoped<ISeatService, SeatService>();
            builder.Services.AddScoped<ISeatPriceService, SeatPriceService>();
            //Showtime
            builder.Services.AddScoped<IShowtimeService, ShowtimeService>();
            //Ticket
            builder.Services.AddScoped<ITicketService, TicketService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<ITicketRepository,TicketRepository>();
            //revenue
            builder.Services.AddScoped<IRevenueService, RevenueService>();
            //Cloudinary
            var cloudinaryConfig = builder.Configuration.GetSection("Cloudinary");
            var account = new Account(
                cloudinaryConfig["CloudName"],
                cloudinaryConfig["ApiKey"],
                cloudinaryConfig["ApiSecret"]
            );
            builder.Services.AddSingleton(new CloudinaryDotNet.Cloudinary(account));

            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<ICloudService, CloudService>();
            //Email
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<SmsService>();

            // p gửi cả Email và SMS
            builder.Services.AddScoped<INotificationService>(provider =>
            {
                var emailService = provider.GetRequiredService<EmailService>();
                var smsService = provider.GetRequiredService<SmsService>();

                return new CombinedNotificationService(emailService, smsService);
            });
            builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();

            //HangFire
            builder.Services.AddHangfire(config =>
                config.UseSqlServerStorage(builder.Configuration.GetConnectionString("Default"))
            );
            builder.Services.AddHangfireServer();

           
            // Cấu hình JWT Authentication

            builder
                .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])
                        ),
                    };
                });
        }
    }
}
