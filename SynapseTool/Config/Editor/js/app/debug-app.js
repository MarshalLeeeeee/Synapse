
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

        // config
        this.config = new Config();
    }

    async init() {
        try {
            await consts.load();
            await elementLoader.loadElement(document.body);
            await this._initOnLoaded();
            console.log('Debug app initialized successfully.');
            console.log(crypto.randomUUID());
        } catch (error) {
            console.error('Error during debug app initialization:', error);
        }
    }

    async _initOnLoaded() {
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
        consts.versions.forEach(version => {
            options.push({'value': version, 'text': version});
        });
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
        await this._setToolButtons();
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
        this._refreshConfigTable();
    }

    _onAddAttribute() {
        console.log('on add attribute');
    }

    _onEditAttribute() {
        console.log('on edit attribute');
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
            'data-tag': 'addDiv',
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
        const loaded = this.config.isLoaded();
        this.fileDropArea.setVisible(!loaded);
        this.configTable.setVisible(loaded);
    }

    _refreshConfigFileNameDisplay(title) {
        this.configTable.setTitle(title);
    }

    _refreshConfigTable() {
        this.configTable.renderTable(this.config)
    }

    _getToolButtonConfigs() {
        const configs = [];
        configs.push({
            'id': 'addAttributeBtn',
            'text': 'Add Attribute',
            'callback': () => this._onAddAttribute(),
            'data-tag': 'button',
            'data-class': 'primary',
            'data-css-path': ['css/btn/primary.css'],
        });
        configs.push({
            'id': 'editAttributeBtn',
            'text': 'Edit Attribute',
            'callback': () => this._onEditAttribute(),
            'data-tag': 'button',
            'data-class': 'primary',
            'data-css-path': ['css/btn/primary.css'],
        });
        return configs;
    }

    async _setToolButtons() {
        const configs = this._getToolButtonConfigs();
        await this.configTable.setButtons(configs);
    }

    //#endregion

    //#region data management

    /* implement the json content to the current data */
    _applyJson(file, content) {
        try {
            if (this.config.load(content)) {
                this._refreshView();
                this._refreshConfigFileNameDisplay(file.name);
                this._refreshConfigTable();
            }
            else {
                this._showMessageModal(
                    'Load Error',
                    `Failed to load config file "${file.name}": unable to parse to config`,
                    null
                );
            }
        }
        catch (error) {
            this._showMessageModal(
                'Load Error',
                `Failed to load config file "${file.name}": ${error.message}`,
                null
            );
        }
    }

    _saveConfig() {
        console.log('Config saved.');
    }

    //#endregion
}


debugApp = new DebugApp();
debugApp.init();
