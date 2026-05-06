using System.Globalization;
using System.Text.Json;
using FormulaEngine.Api.Data;
using FormulaEngine.Api.Models;
using FormulaEngine.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FormulaEngine.Api.Middleware;

public class TenantResolutionMiddleware
{
    private const string TenantIdHeader         = "X-Tenant-Id";
    private const string TenantContextKey = TenantConstants.TenantContextKey;    private const string MissingTenantMessage   = "X-Tenant-Id header is required.";
    private const string InactiveTenantMessage  = "This tenant is inactive.";
    private const string NotFoundTenantMessage  = "Tenant not found.";
    private const int    CacheExpiryMinutes     = 5;

    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext          context,
        FormulaEngineContext db,
        IDistributedCache    cache)
    {
        var tenantId = context.Request.Headers[TenantIdHeader].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(MissingTenantMessage);
            return;
        }

        var tenant = await ResolveTenantAsync(tenantId, db, cache);

        if (tenant is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(NotFoundTenantMessage);
            return;
        }

        if (!tenant.IsActive)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync(InactiveTenantMessage);
            return;
        }

        var culture = CultureInfo.GetCultureInfo(tenant.CultureCode);
        CultureInfo.CurrentCulture   = culture;
        CultureInfo.CurrentUICulture = culture;

        context.Items[TenantContextKey] = tenant;

        await _next(context);
    }

    private static async Task<Tenant?> ResolveTenantAsync(
        string               tenantId,
        FormulaEngineContext db,
        IDistributedCache    cache)
    {
        var cacheKey    = $"tenant:{tenantId}";
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