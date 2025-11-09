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

    async loadElementTree(elementConfigMap) {
        if ('_' in elementConfigMap && 'chilrent' in elementConfigMap['_']) {
            const children = elementConfigMap['_']['chilrent'];
            for (const childId in children) {
                const parentId = children[childId];
                await this._loadElement(
                    document.body,
                    parentId,
                    childId,
                    elementConfigMap
                );
            }
        }
    }

    async _loadElement(parentElement, parentId, elementId, elementConfigMap) {
        if (elementId in elementConfigMap) {
            const elementConfig = elementConfigMap[elementId];
            const loaderMethod = elementConfig.loader;
            if (loaderMethod) {
                const parent = parentId ? parentElement.querySelector(parentId) : parentElement;
                await this._callMethod(
                    loaderMethod,
                    parent,
                    elementId,
                    elementConfigMap
                );
            }
            else {
                throw new Error(`Loader method not specified for element "${elementId}".`);
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

    async loadDiv(parentElement, elementId, elementConfigMap) {
        if (elementId in elementConfigMap) {
            const elementConfig = elementConfigMap[elementId];
            const div = document.createElement('div');
            div.id = elementId;
            if (elementConfig.class) {
                div.classList.add(elementConfig.class);
            }
            if (elementConfig.elementPath) {
                const html = await this._loadElementContent(elementConfig.elementPath);
                div.innerHTML = html;
            }
            if (elementConfig.cssPathes) {
                for (const cssPath of elementConfig.cssPathes) {
                    await cssLoader.loadCssFile(cssPath);
                }
            }
            if (elementConfig.children) {
                for (const childId in elementConfig.children) {
                    const parentId = elementConfig.children[childId];
                    await this._loadElement(
                        div,
                        parentId,
                        childId,
                        elementConfigMap
                    );
                }
            }
            parentElement.appendChild(div);
        }
    }

    async loadButton(parentElement, elementId, elementConfigMap) {
        if (elementId in elementConfigMap) {
            const elementConfig = elementConfigMap[elementId];
            const button = document.createElement('button');
            button.id = elementId;
            if (elementConfig.class) {
                button.classList.add(elementConfig.class);
            }
            if (elementConfig.elementPath) {
                const html = await this._loadElementContent(elementConfig.elementPath);
                button.innerHTML = html;
            }
            if (elementConfig.text) {
                const textNode = document.createTextNode(elementConfig.text);
                button.appendChild(textNode);
            }
            if (elementConfig.cssPathes) {
                for (const cssPath of elementConfig.cssPathes) {
                    await cssLoader.loadCssFile(cssPath);
                }
            }
            if (elementConfig.children) {
                for (const childId in elementConfig.children) {
                    const parentId = elementConfig.children[childId];
                    await this._loadElement(
                        button,
                        parentId,
                        childId,
                        elementConfigMap
                    );
                }
            }
            parentElement.appendChild(button);
        }
    }
}

const elementLoader = new ElementLoader();
