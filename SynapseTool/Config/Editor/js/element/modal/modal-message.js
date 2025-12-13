
class MessageModal extends Modal {
    constructor(domElement, title='', message='', onConfirm=null) {
        super(domElement);
        this.onConfirm = onConfirm;
        this._initialize();
        this._render(title, message);
        this.setVisible(false);
    }

    setData(title, message, onConfirm) {
        this.onConfirm = onConfirm;
        this._render(title, message);
    }

    _initialize() {
        this.btnClose = new Btn(
            document.getElementById('messageModalBtnClose'),
            '×',
            () => this.setVisible(false)
        );
        this.btnConfirm = new Btn(
            document.getElementById('messageModalBtnConfirm'),
            'OK',
            () => this._onConfirm()
        );
    }
    
    _render(title, message) {
        document.getElementById('messageModalTitle').innerText = title;
        document.getElementById('messageModalMessage').innerText = message;
    }

    _onConfirm() {
        if (this.onConfirm) {
            this.onConfirm();
        }
        this.setVisible(false);
    }
}
