
class SelectorModalElement extends ModalElement {
    constructor(domElement, title='', options=[], selectId='') {
        super(domElement);
        this._render(title, options, selectId);
    }

    setData(title, options, selectId) {
        this._render(title, options, selectId);
    }

    getValue() {
        const selectElement = this.domElement.querySelector('select');
        return selectElement.value;
    }

    _render(title, options, selectId) {
        const labelElement = this.domElement.querySelector('label')
        labelElement.innerText = title;
        labelElement.setAttribute('for', selectId);
        const selectElement = this.domElement.querySelector('select');
        selectElement.id = selectId;
        options.forEach(option => {
            const optionElement = document.createElement('option');
            optionElement.value = option['value'];
            optionElement.innerText = option['text'];
            selectElement.appendChild(optionElement);
        });
    }
}
