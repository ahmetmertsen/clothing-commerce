using auth_service.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Persistence.Context
{
    public class AuthServiceDbContext : IdentityDbContext<User, Role, Guid>
    {
        public AuthServiceDbContext(DbContextOptions<AuthServiceDbContext> options) : base(options) { }

        public DbSet<AuthSession> AuthSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthSession>(entity =>
            {
                entity.HasKey(session => session.Id);

                entity.Property(session => session.RefreshTokenHash)
                    .HasMaxLength(64)
                    .IsRequired();

                entity.Property(session => session.DeviceName)
                    .HasMaxLength(100);

                entity.Property(session => session.UserAgent)
                    .HasMaxLength(500);

                entity.Property(session => session.IpAddress)
                    .HasMaxLength(64);

                entity.Property(session => session.RevokedReason)
                    .HasMaxLength(200);

                entity.HasOne(session => session.User)
                    .WithMany(user => user.AuthSessions)
                    .HasForeignKey(session => session.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(session => session.RefreshTokenHash)
                    .IsUnique();

                entity.HasIndex(session => new { session.UserId, session.RevokedAt });
                entity.HasIndex(session => session.TokenFamilyId);
                entity.HasIndex(session => session.ExpiresAt);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
