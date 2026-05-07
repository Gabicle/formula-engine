using FormulaEngine.Api.Data;
using FormulaEngine.Api.Models;
using FormulaEngine.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Text.Json;

namespace FormulaEngine.Api.Middleware;

public class TenantResolutionMiddleware
{
    private const string TenantIdHeader = "X-Tenant-Id";
    private const int CacheExpiryMinutes = 5;
    private const string TenantNotFoundMessage = "Tenant not found";
    private const string MissingTenantIdMessage = "X-Tenant-Id header is required";


    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        FormulaEngineContext db,
        IDistributedCache cache,
        IStringLocalizer<ErrorMessages> localizer)
    {
        var tenantId = context.Request.Headers[TenantIdHeader].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(MissingTenantIdMessage);
            return;
        }

        var tenant = await ResolveTenantAsync(tenantId, db, cache);

        if (tenant is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(TenantNotFoundMessage);
            return;
        }

        var culture = CultureInfo.GetCultureInfo(tenant.CultureCode);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        if (!tenant.IsActive)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync(localizer["TenantInactive"]);
            return;
        }

        context.Items[TenantConstants.TenantContextKey] = tenant;

        await _next(context);
    }

    private static async Task<Tenant?> ResolveTenantAsync(
        string tenantId,
        FormulaEngineContext db,
        IDistributedCache cache)
    {
        var cacheKey = $"tenant:{tenantId}";
        var cachedBytes = await cache.GetAsync(cacheKey);

        if (cachedBytes is not null)
            return JsonSerializer.Deserialize<Tenant>(cachedBytes);

        var tenant = await db.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId);

        if (tenant is null)
            return null;

        var options = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(CacheExpiryMinutes)
        };

        await cache.SetAsync(
            cacheKey,
            JsonSerializer.SerializeToUtf8Bytes(tenant),
            options);

        return tenant;
    }
}
