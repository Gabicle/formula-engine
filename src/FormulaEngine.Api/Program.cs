using FormulaEngine.Api.Data;
using FormulaEngine.Api.Engine.Functions;
using FormulaEngine.Api.Engine.Functions.Languages;
using FormulaEngine.Api.Engine.Localization;
using FormulaEngine.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddSingleton<IFunctionLanguage, EnglishFunctions>();
builder.Services.AddSingleton<IFunctionLanguage, FrenchFunctions>();
builder.Services.AddSingleton<FunctionRegistry>();

builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddScoped<ILocaleSettings, CultureLocaleSettings>();

builder.Services.AddDbContext<FormulaEngineContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();