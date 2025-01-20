using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wl.Jira
{
    public class Issue
    {
        public string IssueKey { get; set; }
        public long IssueId { get; set; }
        public string Summary { get; set; }
    }
}
