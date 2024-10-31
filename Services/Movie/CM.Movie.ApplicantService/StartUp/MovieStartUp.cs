using CM.Movie.ApplicantService.MovieModule.Abstracts;
using CM.Movie.ApplicantService.MovieModule.Implements;
using CM.Movie.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Share.Constant.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Movie.ApplicantService.StartUp
{
    public static class MovieStartUp
    {
        public static void ConfigureMovie(this WebApplicationBuilder builder, string? assemblyName)
        {
            builder.Services.AddDbContext<MovieDbContext>(
                options =>
                {
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString("Default"),
                        options =>
                        {
                            options.MigrationsAssembly(assemblyName);
                            options.MigrationsHistoryTable(
                                DbSchema.TableMigrationsHistory,
                                DbSchema.Movie
                            );
                        }
                    );
                }
            );
             builder.Services.AddScoped<IMovieService, MovieService>();
           
        }
    }
}
