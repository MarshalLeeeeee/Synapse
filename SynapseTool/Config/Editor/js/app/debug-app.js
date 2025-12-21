
class DebugApp {
    constructor() {
        // UI elements
        this.messageModal = null;
        this.confirmationModal = null;
        this.functionModal = null;
        this.fileDropArea = null;
        this.configTable = null;

        // config
        this.config = new Config();
        this.configFileName = '';

        // ui varialbles
        this.selectedAttributeUUid = '';
        this.selectedRowUUid = '';
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
        );
        this.confirmationModal = new ConfirmationModal(
            document.getElementById('confirmationModal'),
            'Debug Confirmation',
            'Are you sure you want to proceed?',
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
            (versoin) => this._onSelectVersion(versoin),
            (th) => this._onSelectTh(th),
            (td) => this._onSelectTd(td)
        );
        this._refreshView();
        await this._setToolButtons();
    }

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

    _refreshView() {
        const loaded = this.config.isLoaded();
        this.fileDropArea.setVisible(!loaded);
        this.configTable.setVisible(loaded);
    }

    _refreshConfigFileNameDisplay() {
        this.configTable.setTitle(this.configFileName);
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

    //#region ui state logic

    /* callback function: a th cell is selected */
    _onSelectTh(th) {
        console.log('select th', th);
        this._updateCellSelect(th.dataset.attributeUUid, '');
    }
    
    /* callback function: a td cell is selected */
    _onSelectTd(td) {
        console.log('select td', td);
        this._updateCellSelect(td.dataset.attributeUUid, td.dataset.rowUUid);
    }
    
    /* update the state of selected cell */
    _updateCellSelect(attributeUUid, rowUUid) {
        this.selectedAttributeUUid = attributeUUid;
        this.selectedRowUUid = rowUUid;
    }

    /* callback function: version is selected */
    _onSelectVersion(version) {
        this._refreshConfigTable();
    }
    
    //#endregion ui state logic

    //#region config management

    /* callback function: if json config is successfully loaded */
    _onLoadConfigSucc(file, content) {
        this._applyJson(file, content);
    }
    
    /* callback function: if json config fails to be loaded */
    _onLoadConfigFail(file, errorMsg) {
        this._showMessageModal(
            'Load Error',
            `Failed to load config file "${file.name}": ${errorMsg}`,
            null
        );
    }

    /* callback function: to create a new config */
    _onNewConfig() {
        console.log('New Config.');
    }

    /* callback function: to save config */
    _onSaveConfig() {
        this._saveConfig();
    }

    /* implement the json content to the current data */
    _applyJson(file, content) {
        try {
            if (this.config.load(content)) {
                this.configFileName = file.name;
                this._refreshView();
                this._refreshConfigFileNameDisplay();
                this._refreshConfigTable();
            }
            else {
                this._showMessageModal(
                    'Load Error',
                    `Failed to load config file "${file.name}": unable to parse the config`,
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

    /* dump config and download to local file system */
    _saveConfig() {
        const configDump = this.config.dump();

        // Create a blob with the config data
        const configJson = JSON.stringify(configDump, null, 2);
        const blob = new Blob([configJson], { type: 'application/json' });
        
        // Create a download link
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = this.configFileName || 'config.json';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    }

    //#endregion

    //#region attribute management

    /* callback function: to add a new attribute */
    _onAddAttribute() {
        this._showAddAttributeModal();
    }
    
    /* callback function: to edit a current attribute */
    _onEditAttribute() {
        console.log('on edit attribute');
    }

    /* show function modal for adding attribute */
    async _showAddAttributeModal() {
        const elementConfigs = [];
        elementConfigs.push({
            'id': 'functionModalAttributeNameInput',
            'data-tag': 'div',
            'data-class': 'modal-element',
            'data-element-path': 'element/modal_element/modal-element-text-input.html',
            'type': 'text-input',
            'title': 'Attribute name:',
            'placeholder': 'Input name of the attribute...'
        });
        await this.functionModal.setData(
            'Add new attribute',
            (values) => this._onAddAttributeConfirm(values),
            null,
            elementConfigs
        );
        this.functionModal.setVisible(true);
    }

    _onAddAttributeConfirm(values) {
        console.log('Function modal confirmed.', values);
    }

    //#endregion
}


debugApp = new DebugApp();
debugApp.init();
