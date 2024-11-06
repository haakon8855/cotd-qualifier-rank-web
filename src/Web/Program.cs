using Azure.Identity;
using CotdQualifierRank.Database;
using CotdQualifierRank.Application.Data;
using CotdQualifierRank.Application.Repositories;
using CotdQualifierRank.Application.Services;
using CotdQualifierRank.Application.Utils;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

if (builder.Environment.IsDevelopment())
{
    // Use User Secrets
    config.AddUserSecrets<Program>();
    // Use credentials.json
    config.AddJsonFile("credentials.json", optional: true, reloadOnChange: true);
}
else
{
    // Use Azure Key Vault for production
    var keyVaultEndpoint = new Uri("https://cotd-qualifier-rank-keys.vault.azure.net/");
    config.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
}

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<CotdContext>(options =>
    options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));

builder.Services.AddSingleton<NadeoCredentialsManager>();
builder.Services.AddSingleton<NadeoApiService>();
builder.Services.AddScoped<NadeoCompetitionService>();
builder.Services.AddScoped<CompetitionService>();
builder.Services.AddScoped<CotdRepository>();
builder.Services.AddScoped<RankService>();

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
