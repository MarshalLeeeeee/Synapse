
class Container extends Element {
    constructor(domElement) {
        super(domElement);
        this.children = {}; // dict: id -> element
    }

    /* load children of html elements
     * @Parameter: 
     * - configs: dict
     * - childrenGenerator: function, return dict { id -> js element instance } 
     */
    async setChildren(configs, childrenGenerator=null) {
        this.domElement.innerHTML = '';
        this.children = {};
        configs.forEach(config => {
            const loadElement = elementLoader.createLoadElement(config);
            this.domElement.appendChild(loadElement);
        });
        await elementLoader.loadElement(this.domElement);
        if (childrenGenerator) {
            this.children = childrenGenerator();
        }
    }

    /* return element values as dict */
    getElementValues() {
        const res = {};
        Object.entries(this.children).forEach(([elementId, element]) => {
            res[elementId] = element.getValue() ?? null;
        });
        return res;
    }
}