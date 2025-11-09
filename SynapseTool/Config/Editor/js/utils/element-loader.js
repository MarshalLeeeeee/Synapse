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
            const elementLoaderElement = loadElement.querySelector('data-loader');
            if (!elementLoaderElement) {
                continue;
            }
            loadElement.replaceWith(await this._callMethod(
                elementLoaderElement.textContent,
                loadElement
            ));
            const cssPathElements = loadElement.querySelectorAll('data-css-path');
            for (const cssPathElement of cssPathElements) {
                await cssLoader.loadCssFile(cssPathElement.textContent);
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
        const elementClassElement = loadElement.querySelector('data-class');
        if (elementClassElement) {
            div.classList.add(elementClassElement.textContent);
        }
        const elementPathElement = loadElement.querySelector('data-element-path');
        if (elementPathElement) {
            const html = await this._loadElementContent(elementPathElement.textContent);
            div.innerHTML = html;
        }
        await this._loadElement(div);
        return div;
    }

    async addButton(loadElement) {
        const button = document.createElement('button');
        button.id = loadElement.id;
        const elementClassElement = loadElement.querySelector('data-class');
        if (elementClassElement) {
            button.classList.add(elementClassElement.textContent);
        }
        const elementPathElement = loadElement.querySelector('data-element-path');
        if (elementPathElement) {
            const html = await this._loadElementContent(elementPathElement.textContent);
            button.innerHTML = html;
        }
        const elementTextElement = loadElement.querySelector('data-text');
        if (elementTextElement) {
            const textNode = document.createTextNode(elementTextElement.textContent);
            button.appendChild(textNode);
        }
        await this._loadElement(button);
        return button;
    }
}

const elementLoader = new ElementLoader();
