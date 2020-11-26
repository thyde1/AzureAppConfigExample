namespace AzureAppConfigurationExample.FeatureFilters
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.FeatureManagement;
    using System.Threading.Tasks;

    [FilterAlias("Secret")]
    public class SecretFeatureFilter : IFeatureFilter
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public SecretFeatureFilter(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            return Task.FromResult(this.httpContextAccessor.HttpContext.Request.Query["secret"] == "chips");
        }
    }
}
