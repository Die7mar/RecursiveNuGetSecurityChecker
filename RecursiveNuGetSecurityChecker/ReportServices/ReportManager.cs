using Microsoft.Extensions.Configuration;
using RecursiveNuGetSecurityChecker.Config;
using RecursiveNuGetSecurityChecker.DataTransferObject;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecursiveNuGetSecurityChecker.ReportServices
{
    class ReportManager
    {
        public static void WriteReports(ILogger _logger, IConfigurationRoot? _configuration, List<NugetCheckerResult> nugetCheckerResults)
        {
            try
            {
                var logFileName = _configuration["FileName"];
                if (!String.IsNullOrEmpty(logFileName))
                {
                    FileReport fileReport = new FileReport(nugetCheckerResults, logFileName);
                    fileReport.WriteFile();
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error to report to text ex: {ex.Message}");
            }


            try
            {
                if (!String.IsNullOrEmpty(_configuration["ReportHour"]))
                {
                    var reportHour = Convert.ToInt32(_configuration["ReportHour"]);

                    if (reportHour != DateTime.Now.Hour)
                    {
                        //Report to Discord and Teams only when reportHour match with current hour
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error to get the report hour ex: {ex.Message}");
            }

            try
            {
                var discordWebHookUrl = _configuration.GetSection("DiscordWebHookUrl").Get<string>();
                if (!String.IsNullOrEmpty(discordWebHookUrl))
                {
                    DiscordReport discordReport = new DiscordReport(discordWebHookUrl, nugetCheckerResults);
                    discordReport.SendReport();
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error to Report to Discord ex: {ex.Message}");
            }


            try
            {
                var teamsWebHookUrl = _configuration.GetSection("TeamsWebHookUrl").Get<string>();
                if (!String.IsNullOrEmpty(teamsWebHookUrl))
                {
                    TeamsWebHook teamsWebHook = new TeamsWebHook(teamsWebHookUrl, _logger);
                    var task = teamsWebHook.SendMessage(nugetCheckerResults);
                    task.Wait();
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error to Report to Teams ex: {ex.Message}");
            }
        }

    }
}
