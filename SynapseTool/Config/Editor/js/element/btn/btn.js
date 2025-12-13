
class Btn extends Element {
    constructor(domElement, text, callback=null) {
        super(domElement);
        this._initialize(text, callback);
    }

    setText(text) {
        this.textNode.nodeValue = text;
    }

    setCallback(callback) {
        this.callback = callback;
    }

    _initialize(text, callback) {
        this.callback = callback;
        this.domElement.addEventListener('click', () => this._onClick());
        const textNode = document.createTextNode(text);
        this.domElement.appendChild(textNode);
        this.textNode = textNode;
    }

    _onClick() {
        if (this.callback) {
            this.callback();
        }
    }
}
