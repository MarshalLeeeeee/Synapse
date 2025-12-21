
class FunctionModal extends Modal {
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
        await this._renderElements(title, element_configs);
    }
    
    _initialize() {
        this.btnClose = new Btn(
            document.getElementById('functionModalBtnClose'),
            '×',
            () => this.setVisible(false)
        );
        this.btnConfirm = new Btn(
            document.getElementById('functionModalBtnConfirm'),
            'Confirm',
            () => this._onConfirm()
        );
        this.btnCancel = new Btn(
            document.getElementById('functionModalBtnCancel'),
            'Cancel',
            () => this._onCancel()
        );
        this.modalElements = new Container(document.getElementById('functionModalBody'));
    }
    
    async _renderElements(title, element_configs) {
        document.getElementById('functionModalTxtTitle').innerText = title;
        await this.modalElements.setChildren(
            element_configs,
            () => this._childrenGenerator(element_configs)
        );
    }

    _onConfirm() {
        if (this.onConfirm) {
            this.onConfirm(this.modalElements.getElementValues());
        }
        this.setVisible(false);
    }

    _onCancel() {
        if (this.onCancel) {
            this.onCancel(this.modalElements.getElementValues());
        }
        this.setVisible(false);
    }

    _childrenGenerator(element_configs) {
        const res = {};
        element_configs.forEach(config => {
            const elementId = config['id'];
            const element = document.getElementById(elementId);
            if (element == null) return;
            res[config['id']] = ModalElementFactory.createElement(element, config);
        });
        return res;
    }
}
