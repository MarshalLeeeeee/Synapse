
class FunctionModal extends Element {
    constructor(domElement) {
        super(domElement);
        this.onConfirm = null;
        this.onCancel = null;
        this._initialize();
        this.setVisible(false);
    }

    async setData(title, onConfirm, onCancel, element_configs) {
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;
        await this._render(title, element_configs);
    }
    
    _initialize() {
        this.btnClose = new Btn(document.getElementById('functionModalBtnClose'), () => this.setVisible(false));
        this.btnConfirm = new Btn(document.getElementById('functionModalBtnConfirm'), () => this._onConfirm());
        this.btnCancel = new Btn(document.getElementById('functionModalBtnCancel'), () => this._onCancel());
        this.modalElements = new ModalElementContainer(document.getElementById('functionModalBody'));
    }
    
    async _render(title, element_configs) {
        document.getElementById('functionModalTxtTitle').innerText = title;
        await this.modalElements.setElements(element_configs);
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
