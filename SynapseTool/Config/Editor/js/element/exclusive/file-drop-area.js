
class FileDropArea extends Element {
    constructor(domElement, onNewConfig=null, onLoadConfigSucc=null, onLoadConfigFail=null) {
        super(domElement);
        this.onNewConfig = onNewConfig;
        this.onLoadConfigSucc = onLoadConfigSucc;
        this.onLoadConfigFail = onLoadConfigFail;
        this._initialize();
    }

    _initialize() {
        this.btnNew = new BtnFas(
            document.getElementById('fileDropAreaBtnNew'),
            'New Config',
            () => this._onBtnNewClick(),
            'fa-plus'
        );
        this.btnLoad = new BtnFas(
            document.getElementById('fileDropAreaBtnLoad'),
            'Load Config',
            () => this._onBtnLoadClick(),
            'fa-folder-open'
        );
        this.fileInput = document.getElementById('fileInput');
        this.fileInput.addEventListener('change', (e) => this._handleFileSelect(e));
        this.fileDropArea = document.getElementById('fileDropArea');
        this.fileDropArea.addEventListener('dragover', (e) => this._handleDragOver(e));
        this.fileDropArea.addEventListener('dragleave', (e) => this._handleDragLeave(e));
        this.fileDropArea.addEventListener('drop', (e) => this._handleDrop(e));
    }

    _onBtnNewClick() {
        if (this.onNewConfig) {
            this.onNewConfig();
        }
    }

    _onBtnLoadClick() {
        this.fileInput.click();
    }

    _handleFileSelect(event) {
        const files = event.target.files;
        if (files.length > 0) {
            const file = files[0];
            this._loadFile(file);
        }
    }

    _handleDragOver(event) {
        event.preventDefault();
        this.fileDropArea.classList.add('dragover');
    }

    _handleDragLeave(event) {
        event.preventDefault();
        this.fileDropArea.classList.remove('dragover');
    }

    _handleDrop(event) {
        this._handleDragLeave(event);
        const files = event.dataTransfer.files;
        if (files.length > 0) {
            const file = files[0];
            this._loadFile(file);
        }
    }

    _loadFile(file) {
        if (file.type === 'application/json' || file.name.endsWith('.json')) {
            const reader = new FileReader();
            reader.onload = (e) => {
                try {
                    const content = JSON.parse(e.target.result);
                    if (this.onLoadConfigSucc) {
                        this.onLoadConfigSucc(file, content);
                    }
                }
                catch (error) {
                    if (this.onLoadConfigFail) {
                        this.onLoadConfigFail(file, error.message);
                    }
                }

            };
            reader.readAsText(file);
        }
        else {
            if (this.onLoadConfigFail) {
                this.onLoadConfigFail(file, 'Invalid file type. Only JSON files are supported.');
            }
        }
    }
}
