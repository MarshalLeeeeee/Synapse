
class RadioModalElement extends Element {
    constructor(domElement, title='', radios=[], radioGroupId='') {
        super(domElement);
        this._render(title, radios, radioGroupId);
    }

    setData(title, radios, radioGroupId) {
        this._render(title, radios, radioGroupId);
    }

    getValue() {
        const radioGroupElement = this.domElement.querySelector('.radio-group');
        const selectedRadio = radioGroupElement.querySelector('input[type="radio"]:checked');
        return selectedRadio ? selectedRadio.value : null;
    }

    _render(title, radios, radioGroupId) {
        const labelElement = this.domElement.querySelector('label');
        labelElement.innerText = title;
        const radioGroupElement = this.domElement.querySelector('.radio-group');
        radioGroupElement.id = radioGroupId;
        radioGroupElement.innerHTML = '';
        radios.forEach((radioData, index) => {
            const radioWrapper = document.createElement('div');
            radioWrapper.classList.add('radio-option');
            const radioInput = document.createElement('input');
            radioInput.type = 'radio';
            radioInput.id = radioData['id'];
            radioInput.name = radioData['name'];
            radioInput.value = radioData['value'];
            if (index === 0) {
                radioInput.checked = true;
            }
            const radioLabel = document.createElement('label');
            radioLabel.setAttribute('for', radioData['id']);
            radioLabel.innerText = radioData['text'];
            radioWrapper.appendChild(radioInput);
            radioWrapper.appendChild(radioLabel);
            radioGroupElement.appendChild(radioWrapper);
        });
    }
}