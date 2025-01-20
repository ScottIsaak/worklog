using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using wl.Jira;

namespace wl.Tempo
{
    public class Client
    {
        private string jiraAccountId;
        private string username;
        private string tempoClientSecret;
        private string jiraAccessToken;
        private const string worklogUri = "https://api.tempo.io/4/worklogs";
        private const string issueUriFormat = "https://rollick.atlassian.net/rest/api/3/issue/{0}?fields=summary";

        private Dictionary<string, Issue> _issueCache = new Dictionary<string, Issue>();

        public Client(string jiraAccountId, string username, string tempoClientSecret, string jiraAccessToken)
        {
            this.jiraAccountId = jiraAccountId;
            this.username = username;
            this.tempoClientSecret = tempoClientSecret;
            this.jiraAccessToken = jiraAccessToken;
        }

        public bool CreateWorkLog(wl.WorkLog wl)
        {
            if (wl.TaskId == 0) return true;

            var workLogBean = new WorkLog
            {
                IssueId = GetIssue(wl.IssueKey).IssueId,
                TimeSpent = new TimeSpan(0, (int)wl.Minutes, 0),
                Start = wl.Begin,
                Description = wl.Message,
                AuthorAccountId = jiraAccountId
            };

            return PostWorklog(workLogBean);
        }

        public void AddIssueDetails(wl.WorkLog wl)
        {
            if (wl.TaskId == 0) return;

            var issue = GetIssue(wl.IssueKey);

            if (issue != null)
                wl.Message = $"[{wl.IssueKey}] {issue.Summary.Trim()} - {wl.Message}";
        }

        private Issue GetIssue(string issueKey)
        {
            if (!_issueCache.ContainsKey(issueKey))
            {
                var uri = string.Format(issueUriFormat, issueKey);

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

                //Start the request using the necessary url, credentials, and content
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.ContentLength = 0;
                var svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(this.username + ":" + this.jiraAccessToken));
                request.Headers.Add("Authorization", string.Format("Basic {0}", svcCredentials));


                var response = (HttpWebResponse)request.GetResponse();

                if ((response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created) && response.ContentType.Contains("application/json"))
                {
                    var memoryStreamResponse = new MemoryStream();
                    response.GetResponseStream().CopyTo(memoryStreamResponse);
                    memoryStreamResponse.Position = 0;

                    var result = new StreamReader(memoryStreamResponse).ReadToEnd();

                    memoryStreamResponse.Position = 0;

                    var responseObject = JObject.Parse(result);
                    var issueName = ((string)responseObject["fields"]["summary"]);

                    var issue = new Issue
                    {
                        IssueKey = issueKey,
                        Summary = ((string)responseObject["fields"]["summary"]),
                        IssueId = ((long)responseObject["id"])
                    };

                    _issueCache[issueKey] = issue;
                }
                else
                {
                    _issueCache[issueKey] = null;
                }
            }

            return _issueCache[issueKey];
        }



        private bool PostWorklog(WorkLog workLogBean)
        {
            var javascriptSerializer = new DataContractJsonSerializer(typeof(WorkLog), new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true,
                EmitTypeInformation = System.Runtime.Serialization.EmitTypeInformation.Never,
                KnownTypes = new[] { typeof(WorkLog) },
                DateTimeFormat = new System.Runtime.Serialization.DateTimeFormat("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff")
            });

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

            //Start the request using the necessary url, credentials, and content
            var request = (HttpWebRequest)WebRequest.Create(worklogUri);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = 0;
            request.Headers.Add("Authorization", string.Format("Bearer {0}", this.tempoClientSecret));

            var memoryStream = new MemoryStream();
            javascriptSerializer.WriteObject(memoryStream, workLogBean);
            memoryStream.Position = 0;
            StreamReader sr = new StreamReader(memoryStream);
            string serializedRequestContent = sr.ReadToEnd();
            byte[] contentArray = System.Text.Encoding.UTF8.GetBytes(serializedRequestContent);
            request.ContentLength = contentArray.Length;
            var requestStream = request.GetRequestStream();
            requestStream.Write(contentArray, 0, contentArray.Length);
            requestStream.Close();

            var response = (HttpWebResponse)request.GetResponse();

            if ((response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created) && response.ContentType.Contains("application/json"))
            {
                var memoryStreamResponse = new MemoryStream();
                response.GetResponseStream().CopyTo(memoryStreamResponse);
                memoryStreamResponse.Position = 0;

                var result = new StreamReader(memoryStreamResponse).ReadToEnd();

                memoryStreamResponse.Position = 0;

                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
