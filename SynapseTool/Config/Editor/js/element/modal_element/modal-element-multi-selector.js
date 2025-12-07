
class MultiSelectorModalElement extends Element {
    constructor(domElement, title='', options=[]) {
        super(domElement);
        this._render(title, options);
    }

    setData(title, options) {
        this._render(title, options);
    }

    getValue() {
        const versionGroupElement = this.domElement.querySelector('.multi-select-group');
        const selectedOptions = [];
        const checkboxes = versionGroupElement.querySelectorAll('input[type="checkbox"]:checked');
        checkboxes.forEach((checkbox) => {
            selectedOptions.push(checkbox.value);
        });
        return selectedOptions;
    }

    _render(title, options) {
        const labelElement = this.domElement.querySelector('label');
        labelElement.innerText = title;
        const versionGroupElement = this.domElement.querySelector('.multi-select-group');
        versionGroupElement.innerHTML = '';
        options.forEach((option) => {
            const optionWrapper = document.createElement('div');
            optionWrapper.classList.add('multi-select-option');
            const optionInput = document.createElement('input');
            optionInput.type = 'checkbox';
            optionInput.id = `multiSelect-${option['value']}`;
            optionInput.value = option['value'];
            const optionLabel = document.createElement('label');
            optionLabel.setAttribute('for', `multiSelect-${option['value']}`);
            optionLabel.innerText = option['text'];
            optionWrapper.appendChild(optionInput);
            optionWrapper.appendChild(optionLabel);
            versionGroupElement.appendChild(optionWrapper);
        });
    }
}