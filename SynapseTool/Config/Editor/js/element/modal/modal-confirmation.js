
class ConfirmationModal extends Element {
    constructor(domElement, title='', message='', onConfirm=null, onCancel=null) {
        super(domElement);
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;
        this._initialize();
        this._render(title, message);
        this.setVisible(false);
    }

    setData(title, message, onConfirm, onCancel) {
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;
        this._render(title, message);
    }

    _initialize() {
        this.btnClose = new Btn(document.getElementById('confirmationModalBtnClose'), () => this.setVisible(false));
        this.btnConfirm = new Btn(document.getElementById('confirmationModalBtnConfirm'), () => this._onConfirm());
        this.btnCancel = new Btn(document.getElementById('confirmationModalBtnCancel'), () => this._onCancel());
    }
    
    _render(title, message) {
        document.getElementById('confirmationModalTitle').innerText = title;
        document.getElementById('confirmationModalMessage').innerText = message;
    }

    _onConfirm() {
        if (this.onConfirm) {
            this.onConfirm();
        }
        this.setVisible(false);
    }

    _onCancel() {
        if (this.onCancel) {
            this.onCancel();
        }
        this.setVisible(false);
    }
}
