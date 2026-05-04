using FormulaEngine.Api.Data;
using FormulaEngine.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

// Register tenant provider
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

// Register DbContext
builder.Services.AddDbContext<FormulaEngineContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();