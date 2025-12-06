
class App {
    constructor() {
        this.hasData = false; // bool: has data loaded
        this.attributes = []; // dict: attribute name -> AttributeData
        this.attributeOrder = []; // list: attribute name
        this.rows = []; // list: RowData

        this.currentVersion = ''; // string, current selected version
        this.selectedCell = null; // css cell, current selected cell
        this.copiedRowData = null; // copied row data
        this.copiedCellValue = null; // copied cell data
    }

    async init() {
        try {
            await config.load();
            await elementLoader.loadElementTree();
            this._initOnLoaded();
            console.log('App initialized successfully.');
        } catch (error) {
            console.error('Error during app initialization:', error);
        }
    }
    
    _initOnLoaded() {
        // DOM elements
        this.attributeRow = document.getElementById('attributeRow');
        this.dataTypeRow = document.getElementById('dataTypeRow');
        this.toolbar = document.getElementById('toolbar');
        this.tableContainer = document.getElementById('tableContainer');
        this.tableBody = document.getElementById('tableBody');
        this.contextMenu = document.getElementById('contextMenu');
        this.addAttributeModal = document.getElementById('addAttributeModal');
        this.versionSelect = document.getElementById('versionSelect');
        this.copyCell = document.getElementById('copyCell');
        this.pasteCell = document.getElementById('pasteCell');
        this.copyRow = document.getElementById('copyRow');
        this.pasteRow = document.getElementById('pasteRow');
        this.editAttribute = document.getElementById('editAttribute');
        this.dragFile = document.getElementById('dragFile');
        
        this._initVersions();
        this._renderTable();
        this._setupEventListeners();
        this._updateHasDataView();
    }

    // init version selector choices
    _initVersions() {
        const versions = config.versions;
        for (const version of versions) {
            const option = document.createElement('option');
            const textNode = document.createTextNode(version);
            option.appendChild(textNode);
            option.value = version;
            this.versionSelect.appendChild(option);
        }
        this.currentVersion = versions[0];
    }

    // render table
    _renderTable() {
        console.log('Debug: data');
        console.log(this.attributes);
        console.log(this.attributeOrder);
        console.log(this.rows);
        this._renderHeader();
        this._renderBody();
    }

    // render header
    _renderHeader() {
        // reset attribute and data type
        this.attributeRow.innerHTML = '<th class="style1">Attributes</th>';
        this.dataTypeRow.innerHTML = '<th class="style1">Data type</th>';
        
        // Add attribute and data type as new column
        this.attributeOrder.forEach(attributeName => {
            const attributeData = this.attributes[attributeName];

            // attribute name
            const headerCell = document.createElement('th');
            headerCell.className = 'style2';
            headerCell.textContent = attributeData.name;
            this.attributeRow.appendChild(headerCell);
            
            // data type
            const dataTypeCell = document.createElement('th');
            dataTypeCell.className = 'style3';
            const typeTag = document.createElement('span');
            typeTag.className = `data-type-tag ${attributeData.dataType}`;
            typeTag.textContent = CellValueFactory.getDisplayName(attributeData.dataType);
            dataTypeCell.appendChild(typeTag);
            this.dataTypeRow.appendChild(dataTypeCell);
        });
    }

    // render table
    _renderBody() {
        // reset table
        this.tableBody.innerHTML = '';

        // Add new row
        this.rows.forEach((rowData, rowIdx) => {
            // check row versions
            if (rowData.versions.indexOf(this.currentVersion) == -1) return;

            // Add row id cell
            const tableRow = document.createElement('tr');
            const rowHeaderCell = document.createElement('td');
            rowHeaderCell.className = 'style1';
            rowHeaderCell.textContent = rowIdx + 1;
            rowHeaderCell.dataset.rowIdx = rowIdx;
            rowHeaderCell.dataset.isRowHeader = true;
            tableRow.appendChild(rowHeaderCell);
            
            // Add data cell
            this.attributeOrder.forEach(attributeName => {
                const attributeData = this.attributes[attributeName];

                const dataCell = document.createElement('td');
                dataCell.classList.add('style2');
                dataCell.dataset.attributeName = attributeData.name;
                dataCell.dataset.rowIdx = rowIdx;
                
                const cellData = rowData.getCellData(attributeData.name);
                const input = document.createElement('input');
                input.type = 'text';
                input.value = cellData.getCellValueByVersion(this.currentVersion).parse();
                input.addEventListener('change', (e) => {
                    this._handleCellInputChange(cellData, e.target.value);
                });
                input.addEventListener('blur', (e) => {
                    this._handleCellInputChange(cellData, e.target.value);
                });
                dataCell.appendChild(input);
                tableRow.appendChild(dataCell);
            });
            
            this.tableBody.appendChild(tableRow);
        });
        this.selectedCell = null;
    }

