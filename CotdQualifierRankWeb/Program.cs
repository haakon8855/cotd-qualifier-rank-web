using CotdQualifierRankWeb.Controllers;
using CotdQualifierRankWeb.Data;
using CotdQualifierRankWeb.Services;
using CotdQualifierRankWeb.Utils;
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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<LeaderboardQueueWorker>();

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapRazorPages();
app.MapControllers();

app.Run();
