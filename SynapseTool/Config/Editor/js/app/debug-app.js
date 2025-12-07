
class DebugApp {
    constructor() {
        this.messageModal = null;
        this.confirmationModal = null;
        this.functionModal = null;

        this.showMessageBtn = null;
        this.showConfirmationBtn = null;
        this.showVoidFunctionBtn = null;
        this.showFullFunctionBtn = null;
    }

    async init() {
        try {
            await config.load();
            await elementLoader.loadElementTree();
            this._initOnLoaded();
            console.log('Debug app initialized successfully.');
        } catch (error) {
            console.error('Error during debug app initialization:', error);
        }
    }

    _initOnLoaded() {
        this.messageModal = new MessageModal(
            document.getElementById('messageModal'),
            'Debug Message',
            'This is a debug message modal.',
            () => this._onMsgConfirm()
        );
        this.confirmationModal = new ConfirmationModal(
            document.getElementById('confirmationModal'),
            'Debug Confirmation',
            'Are you sure you want to proceed?',
            () => this._onConfirmConfirm(),
            () => this._onConfirmCancel()
        );
        this.functionModal = new FunctionModal(
            document.getElementById('functionModal'),
        );
        this.showMessageBtn = new Btn(
            document.getElementById('btnShowMessageModal'),
            () => this.messageModal.setVisible(true)
        );
        this.showConfirmationBtn = new Btn(
            document.getElementById('btnShowConfirmationModal'),
            () => this.confirmationModal.setVisible(true)
        );
        this.showVoidFunctionBtn = new Btn(
            document.getElementById('btnShowVoidFunctionModal'),
            () => this._showVoidFunctionModal()
        );
        this.showFullFunctionBtn = new Btn(
            document.getElementById('btnShowFullFunctionModal'),
            () => this._showFullFunctionModal()
        );
    }

    _onMsgConfirm() {
        console.log('Debug message confirmed.');
    }

    _onConfirmConfirm() {
        console.log('Debug confirmation accepted.');
    }

    _onConfirmCancel() {
        console.log('Debug confirmation canceled.');
    }

    _onFunctionConfirm() {
        console.log('Function modal confirmed.');
    }

    _onFunctionCancel() {
        console.log('Function modal canceled.');
    }

    _showVoidFunctionModal() {
        this.functionModal.setData(
            'Void Function Modal',
            () => this._onFunctionConfirm(),
            () => this._onFunctionCancel(),
            []
        );
        this.functionModal.setVisible(true);
    }

    async _showFullFunctionModal() {
        const elementConfigs = [];
        const elementTextInputConfig = {
            'data-loader': 'addDiv',
            'data-class': 'modal-element',
            'data-element-path': 'element/modal_element/modal-element-text-input.html'
        }
        elementConfigs.push(elementTextInputConfig);
        await this.functionModal.setData(
            'Full Function Modal',
            () => this._onFunctionConfirm(),
            () => this._onFunctionCancel(),
            elementConfigs
        );
        this.functionModal.setVisible(true);
    }
}


debugApp = new DebugApp();
debugApp.init();
