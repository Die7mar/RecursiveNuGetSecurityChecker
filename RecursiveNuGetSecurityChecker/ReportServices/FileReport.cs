using RecursiveNuGetSecurityChecker.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecursiveNuGetSecurityChecker.ReportServices
{
    internal class FileReport
    {
        List<NugetCheckerResult> _nugetCheckerResults;
        string _fileName;

        public FileReport(List<NugetCheckerResult> nugetCheckerResults, string fileName)
        {
            _nugetCheckerResults = nugetCheckerResults;
            this._fileName = fileName;
        }

        public void WriteFile()
        {
            if (File.Exists(_fileName)) { File.Delete(_fileName); }

            foreach (var item in _nugetCheckerResults)
            {
                File.AppendAllText(_fileName, $"Found in: {item.ProjectName}\nPath: {item.ProjectPath}\nMessage: {item.Report}");
            }
        }
    }
}
