# RecursiveNuGetSecurityChecker
This tool search recursive all folders in the appsettings.json for the csproj files and check it with the [NuGet Packages for Security Vulnerabilities](https://devblogs.microsoft.com/nuget/how-to-scan-nuget-packages-for-security-vulnerabilities/) for Security Vulnerabilities in nuget packages.
The results can be reported to a file, Discord and/or Teams.

## Configuration

- Paths: All your Path you want to check.
- NugetCheckerResultCheckTexts: For testing if the result a pass or a not supported you need a part of the message. (Use contains)
- Filename: The Filename for the text report. (Empty = no text report)
- ReportHour: Only on this hour of the day it will report to Discord and/or Teams. Example you have ReportHour = 5 o´clock and you run this program at 7 o´clock then you dosen´t became a Report for Discord and/or Teams. Only when you run the program on 5:10 or 5:12 … you became a report.
- DiscordWebHookUrl: [Your Discord Web Hook Url](https://support.discord.com/hc/en-us/articles/228383668-Intro-to-Webhooks)
- TeamsWebHookUrl: [Your Teams Web Hook Url](https://docs.microsoft.com/en-us/microsoftteams/platform/webhooks-and-connectors/how-to/add-incoming-webhook)