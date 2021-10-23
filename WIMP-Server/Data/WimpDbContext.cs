using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WIMP_Server.Models;
using WIMP_Server.Models.Users;

namespace WIMP_Server.Data
{
    public class WimpDbContext : IdentityDbContext<User>
    {
        public WimpDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Intel> Intel { get; set; }

        public DbSet<Character> Characters { get; set; }

        public DbSet<Ship> Ships { get; set; }

        public DbSet<StarSystem> StarSystems { get; set; }

        public DbSet<Stargate> Stargates { get; set; }

        public DbSet<InvitationKey> InvitationKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<Character>()
                .HasMany(c => c.Intel)
                .WithOne(c => c.Character)
                .HasForeignKey(c => c.CharacterId);

            modelBuilder
                .Entity<Intel>()
                .HasOne(c => c.Character)
                .WithMany(c => c.Intel)
                .HasForeignKey(c => c.CharacterId);

            modelBuilder
                .Entity<StarSystem>()
                .HasMany(c => c.Intel)
                .WithOne(c => c.StarSystem)
                .HasForeignKey(c => c.StarSystemId);

            modelBuilder
                .Entity<Intel>()
                .HasOne(c => c.StarSystem)
                .WithMany(c => c.Intel)
                .HasForeignKey(c => c.StarSystemId);

            modelBuilder
                .Entity<Ship>()
                .HasMany(c => c.Intel)
                .WithOne(c => c.Ship)
                .HasForeignKey(c => c.ShipId);

            modelBuilder
                .Entity<Intel>()
                .HasOne(c => c.Ship)
                .WithMany(c => c.Intel)
                .HasForeignKey(c => c.ShipId);

            modelBuilder
                .Entity<StarSystem>()
                .HasMany(s => s.OutgoingStargates)
                .WithOne(s => s.SrcStarSystem)
                .HasForeignKey(s => s.SrcStarSystemId);

            modelBuilder
                .Entity<StarSystem>()
                .HasMany(s => s.IncomingStargates)
                .WithOne(s => s.DstStarSystem)
                .HasForeignKey(s => s.DstStarSystemId);

            modelBuilder
                .Entity<Stargate>()
                .HasOne(s => s.SrcStarSystem)
                .WithMany(s => s.OutgoingStargates)
                .HasForeignKey(s => s.SrcStarSystemId);

            modelBuilder
                .Entity<Stargate>()
                .HasOne(s => s.DstStarSystem)
                .WithMany(s => s.IncomingStargates)
                .HasForeignKey(s => s.DstStarSystemId);

            modelBuilder
                .Entity<User>()
                .HasMany(u => u.InvitationKeys)
                .WithOne(ik => ik.GeneratedByUser)
                .HasForeignKey(u => u.GeneratedByUserId);
        }
    }
}