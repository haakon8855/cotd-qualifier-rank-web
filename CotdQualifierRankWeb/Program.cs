using CotdQualifierRankWeb.Controllers;
using CotdQualifierRankWeb.Data;
using CotdQualifierRankWeb.Services;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<CredentialsManager>();
builder.Services.AddDbContext<CotdContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("CotdContext") ?? throw new InvalidOperationException("Connection string 'CotdContext' not found.")));
builder.Services.AddSingleton<NadeoApiController>();
builder.Services.AddScoped<NadeoCompetitionService>();
builder.Services.AddScoped<CompetitionService>();
builder.Services.AddScoped<RankController>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CotdContext>();
    //context.Database.EnsureCreated();
    //DbInitializer.Initialize(context);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
