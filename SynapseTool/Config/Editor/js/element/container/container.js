
class Container extends Element {
    constructor(domElement) {
        super(domElement);
    }

    async setChildren(configs, onSetChildren=null) {
        this.domElement.innerHTML = '';
        configs.forEach(config => {
            const loadElement = elementLoader.createLoadElement(config);
            this.domElement.appendChild(loadElement);
        });
        await elementLoader.loadElement(this.domElement);
        if (onSetChildren) {
            onSetChildren();
        }
    }
}