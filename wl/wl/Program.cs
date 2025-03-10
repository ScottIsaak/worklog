﻿using Mono.Options;
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
            string tempoToken = ConfigurationManager.AppSettings["TempoToken"];
            string jiraUsername = ConfigurationManager.AppSettings["JiraUsername"];
            string jiraAccountId = ConfigurationManager.AppSettings["JiraAccountId"];
            string jiraAccessToken = ConfigurationManager.AppSettings["JiraAccessToken"];
            bool addIssueNames = false;

            Console.OutputEncoding = Encoding.UTF8;

            var p = new OptionSet()
            {
                { "l|log=", "The path to a log file to parse. Multiple -l options can be specified on the command line.",
                    l => logFilePaths.Add(l) },

                { "c|calculate", "Calculate hours only. Do not post work logs to Tempo.",
                    c => calculateOnly = (c != null)},

                { "t|tempoToken=", "Set the token value for the api call.",
                    t => tempoToken = t },

                { "u|jiraUsername=", "Set the username for the api call.",
                    u => jiraUsername = u },

                { "d|jiraAccountId=", "Set the account id for the api call.",
                    d => jiraAccountId = d},

                { "i|addIssueNames", "Add the issue name to the description.",
                    i => addIssueNames = (i != null) },

                { "j|jiraAccessToken=", "Set the access token for the api call.",
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

            var service = new Tempo.Client(jiraAccountId, jiraUsername, tempoToken, jiraAccessToken);

            foreach (var logFilePath in logFilePaths)
            {
                if (File.Exists(logFilePath))
                {
                    var logs = GetWorklogs(logFilePath);
                    PostWorklogs(service, logs, calculateOnly, addIssueNames);
                }
            }
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
            Console.WriteLine("Worklogs:");
            foreach (var log in logs)
            {
                if (addIssueNames)
                {
                    service.AddIssueDetails(log);
                }

                var logText = log.ToString();
                var width = 80;
                try
                {
                    width = Console.BufferWidth - 1;
                }
                catch { }
                var status = Status.Skipped;

                if (logText.Length > width) logText = string.Concat(logText.Substring(0, width - 3), "...");

                Console.Write(logText);

                if (!calculateOnly)
                {
                    status = service.CreateWorkLog(log) ? Status.Posted : Status.Failed;
                }

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
        }

        static void WriteStatus(Status status)
        {
            Console.CursorLeft = 0;
            switch (status)
            {
                case Status.Failed: Console.Write('X'); break;
                case Status.Posted: Console.Write('√'); break;
                case Status.Skipped: Console.Write('-'); break;
            }
            Console.CursorLeft = Console.BufferWidth - 1;
            Console.WriteLine();
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
