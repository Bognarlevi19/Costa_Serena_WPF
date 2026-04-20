using Costa_Serena_Grand_Hotel_WPF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Costa_Serena_Grand_Hotel_WPF.Data
{
    public class HotelDbContext : DbContext
    {
        public DbSet<Vendeg> Vendegek => Set<Vendeg>();
        public DbSet<Szoba> Szobak => Set<Szoba>();
        public DbSet<SzobaKategoria> SzobaKategoriak => Set<SzobaKategoria>();
        public DbSet<Foglalas> Foglalasok => Set<Foglalas>();
        public DbSet<Rendeles> Rendelesek => Set<Rendeles>();
        public DbSet<RendelesTetel> RendelesTetelek => Set<RendelesTetel>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var connectionString = configuration.GetConnectionString("HotelDb");
            optionsBuilder.UseMySQL(connectionString!);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vendeg>().ToTable("vendeg");
            modelBuilder.Entity<Szoba>().ToTable("szoba");
            modelBuilder.Entity<SzobaKategoria>().ToTable("szoba_kategoria");
            modelBuilder.Entity<Foglalas>().ToTable("foglalas");
            modelBuilder.Entity<Rendeles>().ToTable("rendeles");
            modelBuilder.Entity<RendelesTetel>().ToTable("rendeles_tetel");

            modelBuilder.Entity<Vendeg>().HasKey(x => x.Id);
            modelBuilder.Entity<Szoba>().HasKey(x => x.Id);
            modelBuilder.Entity<SzobaKategoria>().HasKey(x => x.Id);
            modelBuilder.Entity<Foglalas>().HasKey(x => x.Id);
            modelBuilder.Entity<Rendeles>().HasKey(x => x.Id);
            modelBuilder.Entity<RendelesTetel>().HasKey(x => x.Id);

            modelBuilder.Entity<Szoba>()
                .HasOne(x => x.SzobaKategoria)
                .WithMany(x => x.Szobak)
                .HasForeignKey(x => x.SzobaKategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Foglalas>()
                .HasOne(x => x.Szoba)
                .WithMany(x => x.Foglalasok)
                .HasForeignKey(x => x.SzobaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Foglalas>()
                .HasOne(x => x.Vendeg)
                .WithMany(x => x.Foglalasok)
                .HasForeignKey(x => x.VendegId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rendeles>()
                .HasOne(x => x.Vendeg)
                .WithMany()
                .HasForeignKey(x => x.VendegId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RendelesTetel>()
                .HasOne(x => x.Rendeles)
                .WithMany(x => x.Tetelek)
                .HasForeignKey(x => x.RendelesId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rendeles>()
                .Property(x => x.Letrehozva)
                .HasColumnType("datetime(6)");
        }
    }
}