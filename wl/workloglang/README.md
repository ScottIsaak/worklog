# WorkLogLang 

WorkLogLang creates a new language for worklogs complete with syntax highlighting, intellisense, and hovers.

## Preview and Commit

`.wl` files show **Preview** and **Commit** buttons at the top of the file (also available in the Command Palette as *Worklog: Preview Worklog* / *Worklog: Commit Worklog to Tempo*).

- **Preview** saves the file and runs `wl.exe -l=<file> -i -c` (calculate only).
- **Commit** asks for confirmation, saves the file, and runs `wl.exe -l=<file> -i` (posts to Tempo).

Set `workloglang.wlPath` to the full path of `wl.exe`. The Tempo/Jira environment variables (`TempoToken`, `JiraUsername`, `JiraAccountId`, `JiraAccessToken`) must be available to VS Code.



## Generating visx files

Ubuntu terminal
Change directory to the workloglang folder
`vsce package <version>`
 
example: `vsce package 1.1.0`