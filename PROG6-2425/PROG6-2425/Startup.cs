using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROG6_2425.Data;
using PROG6_2425.Models;
using PROG6_2425.Repositories;
using PROG6_2425.Services;
using PROG6_2425.Validators;
using PROG6_2425.ViewModels;

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
        
        services.AddDistributedMemoryCache(); 
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        
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
        services.AddScoped<IValidator<Step2VM>, BeestjeBoekingValidator>();
        
        //Discounts
        services.AddSingleton<DiscountService>();

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

        app.UseSession();
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
            SeedData.Initialize(serviceProvider).Wait();
        }
        
    }
}