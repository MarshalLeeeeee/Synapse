
class Selector extends Element {
    constructor(domElement, options=null, onSelect=null) {
        super(domElement);
        this.onSelect = onSelect;
        this.setOptions(options);
        this._initialize();
    }

    setOptions(options) {
        this.domElement.innerHTML = '';
        if (!options) return;
        options.forEach(optionData => {
            const option = document.createElement('option');
            option.value = optionData['value'];
            option.textContent = optionData['text'];
            this.domElement.appendChild(option);
        });
        this._onSelect();
    }

    getOption() {
        return this.domElement.value;
    }

    _initialize() {
        this.domElement.addEventListener('change', () => this._onSelect());
    }

    _onSelect() {
        if (this.onSelect) {
            this.onSelect(this.getOption());
        }
    }
}