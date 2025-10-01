import os

XML = '''<?xml version="1.0" encoding="utf-8"?>
<CodeSnippets xmlns="http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet">
	<CodeSnippet Format="1.0.0">
		<Header>
			<Title>%s</Title>
			<Shortcut>%s</Shortcut>
			<Description>%s</Description>
			<Author>Synapse</Author>
			<SnippetTypes>
				<SnippetType>Expansion</SnippetType>
			</SnippetTypes>
		</Header>
		<Snippet>%s
			<Code Language="csharp"><![CDATA[%s]]>
			</Code>
		</Snippet>
	</CodeSnippet>
</CodeSnippets>
'''

DECLARATION_XML = '''
			<Declarations>
				<Literal>
					<ID>%s</ID>
					<Default>%s</Default>
				</Literal>
			</Declarations>
'''

def generate_visual_studio_snippet(script_dir, data):
    '''
    @Desc: generate visual studio snippet xml from template data
    @Param:
        script_dir: str, absolute directory
        data: dict, output of @Func:load_template
    '''
    name = data['name']
    prefix = data['prefix']
    desc = data['desc']
    body = data['body']
    declaration = ''
    for variable in data['variables']:
        declaration += DECLARATION_XML % (variable, variable)
    xml = XML % (
        name,
        prefix,
        desc,
        declaration,
        body
    )

    snippet_path = os.path.join(script_dir, 'VisualStudio', '%s.snippet'%name)
    with open(snippet_path, 'w') as f:
        f.write(xml)
