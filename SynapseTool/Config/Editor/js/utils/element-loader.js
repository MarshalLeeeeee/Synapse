/* element loader
 * build element tree externally
 */
class ElementLoader {

    /* replace all <load-element> with target content recursively. */
    async loadElement(element) {
        const loadElements = element.querySelectorAll('load-element');
        for (const loadElement of loadElements) {
            const elementLoaderElement = loadElement.querySelector('data-tag');
            if (!elementLoaderElement) {
                continue;
            }
            const element = await this._createElement(loadElement);
            if (element == null) {
                continue;
            }
            loadElement.replaceWith(element);
            const cssPathElements = loadElement.querySelectorAll('data-css-path');
            for (const cssPathElement of cssPathElements) {
                await cssLoader.loadCssFile(cssPathElement.textContent);
            }
        }
    }

    /* fetch element path locally
     * Parameters:
     * - elementPath: str, file path for html content
     * Return:
     * - html: str, file content as html
     */
    async _loadElementContent(elementPath) {
        const response = await fetch(elementPath);
        if (!response.ok) {
            throw new Error(`Failed to load element from path "${elementPath}".`);
        }

        const html = await response.text();
        return html;
    }

    /* create element from <load-element>
     * Require: <load-element> has tag <data-tag>
     */
    async _createElement(loadElement) {
        const elementLoaderElement = loadElement.querySelector('data-tag');
        if (!elementLoaderElement) {
            return null;
        }

        const element = document.createElement(elementLoaderElement.textContent);
        element.id = loadElement.id;
        const elementClassElement = loadElement.querySelector('data-class');
        if (elementClassElement) {
            element.classList.add(elementClassElement.textContent);
        }
        const elementPathElement = loadElement.querySelector('data-element-path');
        if (elementPathElement) {
            const html = await this._loadElementContent(elementPathElement.textContent);
            element.innerHTML = html;
        }
        await this.loadElement(element);
        return element;
    }

    /* create a <load-element> element
     * Parameter:
     * - config: dict, contains
     */
    createLoadElement(config) {
        const loadElement = document.createElement('load-element');
        loadElement.id = config['id'];

        const dataTag = config['data-tag'];
        if (dataTag) {
            const dataLoaderElement = document.createElement('data-tag');
            dataLoaderElement.innerText = dataTag;
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

        return loadElement;
    }
}

const elementLoader = new ElementLoader();
