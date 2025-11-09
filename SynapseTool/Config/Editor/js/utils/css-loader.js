/* css loader
 * load css files dynamically without duplication
 */
class CssLoader {
    constructor() {
        this.loadedCssFiles = new Set();
    }

    async loadCssFile(cssPath) {
        if (this.loadedCssFiles.has(cssPath)) {
            return;
        }
        return new Promise((resolve, reject) => {
            const linkElement = document.createElement('link');
            linkElement.rel = 'stylesheet';
            linkElement.href = cssPath;
            linkElement.onload = () => {
                this.loadedCssFiles.add(cssPath);
                resolve();
            };
            linkElement.onerror = () => {
                reject(new Error(`Failed to load CSS file from path "${cssPath}".`));
            }
            document.head.appendChild(linkElement);
        });
    }
}

cssLoader = new CssLoader();
