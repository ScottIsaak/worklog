using Mono.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wl
{
    class Program
    {
        enum Status
        {
            Posted,
            Failed,
            Skipped
        }

        static void Main(string[] args)
        {
            var logFilePaths = new List<string>();
            bool calculateOnly = false;
            bool showHelp = false;
            string tempoToken = Environment.GetEnvironmentVariable("TempoToken");
            string jiraUsername = Environment.GetEnvironmentVariable("JiraUsername");
            string jiraAccountId = Environment.GetEnvironmentVariable("JiraAccountId");
            string jiraAccessToken = Environment.GetEnvironmentVariable("JiraAccessToken");
            bool addIssueNames = false;

            Console.OutputEncoding = Encoding.UTF8;

            var p = new OptionSet()
            {
                { "l|log=", "The path to a log file to parse. Multiple -l options can be specified on the command line.",
                    l => logFilePaths.Add(l) },

                { "c|calculate", "Calculate hours only. Do not post work logs to Tempo.",
                    c => calculateOnly = (c != null)},

                { "t|tempoToken=", "Set the token value for the api call. Or use the TempoToken environment variable. Create one at: https://rollick.atlassian.net/plugins/servlet/ac/io.tempo.jira/tempo-app#!/configuration/api-integration",
                    t => tempoToken = t },

                { "u|jiraUsername=", "Set the username for the api call. Or use the JiraUsername environment variable. Get the value from your Atlassian profile page.",
                    u => jiraUsername = u },

                { "d|jiraAccountId=", "Set the account id for the api call. Or use the JiraAccountId environment variable. Get the value from your Atlassian profile page URL.",
                    d => jiraAccountId = d},

                { "i|addIssueNames", "Add the issue name to the description.",
                    i => addIssueNames = (i != null) },

                { "j|jiraAccessToken=", "Set the access token for the api call. Or use the JiraAccessToken environment variable. Create one at: https://id.atlassian.com/manage-profile/security/api-tokens",
                    j => jiraAccessToken = j },

                { "h|help", "Show this message.",
                    h => showHelp = (h != null) }
            };
            
            try
            {
                p.Parse(args);
                if (!showHelp && !logFilePaths.Any())
                    throw new OptionException("At least one log is required.", "log");
            }
            catch (OptionException ex)
            {
                Console.Write("wl: ");
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine();
                ShowHelp(p);
                return;
            }

            if (showHelp)
            {
                ShowHelp(p);
                return;
            }

            var missing = new List<string>();
            if (addIssueNames || !calculateOnly)
            {
                if (string.IsNullOrWhiteSpace(jiraUsername)) missing.Add("JiraUsername (or -u): your Atlassian account email.");
                if (string.IsNullOrWhiteSpace(jiraAccessToken)) missing.Add("JiraAccessToken (or -j): create one at https://id.atlassian.com/manage-profile/security/api-tokens");
            }
            if (!calculateOnly)
            {
                if (string.IsNullOrWhiteSpace(tempoToken)) missing.Add("TempoToken (or -t): create one at https://rollick.atlassian.net/plugins/servlet/ac/io.tempo.jira/tempo-app#!/configuration/api-integration");
                if (string.IsNullOrWhiteSpace(jiraAccountId)) missing.Add("JiraAccountId (or -d): the id at the end of your Atlassian profile page URL.");
            }
            if (missing.Any())
            {
                WriteError("wl: Missing required credentials. Set these environment variables or pass the options:");
                missing.ForEach(m => WriteError("  " + m));
                Environment.ExitCode = 1;
                return;
            }

            var service = new Tempo.Client(jiraAccountId, jiraUsername, tempoToken, jiraAccessToken);

            foreach (var logFilePath in logFilePaths)
            {
                if (!File.Exists(logFilePath))
                {
                    WriteError($"wl: Log file not found: {logFilePath}");
                    Environment.ExitCode = 1;
                    continue;
                }

                var logs = GetWorklogs(logFilePath);
                try
                {
                    PostWorklogs(service, logs, calculateOnly, addIssueNames);
                }
                catch (Tempo.ApiException ex)
                {
                    Console.WriteLine();
                    WriteError("wl: " + ex.Message);
                    Environment.ExitCode = 1;
                    return;
                }
            }
        }

        static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
            Console.ResetColor();
        }

        static WorkLogCollection GetWorklogs(string logFilePath)
        {
            var logs = new WorkLogCollection();
            var line = 1;

            using (var wr = new WorkLogReader(logFilePath))
            {
                while (!wr.EndOfStream)
                {
                    var log = wr.ReadWorkLog();

                    if (log != null) logs.Add(log);

                    line++;
                }
            }

            logs.Remove(logs.Last()); // The last worklog is just an end time.
            return logs;
        }

        static void PostWorklogs(Tempo.Client service, WorkLogCollection logs, bool calculateOnly, bool addIssueNames)
        {
            var apiErrors = new List<string>();

            Console.WriteLine("Worklogs:");
            foreach (var log in logs)
            {
                if (addIssueNames)
                {
                    try
                    {
                        service.AddIssueDetails(log);
                    }
                    catch (Tempo.ApiException ex) when (!ex.IsAuthError)
                    {
                        apiErrors.Add(ex.Message);
                    }
                }

                var logText = log.ToString();
                var width = 80;
                try
                {
                    width = Console.BufferWidth - 1;
                }
                catch { }
                var status = Status.Skipped;

                if (Console.IsOutputRedirected)
                {
                    // Cursor positioning isn't available, so write the status before the log text.
                    status = PostWorklog(service, log, calculateOnly, apiErrors);
                    Console.WriteLine("{0} {1}", StatusChar(status), logText);
                    continue;
                }

                if (logText.Length > width) logText = string.Concat(logText.Substring(0, width - 3), "...");

                Console.Write(logText);

                status = PostWorklog(service, log, calculateOnly, apiErrors);

                try
                {
                    WriteStatus(status);
                }
                catch 
                {
                    Console.WriteLine();
                }
            }

            ShowSummary(logs);

            if (apiErrors.Any())
            {
                apiErrors.Distinct().ToList().ForEach(e => WriteError("wl: " + e));
                Environment.ExitCode = 1;
            }
        }

        static Status PostWorklog(Tempo.Client service, WorkLog log, bool calculateOnly, List<string> apiErrors)
        {
            if (calculateOnly) return Status.Skipped;

            try
            {
                return service.CreateWorkLog(log) ? Status.Posted : Status.Failed;
            }
            catch (Tempo.ApiException ex) when (!ex.IsAuthError)
            {
                apiErrors.Add($"{log.IssueKey}: {ex.Message}");
                return Status.Failed;
            }
        }

        static void WriteStatus(Status status)
        {
            Console.CursorLeft = 0;
            Console.Write(StatusChar(status));
            Console.CursorLeft = Console.BufferWidth - 1;
            Console.WriteLine();
        }

        static char StatusChar(Status status)
        {
            switch (status)
            {
                case Status.Failed: return 'X';
                case Status.Posted: return '√';
                default: return '-';
            }
        }

        static void ShowSummary(WorkLogCollection logs)
        {
            var totalCount = logs.Count;
            var totalDuration = TimeSpan.FromMinutes(logs.Where(l => !string.IsNullOrEmpty(l.Project)).Sum(l => l.Minutes));

            var groups = logs
                .GroupBy(l => l.Project)
                .Select(g => new 
                { 
                    Project = string.IsNullOrEmpty(g.Key) ? "EMPTY" : g.Key, 
                    Count = g.Count(),
                    Percentage = (double)(g.Sum(l => l.Minutes) / totalDuration.TotalMinutes),
                    Duration = TimeSpan.FromMinutes(g.Sum(l => l.Minutes))
                });

            Console.WriteLine("Summary:");
            groups.ToList().ForEach(t => Console.WriteLine("{0,10} {2,3}: {1:g} {3,7:p1}",
                t.Project,
                t.Duration,
                t.Count,
                t.Percentage));

            Console.WriteLine("{0,10} {2,3}: {1:g}",
                "Total",
                totalDuration,
                totalCount);

            Console.ForegroundColor = ConsoleColor.Red;

            foreach(var error in logs.Errors)
            {
                Console.WriteLine(error);
            }

            Console.ResetColor();
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: wl [OPTIONS] -l=<path to work log>");
            Console.WriteLine("Parses a work log file and posts worklogs to OnTime for the tasks contained within.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
