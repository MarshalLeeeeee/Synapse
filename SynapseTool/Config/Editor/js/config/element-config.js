/* structure of html elements 
 * 
 */
const ElementConfig = {
    '_': {
        'chilrent': {
            'container': '',
            'contextMenu': '',
            'addAttributeModal': '',
            'addVersionModal': ''
        }
    },
    'container': {
        'loader': 'loadDiv',
        'class': 'container',
        'elementPath': 'element/exclusive/container.html',
        'cssPathes': ['css/exclusive/container.css', 'css/header/h1_p_1.css'],
        'children': {
            'toolbar': '',
            'tableContainer': '',
            'footer': ''
        }
    },
    'contextMenu': {
        'loader': 'loadDiv',
        'class': 'context-menu',
        'elementPath': 'element/exclusive/context-menu.html'
    },
    'addAttributeModal': {
        'loader': 'loadDiv',
        'class': 'modal',
        'elementPath': 'element/exclusive/add-attribute-modal.html'
    },
    'addVersionModal': {
        'loader': 'loadDiv',
        'class': 'modal',
        'elementPath': 'element/exclusive/add-version-modal.html'
    },
    'toolbar': {
        'loader': 'loadDiv',
        'class': 'toolbar',
        'elementPath': 'element/exclusive/container/toolbar.html',
        'children': {
            'addAttributeBtn': '#toolbarLeft',
            'addRowBtn': '#toolbarLeft',
            'insertRowBtn': '#toolbarLeft',
            'addVersionBtn': '#toolbarRight'
        }
    },
    'tableContainer': {
        'loader': 'loadDiv',
        'class': 'table-container',
        'elementPath': 'element/exclusive/container/table-container.html'
    },
    'footer': {
        'loader': 'loadDiv',
        'class': 'footer',
        'elementPath': 'element/exclusive/container/footer.html'
    },
    'addAttributeBtn': {
        'loader': 'loadButton',
        'class': 'style1',
        'elementPath': 'element/btn/add-btn.html',
        'cssPathes': ['css/btn/style1.css'],
        'text': 'Add Attribute'
    },
    'addRowBtn': {
        'loader': 'loadButton',
        'class': 'style1',
        'elementPath': 'element/btn/add-btn.html',
        'cssPathes': ['css/btn/style1.css'],
        'text': 'Add row'
    },
    'insertRowBtn': {
        'loader': 'loadButton',
        'class': 'style1',
        'elementPath': 'element/btn/add-btn.html',
        'cssPathes': ['css/btn/style1.css'],
        'text': 'Insert row'
    },
    'addVersionBtn': {
        'loader': 'loadButton',
        'class': 'style2',
        'elementPath': 'element/btn/add-btn.html',
        'cssPathes': ['css/btn/style2.css'],
        'text': 'Add version'
    }
}
