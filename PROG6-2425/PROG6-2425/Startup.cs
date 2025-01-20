using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROG6_2425.Data;
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
        services.AddDefaultIdentity<IdentityUser> 
            (options => 
            { 
                options.SignIn.RequireConfirmedAccount = false; 
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<BeestFeestDbContext>();
            
        services.AddScoped<IAccountRepository, AccountRepository>();
        
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
    
    private static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        
        var adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        // Add admin if needed
        var adminEmail = "admin@gmail.com";
        var adminPassword = "Admin@123";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            var createUserResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (createUserResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
}