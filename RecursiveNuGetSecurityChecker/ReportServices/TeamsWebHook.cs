using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RecursiveNuGetSecurityChecker.Config;
using RecursiveNuGetSecurityChecker.DataTransferObject;
using Serilog;
using static RecursiveNuGetSecurityChecker.DataTransferObject.TeamsWebHookClass;

namespace RecursiveNuGetSecurityChecker.ReportServices
{
    class TeamsWebHook
    {
        private string _url;
        private ILogger _logger;


        public TeamsWebHook(string url, ILogger logger)
        {
            _url = url;
            _logger = logger;
        }


        public async Task SendMessage(List<NugetCheckerResult> nugetCheckerResults, int attempt = 0)
        {
            try
            {
                Random Random = new Random();
                using (HttpClient client = new HttpClient())
                {
                    var settings = new JsonSerializerSettings();
                    settings.ContractResolver = new LowercaseContractResolver();

                    var content = GetContent(nugetCheckerResults);

                    var json = JsonConvert.SerializeObject(content, Formatting.Indented, settings);

                    var httpResponseMessage = await client.PostAsync(_url, new StringContent(json));

                    var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                    if (responseContent.Contains("Microsoft Teams endpoint returned HTTP error 429"))
                    {
                        if (attempt > 10)
                        {
                            throw new Exception("Cant send messages after 10 attempts");
                        }
                        Thread.Sleep(Random.Next(2000, 5000));
                        SendMessage(nugetCheckerResults, attempt++);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error to send to Teams ex: " + ex.Message);
            }
        }
        
        private TeamsWebHookClass GetContent(List<NugetCheckerResult> nugetCheckerResults)
        {
            TeamsWebHookClass message = new TeamsWebHookClass();
            message.Title = $"Nuget Security Vulnerabilities Checker";
            message.Summary = message.Title;
            message.ThemeColor = "#CC0000";
            message.Sections = new List<SectionsObj>();

            if (nugetCheckerResults.Count == 0)
            {
                SectionsObj sectionsObj = new SectionsObj();
                sectionsObj.Text = "All Projects are ok!";
                message.Sections.Add(sectionsObj);
            }

            foreach (var item in nugetCheckerResults)
            {
                SectionsObj sectionsObj = new SectionsObj();
                sectionsObj.Text = item.ProjectName;

                List<Facts> facts = new List<Facts>();
                facts.Add(new Facts("ProjectPath: ", item.ProjectPath ));
                facts.Add(new Facts("NugetCheckResult: ", item.NugetCheckResult.ToString()));
                facts.Add(new Facts("Message: ", item.Report));
                facts.Add(new Facts("- ", "-"));

                sectionsObj.Facts = facts;
                message.Sections.Add(sectionsObj);
            }
            return message;
        }
    }


    public class LowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}
