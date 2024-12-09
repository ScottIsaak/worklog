// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
const { match } = require('assert');
const vscode = require('vscode');
const http = require('http');

// this method is called when your extension is activated
// your extension is activated the very first time the command is executed

/**
 * @param {vscode.ExtensionContext} context
 */
function activate(context) {

	// Use the console to output diagnostic information (console.log) and errors (console.error)
	// This line of code will only be executed once when your extension is activated
	console.log('Congratulations, your extension "workloglang" is now active!');
	
	/*
="new vscode.CompletionItem({ ""label"": '" & A2 & "', ""description"": '" & B2 & "' }, vscode.CompletionItemKind.Text),"
	*/

	var completionItemProvider = vscode.languages.registerCompletionItemProvider('worklog', {
		provideCompletionItems(document, position, token, context) {
			return [
				new vscode.CompletionItem({ "label": 'YAM-1', "description": '5000-Yamaha Marine CSI Ongoing - Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'WIN-7', "description": 'Aimbase Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TXT-120', "description": 'Textron Inventory Ongoing' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TXT-8', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TOY-32', "description": 'Discussions / Meetings / Misc - Aimbase' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-22', "description": 'Scrum' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-21', "description": 'Help Desk Support' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-20', "description": 'Enterprise Client Stand-up' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-34', "description": 'Responding to Email' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-18', "description": 'Sales/Prospect Exp.' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-17', "description": 'Team Management / Supervising' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-16', "description": '1:1' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-15', "description": 'Developer Machine Setup / Configuration' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-14', "description": 'Continuing education' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-10', "description": 'Recruiting' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-9', "description": 'Product Design and R+D' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-30', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-31', "description": 'Daily Stand-Up / Scrum' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-32', "description": 'Time Entry' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-33', "description": 'Agile Planning + Grooming' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-4', "description": 'Employee Training' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-29', "description": 'Internal Service' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-2', "description": 'Holiday' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-1', "description": 'PTO' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIAR-7', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'THOR-77', "description": 'Discussions / Meetings / Misc - Aimbase' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'SMCR-3', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'SC-4', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'PUR-6', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'PBH-11', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'NUC-1', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'NMMA-404', "description": 'Meetings / Discussions / Misc - DBLS' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'NMMA-139', "description": 'Meetings / Discussions / Misc - Aimbase Malibu' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'NMMA-26', "description": 'Meetings / Discussions / Misc - Aimbase Sea Ray' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'MMX-2', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'MERC-1', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'MBG-1', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'MAN-1', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'KRV-28', "description": 'Discussions / Meetings / Misc - Aimbase' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'INV-1', "description": 'Invincible Boats - Meetings/Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'HMY-2', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'HMY-1', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'GW-408', "description": 'Meetings / Discussions - Aimbase' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'GM-2', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'GDRV-2', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'GB-4', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'EVER-11', "description": 'Meetings / Discussions - Aimbase' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'EDGE-2', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'DUTCH-1', "description": 'Meetings / Discussions / Misc' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'COR-1', "description": 'Meetings / Discussions' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'CC-323', "description": 'Meetings / Discussions / Misc' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'AVALA-1', "description": 'AVALA Release Management' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'AIM-2068', "description": 'Documentation' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'AIM-96', "description": 'Code Reviews' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'AIM-95', "description": 'Meetings / Discussions / Misc' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'AIM-55', "description": 'General Aimbase QA Work' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'AIM-6', "description": 'Setup Jira' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-3', "description": 'TIME-3 (Internal Service) is deprecated, use TIME-29 instead' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-8', "description": 'TIME-8 (Meetings / Discussions) is deprecated, use TIME-30 instead' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-7', "description": 'TIME-7 (Daily Stand-Up/Scrum) is deprecated, use TIME-31 instead' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-6', "description": 'TIME-6 (Time Entry) is deprecated, use TIME-32 instead' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-5', "description": 'TIME-5 (Agile Planning + Grooming) is deprecated, use TIME-33 instead' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-19', "description": 'TIME-19 (Responding to Email) is deprecated, use TIME-34 instead' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'TIME-27', "description": 'SOC Compliance' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'NB-1', "description": 'Discussions / Meetings / Misc - Aimbase - Nimbus' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'MAXX-74', "description": 'Discussions / Meetings / Misc - Aimbase - MAXX-D' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'BSPA-1', "description": 'Discussions / Meetings / Misc - Aimbase - Bullfrog Spas' }, vscode.CompletionItemKind.Text),
				new vscode.CompletionItem({ "label": 'NMMA-1811', "description": 'Discussions / Meetings / Misc - Aimbase - Balise Pontoon' }, vscode.CompletionItemKind.Text)
			];
		}
	}, '[');
	
	var hoverProvider = vscode.languages.registerHoverProvider('worklog', {
		provideHover(document, position, token) {
			var line = document.lineAt(position).text;
			var matches = /\[(\w+\-\d+)\]/g.exec(line);
			if(matches.length > 1) {
				return {
					contents: [ 'https://rollick.atlassian.net/browse/' + matches[1] ]
				}
			}
			else {
				return {
					contents: []
				}
			}
		}
	});
	

	context.subscriptions.push(completionItemProvider);
	context.subscriptions.push(hoverProvider);
}


// this method is called when your extension is deactivated
function deactivate() {}

module.exports = {
	activate,
	deactivate
}
