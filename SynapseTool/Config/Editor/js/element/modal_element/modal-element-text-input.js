
class TextInputModalElement extends Element {
    constructor(domElement, title='', placeholder='', description='') {
        super(domElement);
        // TODO input listener
        this._render(title, placeholder, description);
    }

    setData(title, placeholder, description) {
        this._render(title, placeholder, description);
    }

    getValue() {
        const inputElement = this.domElement.querySelector('input');
        return inputElement.value;
    }

    _render(title, placeholder, description) {
        this.domElement.querySelector('label').innerText = title;
        this.domElement.querySelector('input').setAttribute('placeholder', placeholder);
        this.domElement.querySelector('small').innerText = description;
    }
}
