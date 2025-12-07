
class ModalElementContainer extends Element {
    constructor(domElement) {
        super(domElement);
        this.elements = {}
    }

    async setElements(element_configs) {
        this.domElement.innerHTML = '';
        element_configs.forEach(config => {
            const loadElement = document.createElement('load-element');
            loadElement.id = config['id'];

            const dataLoader = config['data-loader'];
            if (dataLoader) {
                const dataLoaderElement = document.createElement('data-loader');
                dataLoaderElement.innerText = dataLoader;
                loadElement.appendChild(dataLoaderElement);
            }

            const dataClass = config['data-class'];
            if (dataClass) {
                const dataClassElement = document.createElement('data-class');
                dataClassElement.innerText = dataClass;
                loadElement.appendChild(dataClassElement);
            }

            const dataElementPath = config['data-element-path'];
            if (dataElementPath) {
                const dataElementPathElement = document.createElement('data-element-path');
                dataElementPathElement.innerText = dataElementPath;
                loadElement.appendChild(dataElementPathElement);
            }

            config['data-css-path']?.forEach(cssPath => {
                const dataCssPath = document.createElement('data-css-path');
                dataCssPath.innerText = cssPath;
                loadElement.appendChild(dataCssPath);
            });

            this.domElement.appendChild(loadElement);
        });
        await elementLoader.loadElement(this.domElement);
    }
}