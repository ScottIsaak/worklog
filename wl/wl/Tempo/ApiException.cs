using System;
using System.IO;
using System.Net;

namespace wl.Tempo
{
    public class ApiException : Exception
    {
        public bool IsAuthError { get; }

        public ApiException(string message, bool isAuthError = false, Exception innerException = null)
            : base(message, innerException)
        {
            IsAuthError = isAuthError;
        }

        public static ApiException FromWebException(WebException ex, string service, string credentialHint, string action, bool notFoundMayBeAuth = false)
        {
            var response = ex.Response as HttpWebResponse;
            if (response == null)
                return new ApiException($"Could not reach {service} while {action}: {ex.Message}", false, ex);

            var code = (int)response.StatusCode;
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                return new ApiException($"{service} rejected the credentials ({code} {response.StatusDescription}) while {action}. Check {credentialHint}.", true, ex);

            string body = null;
            try
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                    body = reader.ReadToEnd();
                if (body.Length > 300) body = body.Substring(0, 300) + "...";
            }
            catch { }

            // Jira answers requests with invalid credentials as anonymous, which surfaces as 404.
            if (notFoundMayBeAuth && response.StatusCode == HttpStatusCode.NotFound)
                body = $"{body} If the issue exists, check {credentialHint}.".Trim();

            return new ApiException($"{service} returned {code} {response.StatusDescription} while {action}. {body}".TrimEnd(), false, ex);
        }
    }
}
