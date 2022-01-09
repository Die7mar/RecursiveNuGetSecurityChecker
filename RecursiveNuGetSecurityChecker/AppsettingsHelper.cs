using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecursiveNuGetSecurityChecker
{
    internal class AppsettingsHelper
    {
        public static IConfigurationRoot ReadAppSetting()
        {
            string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json");
            if (environment == "Development")
            {
                builder.AddJsonFile($"appsettings.{environment}.json", optional: true);
            }
            else if (!string.IsNullOrEmpty(environment))
            {
                builder.AddJsonFile($"appsettings.{environment}.json", optional: false);
            }

            return builder.Build();
        }
    }
}
