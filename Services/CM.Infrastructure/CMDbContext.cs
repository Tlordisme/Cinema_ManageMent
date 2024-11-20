using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CM.Domain.Auth;
using CM.Domain.Movie;
using CM.Domain.Seat;
using CM.Domain.Showtime;
using CM.Domain.Theater;
using Microsoft.EntityFrameworkCore;

namespace CM.Infrastructure
{
    public class CMDbContext : DbContext
    {
        //Auth
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<RolePermission> RolePermissions { get; set; }

        //Movie
        public DbSet<MoMovie> Movies { get; set; }
        public DbSet<MoGenre> Genres { get; set; }
        public DbSet<MoCast> Casts { get; set; }
        public DbSet<MoMovie_Cast> MovieCasts { get; set; }
        public DbSet<MoMovie_Genre> MovieGenres { get; set; }
        public DbSet<MoComment> Comments { get; set; }

        //Theater
        public DbSet<CMTheater> Theaters { get; set; }
        public DbSet<CMTheaterChain> TheaterChains { get; set; }
        public DbSet<CMRoom> Rooms { get; set; }

        //Showtime
        public DbSet<CMShowtime> Showtimes;

        //Seat
        public DbSet<CMSeat> Seats { get; set; }

        public CMDbContext(DbContextOptions<CMDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Auth
            modelBuilder.Entity<User>().Property(u => u.Gender).HasConversion<string>();
            modelBuilder
                .Entity<UserRole>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<UserRole>()
                .HasOne<Role>()
                .WithMany()
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<RolePermission>()
                .HasOne<Role>()
                .WithMany()
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            //Movie
            modelBuilder.Entity<MoMovie_Genre>().HasKey(mg => new { mg.MovieId, mg.GenreId });

            // Mối quan hệ giữa MoMovie_Genre và MoMovie
            modelBuilder
                .Entity<MoMovie_Genre>()
                .HasOne(mg => mg.Movie)
                .WithMany(m => m.MovieGenres)
                .HasForeignKey(mg => mg.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            // Mối quan hệ giữa MoMovie_Genre và MoGenre
            modelBuilder
                .Entity<MoMovie_Genre>()
                .HasOne(mg => mg.Genre)
                .WithMany(g => g.MovieGenres)
                .HasForeignKey(mg => mg.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MoMovie_Cast>().HasKey(mc => new { mc.MovieId, mc.CastId });

            modelBuilder
                .Entity<MoMovie_Cast>()
                .HasOne(mc => mc.Movie)
                .WithMany(m => m.MovieCasts)
                .HasForeignKey(mc => mc.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<MoMovie_Cast>()
                .HasOne(mc => mc.Cast)
                .WithMany(c => c.MovieCasts)
                .HasForeignKey(mc => mc.CastId)
                .OnDelete(DeleteBehavior.Restrict);
            // Cấu hình mối quan hệ giữa Movie và Comment
            modelBuilder
                .Entity<MoComment>()
                .HasOne<MoMovie>()
                .WithMany()
                .HasForeignKey(c => c.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<MoComment>()
                .HasOne<User>() // Liên kết với thực thể User
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //Theater
            modelBuilder
                .Entity<CMTheater>()
                .HasOne(t => t.TheaterChain)
                .WithMany(tc => tc.Theaters)
                .HasForeignKey(t => t.ChainId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder
                .Entity<CMRoom>()
                .HasOne(t => t.Theater)
                .WithMany(tc => tc.Rooms)
                .HasForeignKey(t => t.TheaterId)
                .OnDelete(DeleteBehavior.Restrict);

            //Showtime
            modelBuilder
                .Entity<CMShowtime>()
                .HasOne(st => st.Movie)
                .WithMany()
                .HasForeignKey(s => s.MovieID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder
                .Entity<CMShowtime>()
                .HasOne(st => st.Room)
                .WithMany()
                .HasForeignKey(s => s.RoomID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<CMSeat>()
                .HasOne(s => s.Room)
                .WithMany()
                .HasForeignKey(s => s.RoomID)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
