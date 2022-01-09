using RecursiveNuGetSecurityChecker.Config;
using RecursiveNuGetSecurityChecker.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RecursiveNuGetSecurityChecker
{
    internal class NugetChecker
    {
        public string PathToCsproj { get; set; }

        private string _stdOut = "";
        private string _stdErr = "";

        NugetCheckerResultCheckTexts _nugetCheckerResultCheckTexts;

        public NugetChecker(string pathToCsproj, NugetCheckerResultCheckTexts nugetCheckerResultCheckTexts)
        {
            this.PathToCsproj = pathToCsproj;
            this._nugetCheckerResultCheckTexts = nugetCheckerResultCheckTexts;
        }

        public NugetCheckerResult Check()
        {
            NugetCheckerResult result = new NugetCheckerResult();
            StartCommand();

            result.ProjectName = Path.GetFileName(PathToCsproj).Replace(".csproj", "");
            result.ProjectPath = Path.GetDirectoryName(PathToCsproj);
            result.StandardOut = _stdOut;
            result.StandardError = _stdErr;
            result.NugetCheckResult = GetNugetResult(result.Report);

            return result;
        }

        private NugetCheckerResultType GetNugetResult(string report)
        {
            NugetCheckerResultType output = NugetCheckerResultType.Failed;

            foreach (var item in _nugetCheckerResultCheckTexts.NotSupported )
            {
                if (report.Contains(item)) { return NugetCheckerResultType.NotSupported; }
            }

            foreach (var item in _nugetCheckerResultCheckTexts.Passed)
            {
                if (report.Contains(item)) { return NugetCheckerResultType.Passed; }
            }

            return output;
        }

        private void StartCommand()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                startCmdCommandAndGetOutput("dotnet", $"list \"{PathToCsproj}\" package --vulnerable");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                startCmdCommandAndGetOutput("cmd.exe", $"/C dotnet list \"{PathToCsproj}\" package --vulnerable");
            }
        }

        private void startCmdCommandAndGetOutput(string fileName, string command, bool windowHidden = true, string WorkingDirectory = "")
        {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            //p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;

            if (windowHidden)
            {
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }


            p.StartInfo.FileName = fileName;
            p.StartInfo.Arguments = command;

            if (WorkingDirectory != string.Empty)
            {
                p.StartInfo.WorkingDirectory = WorkingDirectory;
            }

            p.Start();
            _stdErr = p.StandardError.ReadToEnd();
            _stdOut = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

        }
    }
}
