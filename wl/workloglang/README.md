# WorkLogLang 

WorkLogLang creates a new language for worklogs complete with syntax highlighting, intellisense, and hovers.

## Preview and Commit

`.wl` files show **Preview** and **Commit** buttons at the top of the file (also available in the Command Palette as *Worklog: Preview Worklog* / *Worklog: Commit Worklog to Tempo*).

- **Preview** saves the file and runs `wl.exe -l=<file> -i -c` (calculate only).
- **Commit** asks for confirmation, saves the file, and runs `wl.exe -l=<file> -i` (posts to Tempo).

Output from each run is appended to the **Worklog** channel in the Output panel.

Set `workloglang.wlPath` to the full path of `wl.exe`.

### Environment variables

`wl.exe` reads its credentials from these environment variables. After setting them (e.g. with `setx`), fully restart VS Code so the extension sees them. The buttons check for them before running.

| Variable | Needed for | Value |
| --- | --- | --- |
| `JiraUsername` | Preview, Commit | Your Atlassian account email. |
| `JiraAccessToken` | Preview, Commit | An Atlassian API token: https://id.atlassian.com/manage-profile/security/api-tokens |
| `TempoToken` | Commit | A Tempo API token: https://rollick.atlassian.net/plugins/servlet/ac/io.tempo.jira/tempo-app#!/configuration/api-integration |
| `JiraAccountId` | Commit | The id at the end of your Atlassian profile page URL. |



## Generating visx files

Ubuntu terminal
Change directory to the workloglang folder
`vsce package <version>`
 
example: `vsce package 1.1.0`