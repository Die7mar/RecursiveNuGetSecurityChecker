namespace RecursiveNuGetSecurityChecker.DataTransferObject
{
    public class NugetCheckerResult
    {
        public string? ProjectName { get; set; }
        public string? ProjectPath { get; set; }

        public string? StandardOut { get; set; }
        public string? StandardError { get; set; }

        public string Report => StandardError + StandardOut;


        public NugetCheckerResultType NugetCheckResult { get; set; }

    }

    public enum NugetCheckerResultType
    {
        Passed,
        Failed,
        NotSupported
    }
}