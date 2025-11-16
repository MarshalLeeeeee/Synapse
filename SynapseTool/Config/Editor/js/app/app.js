
class App {
    constructor() {
        this.attributes = []; // list of AttributeData
        this.rows = []; // list of RowData

        this.currentVersion = ''; // string, current selected version
        this.selectedCell = null; // css cell, current selected cell
        this.copiedRowData = null; // copied row data
        this.copiedCellValue = null; // copied cell data

        // DOM elements
        this.attributeRow = null;
        this.dataTypeRow = null;
        this.tableBody = null;
        this.contextMenu = null;
        this.addAttributeModal = null;
        this.versionSelect = null;
        this.copyCell = null;
        this.pasteCell = null;
        this.copyRow = null;
        this.pasteRow = null;
        this.editAttribute = null;
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
        this.tableBody = document.getElementById('tableBody');
        this.contextMenu = document.getElementById('contextMenu');
        this.addAttributeModal = document.getElementById('addAttributeModal');
        this.versionSelect = document.getElementById('versionSelect');
        this.copyCell = document.getElementById('copyCell');
        this.pasteCell = document.getElementById('pasteCell');
        this.copyRow = document.getElementById('copyRow');
        this.pasteRow = document.getElementById('pasteRow');
        this.editAttribute = document.getElementById('editAttribute');

        this.attributes.push(new AttributeData('Name', 'type-string'));
        this.attributes.push(new AttributeData('Age', 'type-int'));
        this.attributes.push(new AttributeData('Single', 'type-boolean'));
        this.attributes.push(new AttributeData('Skill_ids', 'type-list-int'));
        
        this.rows.push(new RowData(this.attributes));
        this.rows[0].updateAttributeValue(this.attributes[0], 'Tom');
        this.rows[0].updateAttributeValue(this.attributes[1], 10);
        this.rows[0].updateAttributeValue(this.attributes[2], true);
        this.rows[0].updateAttributeValue(this.attributes[3], [1,2,3]);
        this.rows.push(new RowData(this.attributes));
        this.rows[1].updateAttributeValue(this.attributes[0], 'Bob');
        this.rows[1].updateAttributeValue(this.attributes[1], 20);
        this.rows[1].updateAttributeValue(this.attributes[2], true);
        this.rows[1].updateAttributeValue(this.attributes[3], [2,4,6]);
        this.rows.push(new RowData(this.attributes));
        this.rows[2].updateAttributeValue(this.attributes[0], 'Alice');
        this.rows[2].updateAttributeValue(this.attributes[1], 30);
        this.rows[2].updateAttributeValue(this.attributes[2], false);
        this.rows[2].updateAttributeValue(this.attributes[3], [3,6,9]);
        console.log('rows on loaded', this.rows);
        
        this._initVersions();
        this._renderTable();
        this._setupEventListeners();
        console.log('rows on loaded over', this.rows);
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
        this._renderHeader();
        this._renderBody();
    }

    // render header
    _renderHeader() {
        // reset attribute and data type
        this.attributeRow.innerHTML = '<th class="style1">Attributes</th>';
        this.dataTypeRow.innerHTML = '<th class="style1">Data type</th>';
        
        // Add attribute and data type as new column
        this.attributes.forEach(attributeData => {
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
            const tableRow = document.createElement('tr');
            
            // Add row id cell
            const rowHeaderCell = document.createElement('td');
            rowHeaderCell.className = 'style1';
            rowHeaderCell.textContent = rowIdx + 1;
            rowHeaderCell.dataset.rowIdx = rowIdx;
            rowHeaderCell.dataset.isRowHeader = true;
            tableRow.appendChild(rowHeaderCell);
            
            // Add data cell
            this.attributes.forEach(attributeData => {
                const dataCell = document.createElement('td');
                dataCell.classList.add('style2')
                dataCell.dataset.attributeName = attributeData.name;
                dataCell.dataset.rowIdx = rowIdx;
                
                const cellData = rowData.cellValues[attributeData.name];
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
            this.copiedCellValue = this.rows[rowIdx].cellValues[attributeName].getCellValueByVersion(this.currentVersion);
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
            this.rows[rowIdx].cellValues[attributeName].updateVersionValueByText(this.currentVersion, this.copiedCellValue);
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
            const copiedRowData = new RowData(this.attributes);
            copiedRowData.copyFromCellValues(this.rows[rowIdx])
            this.copiedRowData = copiedRowData;
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
            this.rows[rowIdx].copyFromCellValues(this.copiedRowData);
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
        console.log('rows', this.rows);
        this.rows.push(new RowData(this.attributes));
        this._renderTable();
    }
    
    // insert new row below the selected row
    _doInsertNewRow() {
        if (this.selectedCell) {
            const rowIdx = this.selectedCell.dataset.rowIdx;
            this.rows.splice(rowIdx, 0, new RowData(this.attributes));
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
        const attributeName = document.getElementById('attributeName').value;
        const attributeDataType = document.getElementById('attributeDataType').value;
        
        if (!attributeName) {
            alert('Error: please input column name');
            return;
        }

        const hasDuplicate = false;
        this.attributes.forEach(attributeData => {
            if (attributeName === attributeData.name) {
                hasDuplicate = true;
            }
        });
        if (hasDuplicate) {
            alert('Error: has duplicate attribute');
            return;
        }
        
        const attributeData = new AttributeData(attributeName, attributeDataType);
        this.attributes.push(attributeData);
        this.rows.forEach(rowData => {
            rowData.updateAttributeValue(attributeData);
        });
        this.addAttributeModal.style.display = 'none';
        document.getElementById('attributeName').value = '';
        document.getElementById('attributeDataType').value = 'type-string';
        this.copiedRowData = null;
        this._renderTable();
    }
}

app = new App();
app.init();
