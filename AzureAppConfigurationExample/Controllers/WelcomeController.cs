namespace AzureAppConfigurationExample.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [ApiController]
    [Route("[controller]")]
    public class WelcomeController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public WelcomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public string Get()
        {
            var greeting = configuration.GetValue<string>("greeting");
            var environmentDetail = configuration.GetValue<string>("environmentDetail");
            var key = configuration.GetValue<string>("key");
            return $"{greeting}\n{environmentDetail}\n(The key is {key})";
        }
    }
}
