using Discord;
using Discord.Rest;
using Discord.WebSocket;
using RecursiveNuGetSecurityChecker.DataTransferObject;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RecursiveNuGetSecurityChecker.ReportServices
{
    class DiscordReport
    {
        private string _url;
        List<NugetCheckerResult> _nugetCheckerResults;

        public DiscordReport(string url, List<NugetCheckerResult> nugetCheckerResults)
        {
            _url = url;
            _nugetCheckerResults = nugetCheckerResults;
        }

        public void SendReport()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("username", "NugetChecker");
            headers.Add("content", GetConent());

            var content = new FormUrlEncodedContent(headers);

            HttpClient client = new HttpClient();
            var respone = client.PostAsync(_url, content);
            respone.Wait();
        }

        private string GetConent()
        {
            string output = "";
            if (_nugetCheckerResults.Count() == 0)
            {
                return "All Projects are ok!";
            }

            foreach (var item in _nugetCheckerResults)
            {
                output += $"Found in: {item.ProjectName}\nPath: {item.ProjectPath}\nMessage: {item.Report}";
            }

            return output;
        }

    }

}
