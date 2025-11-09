/* element loader
 * build element tree externally
 */
class ElementLoader {
    constructor() {
        this._methodMap = this._createMethodMap();
    }
    
    _createMethodMap() {
        const methodMap = {};
        const proto = Object.getPrototypeOf(this);
        const propertyNames = Object.getOwnPropertyNames(proto);
        for (const name of propertyNames) {
            if (name !== 'constructor' && 
                !name.startsWith('_') && 
                typeof this[name] === 'function') {
                methodMap[name] = this[name].bind(this);
            }
        }
        return methodMap;
    }
    
    async _callMethod(methodName, ...args) {
        if (this._methodMap[methodName]) {
            const obj = await this._methodMap[methodName](...args);
            return obj;
        }
        throw new Error(`Method ${methodName} not found`);
    }

    async loadElementTree() {
        await this._loadElement(document.body);
    }

    async _loadElement(element) {
        const loadElements = element.querySelectorAll('load-element');
        for (const loadElement of loadElements) {
            const elementLoader = loadElement.getAttribute('data-loader');
            const cssPathesAttr = loadElement.getAttribute('data-css-pathes');
            const elementCssPathes = cssPathesAttr ? cssPathesAttr.split(',') : [];
            loadElement.replaceWith(await this._callMethod(
                elementLoader,
                loadElement
            ));
            for (const cssPath of elementCssPathes) {
                await cssLoader.loadCssFile(cssPath);
            }
        }
    }

    async _loadElementContent(elementPath) {
        const response = await fetch(elementPath);
        if (!response.ok) {
            throw new Error(`Failed to load element from path "${elementPath}".`);
        }

        const html = await response.text();
        return html;
    }

    async addDiv(loadElement) {
        const div = document.createElement('div');
        div.id = loadElement.id;
        const className = loadElement.getAttribute('data-class');
        if (className) {
            div.classList.add(className);
        }
        const elementPath = loadElement.getAttribute('data-element-path');
        if (elementPath) {
            const html = await this._loadElementContent(elementPath);
            div.innerHTML = html;
        }
        await this._loadElement(div);
        return div;
    }

    async addButton(loadElement) {
        const button = document.createElement('button');
        button.id = loadElement.id;
        const className = loadElement.getAttribute('data-class');
        if (className) {
            button.classList.add(className);
        }
        const elementPath = loadElement.getAttribute('data-element-path');
        if (elementPath) {
            const html = await this._loadElementContent(elementPath);
            button.innerHTML = html;
        }
        const text = loadElement.getAttribute('data-text');
        if (text) {
            const textNode = document.createTextNode(text);
            button.appendChild(textNode);
        }
        await this._loadElement(button);
        return button;
    }
}

const elementLoader = new ElementLoader();
