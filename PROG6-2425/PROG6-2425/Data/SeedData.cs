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
                Console.Write("klantenkaarttypes seeded");
                await context.SaveChangesAsync();
            }


            var users = new List<(string email, string naam, string adres, string telefoon, int klantenKaartId)>
            {
                ("zilver@example.com", "zilver Doe", "Straat 1, Amsterdam", "0612345678", 2), // Zilver
                ("goud@example.com", "goud Smith", "Straat 2, Rotterdam", "0623456789", 3), // Goud
                ("platina@example.com", "platina de Bakker", "Straat 3, Utrecht", "0634567890", 4), // Platina
                ("geen@example.com", "Test User", "Straat 4, Den Haag", "0645678901", 1)  // Geen
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
            
           
        }
    }
}