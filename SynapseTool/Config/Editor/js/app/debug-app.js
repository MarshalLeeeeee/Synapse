
class DebugApp {
    constructor() {
        // UI elements
        this.messageModal = null;
        this.confirmationModal = null;
        this.functionModal = null;
        this.fileDropArea = null;
        this.configTable = null;
        this.showMessageBtn = null;
        this.showConfirmationBtn = null;
        this.showVoidFunctionBtn = null;
        this.showFullFunctionBtn = null;

        // data and logic
        this.configLoaded = false;
        this.configAttributes = []; // dict: attribute name -> AttributeData
        this.configAttributeOrder = []; // list: attribute name
        this.configRows = []; // list: RowData
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
        this.fileDropArea = new FileDropArea(
            document.getElementById('fileDropAreaView'),
            () => this._onNewConfig(),
            (file, content) => this._onLoadConfigSucc(file, content),
            (file, errorMsg) => this._onLoadConfigFail(file, errorMsg)
        );
        const options = [];
        options.push({'value': 'base', 'text': 'base'});
        options.push({'value': 'test', 'text': 'test'});
        this.configTable = new ConfigTable(
            document.getElementById('configTableView'),
            '',
            options,
            () => this._onSaveConfig(),
            (versoin) => this._onSelectVersion(versoin)
        );
        this.showMessageBtn = new Btn(
            document.getElementById('btnShowMessageModal'),
            'Show Message Modal',
            () => this._onShowMessageBtnClick()
        );
        this.showConfirmationBtn = new Btn(
            document.getElementById('btnShowConfirmationModal'),
            'Show Confirmation Modal',
            () => this.confirmationModal.setVisible(true)
        );
        this.showVoidFunctionBtn = new Btn(
            document.getElementById('btnShowVoidFunctionModal'),
            'Show Void Function Modal',
            () => this._showVoidFunctionModal()
        );
        this.showFullFunctionBtn = new Btn(
            document.getElementById('btnShowFullFunctionModal'),
            'Show Full Function Modal',
            () => this._showFullFunctionModal()
        );
        this._refreshView();
    }

    //#region element interact interfaces

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

    _onShowMessageBtnClick() {
        console.log('Show Message Modal button clicked.');
        this.messageModal.setVisible(true);
    }

    _onNewConfig() {
        console.log('New Config.');
    }

    _onLoadConfigSucc(file, content) {
        console.log('Load Config Success:', file, content);
        this._applyJson(file, content);
    }

    _onLoadConfigFail(file, errorMsg) {
        this._showMessageModal(
            'Load Error',
            `Failed to load config file "${file.name}": ${errorMsg}`,
            null
        );
    }

    _onSaveConfig() {
        this._saveConfig();
    }

    _onFunctionModalTextChanged(target) {
        if (target.value === 'LMC') {
            console.log('Function Modal Text Input Changed:', target.value);
        }
        else {
            target.value = '';
            console.log('Function Modal Text Input must be "LMC".');
        }
    }

    _onSelectVersion(version) {
        console.log('Selected version:', version);
    }

    //#endregion

    //#region element display functions

    /* show message modal
    Param:
        title: str, modal title
        message: str, modal message
        onConfirm: function() | null, callback when confirm button is clicked
     */
    _showMessageModal(title, message, onConfirm) {
        this.messageModal.setData(title, message, onConfirm);
        this.messageModal.setVisible(true);
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
            'data-element-path': 'element/modal_element/modal-element-text-input.html',
            'id': 'functionModalTextInput',
            'type': 'text-input',
            'title': 'Enter Text:',
            'placeholder': 'Type something...',
            'description': 'This is a breif description.',
            'onChange': (target) => this._onFunctionModalTextChanged(target)
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

    _refreshView() {
        this.fileDropArea.setVisible(!this.configLoaded);
        this.configTable.setVisible(this.configLoaded);
    }

    _refreshConfigFileNameDisplay(title) {
        this.configTable.setTitle(title);
    }

    //#endregion

    //#region data management

    /* implement the json content to the current data */
    _applyJson(file, content) {
        try {
            this.configLoaded = true;
            this._refreshConfigFileNameDisplay(file.name);
            this._refreshView();
        }
        catch (error) {

        }
    }

    _saveConfig() {
        if (!this.configLoaded) return;
        console.log('Config saved.');
    }

    //#endregion
}


debugApp = new DebugApp();
debugApp.init();