    _handleCellInputChange(cellData, newValue) {
        cellData.updateVersionValueByText(this.currentVersion, newValue);
        this._renderTable();
    }

    _updateHasDataView() {
        if (this.hasData) {
            this.toolbar.style.display = 'flex';
            this.tableContainer.style.display = 'flex';
            this.dragFile.style.display = 'none';
        }
        else {
            this.toolbar.style.display = 'none';
            this.tableContainer.style.display = 'none';
            this.dragFile.style.display = 'flex';
        }
    }

    // setup event listeners
    _setupEventListeners() {
        // show add attribute popup
        document.getElementById('addAttributeBtn').addEventListener('click', () => {
            this.addAttributeModal.style.display = 'flex';
        });

        // add new row
        document.getElementById('addRowBtn').addEventListener('click', () => this._doAddNewRow());

        // insert new row
        document.getElementById('insertRowBtn').addEventListener('click', () => this._doInsertNewRow());

        // version select
        this.versionSelect.addEventListener('change', (e) => {
            this.currentVersion = e.target.value;
            this._renderTable();
        });
        
        // close modal
        document.querySelectorAll('.close-modal').forEach(button => {
            button.addEventListener('click', () => {
                this.addAttributeModal.style.display = 'none';
            });
        });
        
        // create attribute btn
        document.getElementById('createAttributeBtn').addEventListener('click', () => this._doCreateAttribute());
        
        // select cell
        this.tableBody.addEventListener('click', (e) => {
            if (e.target.tagName === 'TD' || e.target.tagName === 'INPUT') {
                const cell = e.target.tagName === 'TD' ? e.target : e.target.parentElement;
                this._selectCell(cell);
            }
        });
        
        // show right click context
        document.addEventListener('contextmenu', (e) => {
            if (e.target.tagName === 'TD' || e.target.tagName === 'TH') {
                e.preventDefault();
                this._showContextMenu(e);
            }
        });
        
        // hide right click context
        document.addEventListener('click', () => {
            this.contextMenu.style.display = 'none';
        });
        
        // right click context
        this.copyCell.addEventListener('click', () => this._doCopyCell());
        this.pasteCell.addEventListener('click', () => this._doPasteCell());
        this.copyRow.addEventListener('click', () => this._doCopyRow());
        this.pasteRow.addEventListener('click', () => this._doPasteRow());
        this.editAttribute.addEventListener('click', () => this._doEditAttribute());

        // drag file
        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            this.dragFile.addEventListener(eventName, (e) => this._doDragPreventDefaults(e), false);
        });
        ['dragenter', 'dragover'].forEach(eventName => {
            this.dragFile.addEventListener(eventName, () => this._activeDragFile(), false);
        });
        ['dragleave', 'drop'].forEach(eventName => {
            this.dragFile.addEventListener(eventName, () => this._inactiveDragFile(), false);
        });
        this.dragFile.addEventListener('drop', (e) => this._onFileDragger(e), false);
    }

    // select cell
    _selectCell(cell) {
        // inactive the previous selected
        document.querySelectorAll('td.active').forEach(c => {
            c.classList.remove('active');
        });
        
        // active selected cell
        cell.classList.add('active');
        this.selectedCell = cell;
    }

    // show right click context
    _showContextMenu(e) {
        this.contextMenu.style.display = 'block';
        this.contextMenu.style.left = `${e.pageX}px`;
        this.contextMenu.style.top = `${e.pageY}px`;

        if (e.target.tagName === 'TH') { // attribute cell
            this.copyCell.style.display = 'none';
            this.pasteCell.style.display = 'none';
            this.copyRow.style.display = 'none';
            this.pasteRow.style.display = 'none';
            this.editAttribute.style.display = 'block';
        } else {
            if (e.target.dataset.isRowHeader) { // row header cell
                this.copyCell.style.display = 'none';
                this.pasteCell.style.display = 'none';
                this.copyRow.style.display = 'block';
                this.pasteRow.style.display = 'block';
                this.editAttribute.style.display = 'none';
            }
            else { // common data cell
                this.copyCell.style.display = 'block';
                this.pasteCell.style.display = 'block';
                this.copyRow.style.display = 'none';
                this.pasteRow.style.display = 'none';
                this.editAttribute.style.display = 'none';
            }
        }
    }

    // copy cell
    _doCopyCell() {
        if (this.selectedCell) {
            const rowIdx = this.selectedCell.dataset.rowIdx;
            const attributeName = this.selectedCell.dataset.attributeName;
            const rowData = this.rows[rowIdx];
            const cellData = rowData.getCellData(attributeName)
            this.copiedCellValue = cellData.getCellValueByVersion(this.currentVersion);
        }
        else {
            alert('no selected cell');
        }
        this.contextMenu.style.display = 'none';
    }

    // paste cell
    _doPasteCell() {
        if (this.selectedCell && this.copiedCellValue !== null) {
            const rowIdx = this.selectedCell.dataset.rowIdx;
            const attributeName = this.selectedCell.dataset.attributeName;
            const rowData = this.rows[rowIdx];
            const cellData = rowData.getCellData(attributeName)
            cellData.updateVersionValueByText(this.currentVersion, this.copiedCellValue);
            this._renderTable();
        }
        else if (this.copiedCellValue == null) {
            alert('no copied data');
        }
        else {
            alert('no selected cell')
        }
        this.contextMenu.style.display = 'none';
    }

    // copy row
    _doCopyRow() {
        if (this.selectedCell && this.selectedCell.dataset.isRowHeader) {
            const rowIdx = this.selectedCell.dataset.rowIdx;
            this.copiedRowData = this.rows[rowIdx].getRawData();
        }
        else {
            alert('no selected row header cell');
        }
        this.contextMenu.style.display = 'none';
    }

    // paste row
    _doPasteRow() {
        if (this.copiedRowData == null) {
            alert('no copied row data');
        }
        else if (this.selectedCell && this.selectedCell.dataset.isRowHeader) {
            const rowIdx = this.selectedCell.dataset.rowIdx;
            this.rows[rowIdx].updateData(this.attributes, this.copiedRowData);
            this._renderTable();
        }
        else {
            alert('no selected row head cell');
        }
        this.contextMenu.style.display = 'none';
    }

    // edit attribute
    _doEditAttribute() {
        alert('TODO');
        this.contextMenu.style.display = 'none';
    }

    // add new row at the end
    _doAddNewRow() {
        const rowData = {
            'Versions': [this.currentVersion],
            'Cells': []
        };
        this.rows.push(new RowData(this.attributes, rowData));
        this._renderTable();
    }
    
    // insert new row below the selected row
    _doInsertNewRow() {
        if (this.selectedCell) {
            const rowIdx = this.selectedCell.dataset.rowIdx;
            const rowData = {
                'Versions': [this.currentVersion],
                'Cells': []
            };
            this.rows.splice(rowIdx, 0, new RowData(this.attributes, rowData));
            this._renderTable();
        }
        else {
            alert('no selected cell');
        }
    }

    // create attribute
    // - check empty
    // - check duplication
    _doCreateAttribute() {
        // TODO
        const attributeName = document.getElementById('attributeName').value;
        const attributeDataType = document.getElementById('attributeDataType').value;
        
        if (!attributeName) {
            alert('Error: please input column name');
            return;
        }

        const hasDuplicate = false;
        Object.keys(this.attributes).forEach(key => {
            if (attributeName === key) {
                hasDuplicate = true;
            }
        });
        if (hasDuplicate) {
            alert('Error: has duplicate attribute');
            return;
        }
        
        const attributeData = new AttributeData(attributeName, {'dataType': attributeDataType});
        this.attributes[attributeName] = attributeData;
        this.attributeOrder.push(attributeName);
        this.rows.forEach(rowData => {
            rowData.addAttribute(attributeData);
        });
        this.addAttributeModal.style.display = 'none';
        document.getElementById('attributeName').value = '';
        document.getElementById('attributeDataType').value = 'type-string';
        this.copiedRowData = null;
        this._renderTable();
    }

    _doDragPreventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    _activeDragFile() {
        this.dragFile.classList.add('active');
    }
    
    _inactiveDragFile() {
        this.dragFile.classList.remove('active');
    }

    _onFileDragger(e) {
        const dt = e.dataTransfer;
        const files = dt.files;
        
        if (files.length) {
            const file = files[0];
            if (file.type !== 'application/json' && !file.name.endsWith('.json')) {
                alert('please select json file');
                return;
            }
            
            const reader = new FileReader();
            reader.onload = (ee) => {
                const jsond = JSON.parse(ee.target.result);
                this.attributes = {};
                Object.entries(jsond['Attributes']).forEach(([key, value]) => {
                    this.attributes[key] = new AttributeData(key, value);
                });
                this.attributeOrder = jsond['AttributeOrder'];
                (jsond['Rows']).forEach(rowData => {
                    this.rows.push(new RowData(this.attributes, rowData));
                });
                this.hasData = true;
                this._updateHasDataView();
                this._renderTable();
            };
            reader.readAsText(file);
        }
    }
}

app = new App();
app.init();
