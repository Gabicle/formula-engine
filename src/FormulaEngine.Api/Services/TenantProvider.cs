using FormulaEngine.Api.Models;

namespace FormulaEngine.Api.Services;

public class TenantProvider : ITenantProvider
{
    private const string TenantContextKey = TenantConstants.TenantContextKey; 
    private const string TenantNotResolvedMessage = "Tenant has not been resolved for this request.";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Tenant GetTenant()
    {
        var tenant = _httpContextAccessor.HttpContext?.Items[TenantContextKey] as Tenant;
        return tenant ?? throw new InvalidOperationException(TenantNotResolvedMessage);
    }
}