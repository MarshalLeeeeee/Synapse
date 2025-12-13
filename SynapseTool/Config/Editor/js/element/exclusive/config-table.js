
class ConfigTable extends Element {
    constructor(domElement, title='', options=null, onSave=null, onSelectVersion=null) {
        super(domElement);
        this.onSave = onSave;
        this.onSelectVersion = onSelectVersion;
        this._render(title);
        this._initialize(options);
    }

    setTitle(title) {
        this._render(title);
    }

    setOptions(options) {
        this.versionSelector.setOptions(options);
    }

    _initialize(options) {
        this.btnSave = new BtnFas(
            document.getElementById('configSaveBtn'),
            'Save Config',
            () => this._onBtnSaveClick(),
            'fa-save'
        );
        this.versionSelector = new Selector(
            document.getElementById('versionSelector'),
            options,
            (version) => this._onSelectVersion(version)
        );
    }

    _render(title) {
        document.getElementById('configFileName').innerText = title;
    }

    _onBtnSaveClick() {
        if (this.onSave) {
            this.onSave();
        }
    }

    _onSelectVersion(version) {
        if (this.onSelectVersion) {
            this.onSelectVersion(version);
        }
    }
}