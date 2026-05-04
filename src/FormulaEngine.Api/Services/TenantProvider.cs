namespace FormulaEngine.Api.Services;

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetTenantId()
    {
        var tenantId = _httpContextAccessor.HttpContext?
            .Request.Headers["X-Tenant-Id"]
            .FirstOrDefault();

        return string.IsNullOrEmpty(tenantId) ? throw new UnauthorizedAccessException("Tenant ID is required.") : tenantId;
    }
}