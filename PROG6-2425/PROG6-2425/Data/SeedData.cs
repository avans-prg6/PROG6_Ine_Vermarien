using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROG6_2425.Data;
using PROG6_2425.Models;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<Account>>();

        var adminRole = "Admin";

        // Controleer of de rol al bestaat, zo niet maak deze dan aan
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        // Admin-gebruiker maken als deze nog niet bestaat
        var adminEmail = "admin@gmail.com";
        var adminPassword = "Admin@123";
        var adminPhone = "0612556677";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new Account
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                PhoneNumber = adminPhone,
                Naam = "Admin User", // Naam voor de admin
                Adres = "Adminstraat 123, Amsterdam", // Fictief adres
                KlantenKaart = new KlantenKaart
                {
                    KlantenKaartTypeId = 4 // Platina klantenkaart voor de admin
                }
            };

            var createUserResult = await userManager.CreateAsync(adminUser, adminPassword);

            if (createUserResult.Succeeded)
            {
                // Voeg de gebruiker toe aan de admin rol
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }

        using (var context = serviceProvider.GetRequiredService<BeestFeestDbContext>())
        {
            context.Database.Migrate();

            // klantenkaarten
            if (!context.KlantenKaartTypes.Any())
            {
                context.KlantenKaartTypes.AddRange(
                    new KlantenKaartType { Id = 1, Naam = "Geen" },
                    new KlantenKaartType { Id = 2, Naam = "Zilver" },
                    new KlantenKaartType { Id = 3, Naam = "Goud" },
                    new KlantenKaartType { Id = 4, Naam = "Platina" }
                );
                await context.SaveChangesAsync();
            }


            var users = new List<(string email, string naam, string adres, string telefoon, int klantenKaartId)>
            {
                ("zilver@example.com", "zilver Doe", "Straat 1, Amsterdam", "0612345678", 2), // Zilver
                ("goud@example.com", "goud Smith", "Straat 2, Rotterdam", "0623456789", 3), // Goud
                ("platina@example.com", "platina de Bakker", "Straat 3, Utrecht", "0634567890", 4), // Platina
                ("geen@example.com", "Test User", "Straat 4, Den Haag", "0645678901", 1) // Geen
            };

            foreach (var (email, naam, adres, telefoon, klantenKaartId) in users)
            {
                var user = new Account
                {
                    UserName = email,
                    Email = email,
                    Naam = naam,
                    Adres = adres,
                    TelefoonNummer = telefoon,
                    KlantenKaart = new KlantenKaart
                    {
                        KlantenKaartTypeId = klantenKaartId
                    }
                };

                // Voeg de gebruiker toe met een standaard wachtwoord
                await userManager.CreateAsync(user, "Test1@34");
            }

            if (!context.Beestjes.Any())
            {
                context.Beestjes.AddRange(
                    new Beestje { Naam = "Aap", Type = "Jungle", Prijs = 10.00m, AfbeeldingUrl = "/jungle_aap" },
                    new Beestje { Naam = "Olifant", Type = "Jungle", Prijs = 15.00m, AfbeeldingUrl = "/jungle_olifant" },
                    new Beestje { Naam = "Zebra", Type = "Jungle", Prijs = 12.00m, AfbeeldingUrl = "/jungle_zebra" },
                    new Beestje { Naam = "Leeuw", Type = "Jungle", Prijs = 18.00m, AfbeeldingUrl = "/jungle_leeuw" },
                    
                    new Beestje { Naam = "Hond", Type = "Boerderij", Prijs = 8.00m, AfbeeldingUrl = "/boerderij_hond" },
                    new Beestje { Naam = "Ezel", Type = "Boerderij", Prijs = 9.00m, AfbeeldingUrl = "/boerderij_ezel" },
                    new Beestje { Naam = "Koe", Type = "Boerderij", Prijs = 11.00m, AfbeeldingUrl = "/boerderij_koe" },
                    new Beestje { Naam = "Eend", Type = "Boerderij", Prijs = 7.00m, AfbeeldingUrl = "/boerderij_eend" },
                    new Beestje { Naam = "Kuiken", Type = "Boerderij", Prijs = 6.00m, AfbeeldingUrl = "/boerderij_kuiken" },
                    
                    new Beestje { Naam = "Pinguïn", Type = "Sneeuw", Prijs = 14.00m, AfbeeldingUrl = "/sneeuw_pinguin" },
                    new Beestje { Naam = "IJsbeer", Type = "Sneeuw", Prijs = 20.00m, AfbeeldingUrl = "/sneeuw_ijsbeer" },
                    new Beestje { Naam = "Zeehond", Type = "Sneeuw", Prijs = 13.00m, AfbeeldingUrl = "/sneeuw_zeehond" },
                    
                    new Beestje { Naam = "Kameel", Type = "Woestijn", Prijs = 16.00m, AfbeeldingUrl = "/woestijn_kameel" },
                    new Beestje { Naam = "Slang", Type = "Woestijn", Prijs = 10.00m, AfbeeldingUrl = "/woestijn_slang" },
                    
                    new Beestje { Naam = "T-Rex", Type = "VIP", Prijs = 100.00m, AfbeeldingUrl = "/vip_trex" },
                    new Beestje { Naam = "Unicorn", Type = "VIP", Prijs = 150.00m, AfbeeldingUrl = "/vip_unicorn" }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}