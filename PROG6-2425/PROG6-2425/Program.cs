using PROG6_2425;

var builder = WebApplication.CreateBuilder(args);

// Instantiate the Startup class
var startup = new Startup(builder.Configuration, builder.Environment);

// Configure services using the Startup class
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Configure middleware using the Startup class
startup.Configure(app);

app.Run();