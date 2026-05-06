using FormulaEngine.Api.Models;

namespace FormulaEngine.Api.Services;

public interface ITenantProvider
{
    Tenant GetTenant();
}