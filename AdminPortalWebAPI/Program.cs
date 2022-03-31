using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using McbaExample.Data;
using MvcMovie.Models.DataManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<McbaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("McbaContext")));

// Store session into Web-Server memory.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Make the session cookie essential.
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<AccountManager>();
builder.Services.AddScoped<CustomerManager>();
builder.Services.AddScoped<TransactionManager>();
builder.Services.AddScoped<LoginManager>();
builder.Services.AddScoped<BillPayManager>();

// Configure the default client.
builder.Services.AddHttpClient(Options.DefaultName, client =>
{
    client.BaseAddress = new Uri("http://localhost:5200");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

//// Seed data.
//using(var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        SeedData.Initialize(services);
//    }
//    catch(Exception ex)
//    {
//        var logger = services.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "An error occurred seeding the DB.");
//    }
//}

// Configure the HTTP request pipeline.
if(!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();


app.MapDefaultControllerRoute();

app.Run();
