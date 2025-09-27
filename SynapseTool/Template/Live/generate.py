import os
import re
import yaml
import json
import argparse

import vscode_generator
import visual_studio_generator

def load_template(file_name):
    '''
    @Desc: load the target template file and return data with type of dict
    @Param: 
        file_name: str, absolute file name of template
    @Return:
        data: dict
    '''
    data = {}
    with open(file_name, 'r') as f:
        rd = yaml.safe_load(f)
    if not rd:
        return data

    snippet = rd['snippet'][0]
    data['name'] = snippet['name']
    data['prefix'] = snippet['prefix']
    data['desc'] = snippet['desc']
    variables = snippet.get('variables', '').split('\n')
    data['variables'] = []
    for variable in variables:
        if variable:
            data['variables'].append(variable)
    data['body'] = snippet['body']
    return data

if __name__ == '__main__':

    parse = argparse.ArgumentParser()
    parse.add_argument('--export', nargs='+', default=[], help='Export specific templates')
    parse.add_argument('--export-all', action='store_true', default=False, help='Export all templates')
    args = parse.parse_args()

    if args.export_all or args.export:
        script_path = os.path.abspath(__file__)
        script_dir = os.path.dirname(script_path)
        if args.export_all:
            vs_snippet = {}
            for file_name in os.listdir(script_dir):
                if file_name.endswith('.yaml'):
                    abs_file_name = os.path.join(script_dir, file_name)
                    template_data = load_template(abs_file_name)
                    vs_snippet_name, vs_snippet_data = vscode_generator.generate_vs_code_snippet(template_data)
                    vs_snippet[vs_snippet_name] = vs_snippet_data
                    visual_studio_generator.generate_visual_studio_snippet(script_dir, template_data)
            vscode_generator.dump_snippet(script_dir, vs_snippet)
        else:
            vs_snippet = vscode_generator.load_snippet(script_dir)
            for template_name in args.export:
                abs_file_name = os.path.join(script_dir, '%s.yaml'%template_name)
                if os.path.exists(abs_file_name):
                    template_data = load_template(abs_file_name)
                    vs_snippet_name, vs_snippet_data = vscode_generator.generate_vs_code_snippet(template_data)
                    vs_snippet[vs_snippet_name] = vs_snippet_data
                    visual_studio_generator.generate_visual_studio_snippet(script_dir, template_data)
            vscode_generator.dump_snippet(script_dir, vs_snippet)
