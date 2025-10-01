# File template
File templates enable creation of single or multiple files initialized with common templates.

## Usage
To generate files using template, run ```python SynapseTool\Template\File\generate.py --template [template_name] --variables [...]```. Templatesa are yaml files in ```SynapseTool\Template\File\raw\```. Variables are used to replace file name or file content with args provided by user (by replacing ```$s0$, $s1$, ...``` in templates).

To generate new file template, run ```SynapseTool\Template\File\generate.py --template file_template --variables [name]``` (YES! Generate file template itself can be done by using file template).
