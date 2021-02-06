using AS.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AS.Data
{
    public class ASDbContext : IdentityDbContext<ASUser, IdentityRole, string>
    {
        public DbSet<ASAnimals> ASAnimals { get; set; }
        public DbSet<ASComments> ASComments { get; set; }
        public DbSet<ASEvents> ASEvents { get; set; }
        //public DbSet<ASUsersAnimals> ASUsersAnimals { get; set; }
        //public DbSet<ASAnimalsComments> ASAnimalsComments { get; set; }
        public DbSet<ASUserEvents> ASUserEvents { get; set; }
        //public DbSet<ASUsersComments> ASUsersComments { get; set; }

        public ASDbContext(DbContextOptions<ASDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<ASUsersAnimals>().HasKey(ua => new { ua.UserId, ua.AnimalsId });
            //modelBuilder.Entity<ASUsersAnimals>()
            //   .HasOne(ua => ua.User)
            //   .WithMany(u => u.UsersAnimals)
            //   .HasForeignKey(ua => ua.UserId);
            //modelBuilder.Entity<ASUsersAnimals>()
            //    .HasOne(ua => ua.Animals)
            //    .WithMany(a => a.ASUsersAnimals)
            //    .HasForeignKey(ua => ua.AnimalsId);

            modelBuilder.Entity<ASAnimals>()
                .HasOne(ac => ac.User)
                .WithMany(a => a.Animals)
                .HasForeignKey(ac => ac.UserId);
            modelBuilder.Entity<ASUser>()
                .HasMany(uc => uc.Animals)
                .WithOne(c => c.User);

            modelBuilder.Entity<ASUserEvents>().HasKey(ue => new { ue.UserId, ue.EventsId });
            modelBuilder.Entity<ASUserEvents>()
               .HasOne(ue => ue.User)
               .WithMany(u => u.UserEvents)
               .HasForeignKey(ue => ue.UserId);
            modelBuilder.Entity<ASUserEvents>()
                .HasOne(ue => ue.Event)
                .WithMany(e => e.UserEvents)
                .HasForeignKey(ue => ue.EventsId);

            //modelBuilder.Entity<ASComments>().HasKey(uc => new { uc.UserId, uc.Id });
            modelBuilder.Entity<ASComments>()
                .HasOne(c => c.user)
                .WithMany(us => us.Comments)
                .HasForeignKey(c => c.UserId);
            modelBuilder.Entity<ASUser>()
                .HasMany(uc => uc.Comments)
                .WithOne(c => c.user);

            //modelBuilder.Entity<ASAnimalsComments>().HasKey(ac => new { ac.AnimalsId, ac.CommentsId });
            modelBuilder.Entity<ASComments>()
               .HasOne(ac => ac.Animal)
               .WithMany(a => a.Comments)
               .HasForeignKey(ac => ac.AnimalPostId);
            modelBuilder.Entity<ASAnimals>()
                .HasMany(uc => uc.Comments)
                .WithOne(c => c.Animal);
            //modelBuilder.Entity<ASAnimalsComments>()
            //    .HasOne(cp => cp.Comments)
            //    .WithMany(p => p.AnimalsComments)
            //    .HasForeignKey(cp => cp.CommentsId);
        }
    }
}
