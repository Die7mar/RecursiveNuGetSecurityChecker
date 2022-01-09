using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Configuration;
using RecursiveNuGetSecurityChecker.ReportServices;
using Serilog;
using RecursiveNuGetSecurityChecker.Config;
using RecursiveNuGetSecurityChecker.DataTransferObject;

namespace RecursiveNuGetSecurityChecker
{
    public class Program
    {
        static IConfigurationRoot? _configuration;
        static ILogger _logger;
        static NugetCheckerResultCheckTexts _nugetCheckerResultCheckTexts;

        public static void Init()
        {
            _configuration = AppsettingsHelper.ReadAppSetting();
            _nugetCheckerResultCheckTexts = _configuration.GetSection("NugetCheckerResultCheckTexts")
                                                          .Get<NugetCheckerResultCheckTexts>();
            _logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(_configuration)
                    .CreateLogger();
        }

        public static int Main(string[] args)
        {
            Init();

            var paths = _configuration.GetSection("Paths").Get<string[]>();
            List<NugetCheckerResult> nugetCheckerResults = new List<NugetCheckerResult>();

            foreach (var path in paths)
            {
                try
                {
                    _logger.Information("Check Path: " + path);
                    foreach (var project in Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories))
                    {
                        try
                        {
                            NugetChecker nugetChecker = new NugetChecker(project, _nugetCheckerResultCheckTexts);
                            var result = nugetChecker.Check();

                            switch (result.NugetCheckResult)
                            {
                                case NugetCheckerResultType.Passed:
                                    _logger.Information($"Project {result.ProjectName} passed!");
                                    break;
                                case NugetCheckerResultType.NotSupported:
                                    _logger.Warning($"Project {result.ProjectName} not supported!");
                                    break;
                                case NugetCheckerResultType.Failed:
                                default:
                                    _logger.Error($"Project {result.ProjectName} failed!:\n{result.Report}\n" +
                                                  $"ProjectPath: {Path.GetFullPath(project).Replace(path, "")}");
                                    nugetCheckerResults.Add(result);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"Error to check {project} ex: {ex.Message}");
                            nugetCheckerResults.Add(new NugetCheckerResult()
                            {
                                ProjectPath = path,
                                ProjectName = Path.GetFileName(project),
                                NugetCheckResult = NugetCheckerResultType.Failed,
                                StandardOut = "Programm Error",
                                StandardError = ex.Message
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error to check Path: {path} ex: {ex.Message}");

                    nugetCheckerResults.Add(new NugetCheckerResult()
                    {
                        ProjectPath = path,
                        ProjectName = "Folder error",
                        NugetCheckResult = NugetCheckerResultType.Failed,
                        StandardOut = "Programm Error",
                        StandardError = ex.Message
                    });
                }

            }

            _logger.Information("Fertig!");
            ReportManager.WriteReports(_logger, _configuration, nugetCheckerResults);

            return nugetCheckerResults.Count;
        }
    }
}