using CM.Movie.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Movie.Infrastructure
{
    public class MovieDbContext : DbContext 
    {
        public DbSet<MoMovie> Movies { get; set; }
        public DbSet<MoGenre> Genres { get; set; }
        public DbSet<MoCast> Casts { get; set; }

        public DbSet<MoMovie_Cast> MovieCasts { get; set; }
        public DbSet<MoMovie_Genre> MovieGenres { get; set; }

        public MovieDbContext(DbContextOptions<MovieDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<MoMovie_Genre>()
                .HasOne<MoMovie>()
                .WithMany()
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MoMovie_Genre>()
                .HasOne<MoGenre>()
                .WithMany()
                .HasForeignKey(e => e.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<MoMovie_Cast>()
                .HasOne<MoMovie>()
                .WithMany()
                .HasForeignKey(e => e.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MoMovie_Cast>()
                .HasOne<MoCast>()
                .WithMany()
                .HasForeignKey(e => e.CastId)
                .OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(modelBuilder);
        }
    }
}
