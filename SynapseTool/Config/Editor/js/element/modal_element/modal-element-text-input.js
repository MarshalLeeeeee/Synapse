
class TextInputModalElement extends Element {
    constructor(domElement, title='', placeholder='', description='', onChange=null) {
        super(domElement);
        this._initialize();
        this.setData(title, placeholder, description, onChange);
    }

    setData(title, placeholder, description, onChange) {
        this.onChange = onChange;
        this._render(title, placeholder, description);
    }

    getValue() {
        const inputElement = this.domElement.querySelector('input');
        return inputElement.value;
    }

    _initialize() {
        this.domElement.querySelector('input').addEventListener('change', (event) => this._onChangeEvent(event));
    }

    _render(title, placeholder, description) {
        this.domElement.querySelector('label').innerText = title;
        this.domElement.querySelector('input').setAttribute('placeholder', placeholder);
        this.domElement.querySelector('small').innerText = description;
    }

    _onChangeEvent(event) {
        if (this.onChange) {
            this.onChange(event.target);
        }
    }
}
