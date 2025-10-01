import os
import yaml
import argparse

def load_template(file_name, variables):
    '''
    @Desc: load the target template file and return data with type of dict
        Replace placeholder with variables from user args
    @Param: 
        file_name: str, absolute file name of template
        variables: List[str], list of string variables used in file template
    @Return:
        data: List[(str, str)], list of tuple of file_name and file_content
    '''
    with open(file_name, 'r') as f:
        rd = yaml.safe_load(f)
    if not rd:
        return []
    
    variable_cnt = rd['variable_cnt']
    if len(variables) < variable_cnt:
        return []
    
    data = []
    for file_conf in rd.get('files', ()):
        name = file_conf['name']
        content = file_conf['content']
        for i in range(variable_cnt):
            name = name.replace("$s%d$"%i, variables[i])
            content = content.replace("$s%d$"%i, variables[i])
        data.append((name, content))
    return data

def generate(args):
    '''
    @Desc: generate certain files with file template
    @Params:
        args: Namespace, parsed command arg
    '''
    script_path = os.path.abspath(__file__)
    script_dir = os.path.dirname(script_path)
    repo_dir = os.path.join(script_dir, '../../../')
    template_dir = os.path.join(script_dir, 'raw')
    template_path = os.path.join(template_dir, '%s.yaml'%args.template)

    data = load_template(template_path, args.variables)
    if not data:
        print('>>>> file template is not valid')
        return

    violate_file_names = []
    for file_name, _ in data:
        abs_file_name = os.path.join(repo_dir, file_name)
        if os.path.exists(abs_file_name):
            violate_file_names.append(abs_file_name)
    if violate_file_names:
        print('>>> not all files are new:')
        for fn in violate_file_names:
            print('>>>>> File name: (%s)' % fn)
        return

    for file_name, file_content in data:
        with open(file_name, 'w') as f:
            f.write(file_content)

if __name__ == '__main__':
    parse = argparse.ArgumentParser()
    parse.add_argument('--template', required=True, default='', help='name of file template')
    parse.add_argument('--variables', nargs='+', default=[], help='string variables used in file templates')
    args = parse.parse_args()
    generate(args)
