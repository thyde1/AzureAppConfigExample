namespace AzureAppConfigurationExample.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;
    using System.Threading.Tasks;

    [ApiController]
    [Route("[controller]")]
    public class WelcomeController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IFeatureManager featureManager;

        public WelcomeController(IConfiguration configuration, IFeatureManager featureManager)
        {
            this.configuration = configuration;
            this.featureManager = featureManager;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var greeting = configuration.GetValue<string>("greeting");
            var environmentDetail = configuration.GetValue<string>("environmentDetail");
            var key = configuration.GetValue<string>("key");
            var featureString = await this.featureManager.IsEnabledAsync("CoolNewFeature") ? "This is cool and new" : "This is old and boring";
            var betaString = await this.featureManager.IsEnabledAsync("InterestingBetaFeature") ? "You are one of the lucky ones to try our new beta thingy" : "";
            return $"{greeting}\n{environmentDetail}\n(The key is {key})\n{featureString}\n{betaString}";
        }
    }
}
