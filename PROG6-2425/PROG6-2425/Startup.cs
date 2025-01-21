using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROG6_2425.Data;
using PROG6_2425.Models;
using PROG6_2425.Repositories;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    // Configure services DI
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();
        
        // DbContext
        services.AddDbContext<BeestFeestDbContext>(options =>
            options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

        // Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        // identity settings
        services.AddIdentity<Account, IdentityRole>
            (options => 
            { 
                options.SignIn.RequireConfirmedAccount = false; 
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<BeestFeestDbContext>()
            .AddDefaultTokenProviders();
            
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IBoekingRepository, BoekingRepository>();
        services.AddScoped<IBeestjeRepository, BeestjeRepository>();

    }

    // Configure middleware (HTTP pipeline)
    public void Configure(WebApplication app)
    {
        // Development environment-specific configuration
        if (_environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        // Common middleware
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        // app.UseSession();
        // Map controller routes
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
        // Map fallback route
        app.MapFallbackToFile("index.html");
        
        using (var scope = app.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            SeedRolesAndAdminAsync(serviceProvider).Wait();
        }
        
    }
    
    // Admin maken maar alleen wanneer er nog geen admin bestaat. Dit wordt elke keer gecheckt
    private static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<Account>>();
        
        var adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        // Admin opzetten als deze niet bestaat
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
                PhoneNumber = adminPhone
            };
            var createUserResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (createUserResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
}