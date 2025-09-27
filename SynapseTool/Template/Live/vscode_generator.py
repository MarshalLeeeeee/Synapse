import os
import json

def generate_vs_code_snippet(data):
    '''
    @Desc: generate vs code snippet from template data
    @Param:
        data: dict, output of @Func:load_template
    @Return:
        snippet_data kvp: (str, dict)
        name: {
            'prefix': str,
            'body': List[str],
        }
    '''
    name = data['name']
    prefix = data['prefix']
    body = data['body']
    for vi, variable in enumerate(data['variables']):
        s = '$%s$'%variable
        t = '${%d:%s}' % (vi+1, variable)
        body = body.replace(s, t)
    body = body.split('\n')
    snippet_data = {
        'prefix': prefix,
        'body': body,
    }
    return name, snippet_data

def load_snippet(script_dir):
    '''
    @Desc: load snippet json as dict
    @Param:
        script_dir: str, absolute directory
    @Return:
        vs_snippet: dict 
    '''
    vs_snippet_path = os.path.join(script_dir, 'VsCode', 'snippets.json')
    with open(vs_snippet_path, 'r') as f:
        vs_snippet = json.load(f)
    vs_snippet = vs_snippet or {}
    return vs_snippet

def dump_snippet(script_dir, vs_snippet):
    '''
    @Desc: dump dict to json file
    @Param:
        script_dir: str, absolute directory
        vs_snippet: dict, includes all code snippets
    '''
    vs_snippet_path = os.path.join(script_dir, 'VsCode', 'snippets.json')
    with open(vs_snippet_path, 'w') as f:
        json.dump(vs_snippet, f, indent=4, ensure_ascii=False, sort_keys=True)
