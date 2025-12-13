
class ConfigTable extends Element {
    constructor(domElement, title='', onSave=null) {
        super(domElement);
        this.onSave = onSave;
        this._render(title);
        this._initialize();
    }

    setTitle(title) {
        this._render(title);
    }

    _initialize() {
        this.btnSave = new Btn(document.getElementById('configSaveBtn'), () => this._onBtnSaveClick());
    }

    _render(title) {
        document.getElementById('configFileName').innerText = title;
    }

    _onBtnSaveClick() {
        if (this.onSave) {
            this.onSave();
        }
    }
}