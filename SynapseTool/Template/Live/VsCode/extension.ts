// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import * as vscode from 'vscode';
import * as path from 'path';
import * as fs from 'fs';

const outputChannel = vscode.window.createOutputChannel('SynapseSnippet');

// This method is called when your extension is activated
// Your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext) {

	// Use the console to output diagnostic information (console.log) and errors (console.error)
	// This line of code will only be executed once when your extension is activated
	console.log('Congratulations, your extension "SynapseSnippet" is now activing!');

	// get snippet path
	const config = vscode.workspace.getConfiguration('SynapseSnippet');
	let snippetsPath = config.get<string>('SnippetPath');
	console.log("snippetsPath: ", snippetsPath);
	outputChannel.appendLine("snippetsPath: ${snippetsPath}");
	console.log("context path: ", context.extensionPath);
	console.log("dir path: ", __dirname);
	console.log("process pwd path: ", process.cwd());
	const workspaceFolders = vscode.workspace.workspaceFolders;
	if (workspaceFolders) {
		console.log("workspace path: ", workspaceFolders[0].uri.fsPath);
	}
	if (snippetsPath) {
		const workspaceFolders = vscode.workspace.workspaceFolders;
		if (workspaceFolders) {
			snippetsPath = path.join(workspaceFolders[0].uri.fsPath, snippetsPath);
		}
		else {
			snippetsPath = path.join(context.extensionPath, snippetsPath);
		}
		vscode.window.showErrorMessage(`path: ${snippetsPath}`);
		if (fs.existsSync(snippetsPath)) {
			// load snippets
			const snippetsData = fs.readFileSync(snippetsPath, 'utf-8');
			const snippets = JSON.parse(snippetsData);
			console.log("snippet ", snippets);
			Object.keys(snippets).forEach((key) => {
				const snippet = snippets[key];
				console.log("-----");
				console.log("key: ", key);
				console.log("prefix: ", snippet.prefix);
				console.log("snippet: ", snippet.body.join('\n'));
			});
			const provider = vscode.languages.registerCompletionItemProvider('csharp', {
				provideCompletionItems(document: vscode.TextDocument, position: vscode.Position) {
					const completions: vscode.CompletionItem[] = [];
					Object.keys(snippets).forEach((key) => {
						const snippet = snippets[key];
						const completionItem = new vscode.CompletionItem(snippet.prefix, vscode.CompletionItemKind.Snippet);
						completionItem.insertText = new vscode.SnippetString(snippet.body.join('\n'));
						completions.push(completionItem);
					});
					return completions;
				}
			});
			context.subscriptions.push(provider);
			console.log('Congratulations, your extension "SynapseSnippet" is now actived!');
		}
		else {
			console.error('[SynapseSnippet] Snippet not found: ', snippetsPath);
		}
	}
	else {
		console.error('[SynapseSnippet] Snippet not found: ', snippetsPath);
	}
}

// This method is called when your extension is deactivated
export function deactivate() {}
