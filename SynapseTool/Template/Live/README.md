# Live template
Live template includes code snippet for Synpase.

## IDE
We currently support 
- VsCode: install VSIX in [Vscode](./VsCode/) in ```VsCode / Extension / Install from VSIX...```. If the workspace of VsCode is the root dir of this repo, no more setups are required. Otherwise, define the relative path of your VsCode workspace in the setting of the plugin.
- Visual Studio: import [VisualStudio](./VisualStudio/) in ```VisualStudio / Tools / Code snippet management``` (remeber to choose C# in the language option).

## Usage
Code snippets should be defined as a seperate yaml in this directory. Run ```python generate.py``` (run ```pip install -r requirements.txt``` if there exists missing module) to generate snippet for different IDEs.

To export one or some templates, run ```python generate.py --export xxx yyy``` (there should be ```xxx.yaml``` and ```yyy.yaml```).

To export all templates, run ```python generate.py --export-all```.
