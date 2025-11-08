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
        'elementPath': 'element/container.html',
        'children': {
            'toolbar': '',
            'tableContainer': '',
            'footer': ''
        }
    },
    'contextMenu': {
        'loader': 'loadDiv',
        'class': 'context-menu',
        'elementPath': 'element/context-menu.html'
    },
    'addAttributeModal': {
        'loader': 'loadDiv',
        'class': 'modal',
        'elementPath': 'element/add-attribute-modal.html'
    },
    'addVersionModal': {
        'loader': 'loadDiv',
        'class': 'modal',
        'elementPath': 'element/add-version-modal.html'
    },
    'toolbar': {
        'loader': 'loadDiv',
        'class': 'toolbar',
        'elementPath': 'element/container/toolbar.html',
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
        'elementPath': 'element/container/table-container.html'
    },
    'footer': {
        'loader': 'loadDiv',
        'class': 'footer',
        'elementPath': 'element/container/footer.html'
    },
    'addAttributeBtn': {
        'loader': 'loadButton',
        'elementPath': 'element/btn/add-btn.html',
        'text': 'Add Attribute'
    },
    'addRowBtn': {
        'loader': 'loadButton',
        'elementPath': 'element/btn/add-btn.html',
        'text': 'Add row'
    },
    'insertRowBtn': {
        'loader': 'loadButton',
        'elementPath': 'element/btn/add-btn.html',
        'text': 'Insert row'
    },
    'addVersionBtn': {
        'loader': 'loadButton',
        'class': 'secondary',
        'elementPath': 'element/btn/add-btn.html',
        'text': 'Add version'
    }
}
