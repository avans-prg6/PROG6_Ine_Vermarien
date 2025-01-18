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
        
        
    }
}