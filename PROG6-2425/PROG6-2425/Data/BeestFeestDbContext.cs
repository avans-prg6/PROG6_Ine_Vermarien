using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PROG6_2425.Models;

namespace PROG6_2425.Data;

public class BeestFeestDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public BeestFeestDbContext(DbContextOptions<BeestFeestDbContext> options) : base(options)
    {
    }

    public DbSet<Beestje> Beestjes { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Boeking> Boekingen { get; set; }
    public DbSet<KlantenKaart> KlantenKaarten { get; set; }

    public DbSet<KlantenKaartType> KlantenKaartTypes { get; set; }

    public DbSet<BeestjeBoeking> BeestjeBoekingen { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuratie voor Beestje
        modelBuilder.Entity<Beestje>()
            .HasKey(b => b.BeestjeId);

        modelBuilder.Entity<Beestje>()
            .Property(b => b.Naam)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Beestje>()
            .Property(b => b.Type)
            .IsRequired();

        modelBuilder.Entity<Beestje>()
            .Property(b => b.Prijs)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Beestje>()
            .Property(b => b.AfbeeldingUrl)
            .HasMaxLength(255);

        // Configuratie voor Account
        modelBuilder.Entity<Account>()
            .Property(a => a.Naam)
            .HasMaxLength(100);

        modelBuilder.Entity<Account>()
            .Property(a => a.Adres)
            .HasMaxLength(255);

        modelBuilder.Entity<Account>()
            .Property(a => a.TelefoonNummer)
            .HasMaxLength(15);

        // Configuratie voor KlantenKaartType
        modelBuilder.Entity<KlantenKaartType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Naam)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<KlantenKaartType>().HasData(
            new KlantenKaartType { Id = 1, Naam = "Geen" },
            new KlantenKaartType { Id = 2, Naam = "Zilver" },
            new KlantenKaartType { Id = 3, Naam = "Goud" },
            new KlantenKaartType { Id = 4, Naam = "Platina" }
        );

        // Configuratie voor KlantenKaart
        modelBuilder.Entity<KlantenKaart>()
            .HasKey(k => k.Id);

        modelBuilder.Entity<KlantenKaart>()
            .HasOne(k => k.KlantenKaartType)
            .WithMany()
            .HasForeignKey(k => k.KlantenKaartTypeId)
            .OnDelete(DeleteBehavior.Restrict); // Voorkom cascade delete

        modelBuilder.Entity<KlantenKaart>()
            .HasOne(k => k.Account)
            .WithOne(u => u.KlantenKaart)
            .HasForeignKey<KlantenKaart>(k => k.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configuratie voor Boeking
        modelBuilder.Entity<Boeking>()
            .HasKey(b => b.BoekingId);

        modelBuilder.Entity<Boeking>()
            .Property(b => b.Datum)
            .IsRequired();

        modelBuilder.Entity<Boeking>()
            .Property(b => b.UiteindelijkePrijs)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Boeking>()
            .Property(b => b.KortingPercentage)
            .HasColumnType("decimal(5,2)");

        modelBuilder.Entity<Boeking>()
            .Property(b => b.Naam)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Boeking>()
            .Property(b => b.Adres)
            .HasMaxLength(255);

        modelBuilder.Entity<Boeking>()
            .Property(b => b.Email)
            .HasMaxLength(255);

        modelBuilder.Entity<Boeking>()
            .Property(b => b.Telefoonnummer)
            .HasMaxLength(15);

        // Relatie tussen Boeking en BeestjeBoeking (many-to-many)
        modelBuilder.Entity<BeestjeBoeking>()
            .HasKey(bb => bb.BeestjeBoekingId);

        modelBuilder.Entity<BeestjeBoeking>()
            .Property(bb => bb.Prijs)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<BeestjeBoeking>()
            .HasOne(bb => bb.Beestje)
            .WithMany(b => b.Boekingen)
            .HasForeignKey(bb => bb.BeestjeId)
            .OnDelete(DeleteBehavior.Restrict); // Voorkom cascade delete

        modelBuilder.Entity<BeestjeBoeking>()
            .HasOne(bb => bb.Boeking)
            .WithMany(b => b.Beestjes)
            .HasForeignKey(bb => bb.BoekingId)
            .OnDelete(DeleteBehavior.Cascade); // Verwijder boeking niet als beestje wordt verwijderd

        // Relatie tussen Boeking en Account
        modelBuilder.Entity<Boeking>()
            .HasOne(b => b.Account)
            .WithMany() // Account kan meerdere boekingen hebben
            .HasForeignKey(b => b.AccountId)
            .OnDelete(DeleteBehavior.Restrict); // Voorkom cascade delete als Account wordt verwijderd
    }
}