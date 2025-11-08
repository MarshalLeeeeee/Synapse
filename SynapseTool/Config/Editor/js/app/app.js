
class App {
    constructor() {
        // table data
        this.state = {
            attributes: [
                { name: 'Name', type: 'string' },
                { name: 'Age', type: 'int' },
                { name: 'Single', type: 'bool' },
                { name: 'Skill_ids', type: 'list-int' }
            ],
            rows: [
                { 
                    'Name': { 'regular': 'Tom' }, 
                    'Age': { 'regular': '10' }, 
                    'Single': { 'regular': 'true' }, 
                    'Skill_ids': { 'regular': '[1,2,3]' }
                },
                { 
                    'Name': { 'regular': 'Bob' }, 
                    'Age': { 'regular': '20' }, 
                    'Single': { 'regular': 'true' }, 
                    'Skill_ids': { 'regular': '[2,4,6]' }
                },
                { 
                    'Name': { 'regular': 'Alice' }, 
                    'Age': { 'regular': '30' }, 
                    'Single': { 'regular': 'false' }, 
                    'Skill_ids': { 'regular': '[3,6,9]' }
                }
            ],
            versions: ['regular', 'beta'],
            currentVersion: 'regular',
            selectedCell: null,
            copiedRowData: null,
            copiedData: null
        };

        // DOM elements
        this.attributeRow = null;
        this.dataTypeRow = null;
        this.tableBody = null;
        this.contextMenu = null;
        this.addAttributeModal = null;
        this.addVersionModal = null;
        this.versionSelect = null;
    }

    async init() {
        try {
            await elementLoader.loadElementTree(ElementConfig);
            this.initOnElementLoaded();
            console.log('App initialized successfully.');
        } catch (error) {
            console.error('Error during app initialization:', error);
        }
    }

    // render table
    renderTable() {
        this.renderHeader();
        this.renderBody();
    }

    // render header
    renderHeader() {
        // reset attribute and data type
        this.attributeRow.innerHTML = '<th class="row-header">Attributes</th>';
        this.dataTypeRow.innerHTML = '<th class="row-header">Data type</th>';
        
        // Add attribute and data type as new column
        this.state.attributes.forEach((column, columnIndex) => {
            // attribute name
            const headerCell = document.createElement('th');
            headerCell.className = 'attribute-cell';
            headerCell.textContent = column.name;
            headerCell.dataset.columnId = columnIndex;
            this.attributeRow.appendChild(headerCell);
            
            // data type
            const dataTypeCell = document.createElement('th');
            dataTypeCell.className = 'data-type';
            const typeTag = document.createElement('span');
            typeTag.className = `data-type-tag type-${column.type}`;
            typeTag.textContent = this.getTypeDisplayName(column.type);
            dataTypeCell.appendChild(typeTag);
            this.dataTypeRow.appendChild(dataTypeCell);
        });
    }

    // render table
    renderBody() {
        // reset table
        this.tableBody.innerHTML = '';

        // Add new row
        this.state.rows.forEach((row, rowIndex) => {
            const tableRow = document.createElement('tr');
            
            // Add row id cell
            const rowHeaderCell = document.createElement('td');
            rowHeaderCell.className = 'row-header';
            rowHeaderCell.textContent = rowIndex + 1;
            rowHeaderCell.dataset.rowIndex = rowIndex;
            rowHeaderCell.dataset.isRowHeader = true;
            tableRow.appendChild(rowHeaderCell);
            
            // Add data cell
            this.state.attributes.forEach((column, columnIndex) => {
                const dataCell = document.createElement('td');
                dataCell.dataset.attributeName = column.name;
                dataCell.dataset.rowIndex = rowIndex;
                dataCell.dataset.columnIndex = columnIndex;
                
                const input = document.createElement('input');
                input.type = 'text';
                input.value = this.getDataWithVersion(row[column.name], this.state.currentVersion) || '';
                
                dataCell.appendChild(input);
                tableRow.appendChild(dataCell);
            });
            
            this.tableBody.appendChild(tableRow);
        });
        this.state.selectedCell = null;
    }

    // get attribute data with given version
    getDataWithVersion(attributeData, version) {
        return attributeData[version] || attributeData['regular'];
    }

    // get shown text for given type
    getTypeDisplayName(type) {
        const typeMap = {
            'string': 'string text',
            'int': 'int number',
            'float': 'float number',
            'bool': 'true or false',
            'list-int': 'list of int number'
        };
        return typeMap[type] || type;
    }

    // setup event listeners
    setupEventListeners() {
        // show add attribute popup
        document.getElementById('addAttributeBtn').addEventListener('click', () => {
            this.addAttributeModal.style.display = 'flex';
        });

        // add new row
        document.getElementById('addRowBtn').addEventListener('click', this.addNewRow);

        // insert new row
        document.getElementById('insertRowBtn').addEventListener('click', this.insertNewRow);

        // show add version popup
        document.getElementById('addVersionBtn').addEventListener('click', () => {
            this.addVersionModal.style.display = 'flex';
        });

        // version select
        this.versionSelect.addEventListener('change', (e) => {
            this.state.currentVersion = e.target.value;
            renderTable();
        });
        
        // close modal
        document.querySelectorAll('.close-modal').forEach(button => {
            button.addEventListener('click', () => {
                this.addAttributeModal.style.display = 'none';
                this.addVersionModal.style.display = 'none';
            });
        });
        
        // create attribute btn
        document.getElementById('createAttributeBtn').addEventListener('click', this.createAttribute);
        
        // create version btn
        document.getElementById('createVersionBtn').addEventListener('click', this.createVersion);
        
        // select cell
        this.tableBody.addEventListener('click', (e) => {
            if (e.target.tagName === 'TD' || e.target.tagName === 'INPUT') {
                const cell = e.target.tagName === 'TD' ? e.target : e.target.parentElement;
                selectCell(cell);
            }
        });
        
        // show right click context
        document.addEventListener('contextmenu', (e) => {
            if (e.target.tagName === 'TD' || e.target.tagName === 'TH') {
                e.preventDefault();
                showContextMenu(e);
            }
        });
        
        // hide right click context
        document.addEventListener('click', () => {
            this.contextMenu.style.display = 'none';
        });
        
        // right click context
        document.getElementById('copyCell').addEventListener('click', this.copyCell);
        document.getElementById('pasteCell').addEventListener('click', this.pasteCell);
        document.getElementById('copyRow').addEventListener('click', this.copyRow);
        document.getElementById('pasteRow').addEventListener('click', this.pasteRow);
        document.getElementById('editAttribute').addEventListener('click', this.editAttribute);
    }

    // select cell
    selectCell(cell) {
        // inactive the previous selected
        document.querySelectorAll('td.active').forEach(c => {
            c.classList.remove('active');
        });
        
        // active selected cell
        cell.classList.add('active');
        this.state.selectedCell = cell;
    }

    // show right click context
    showContextMenu(e) {
        this.contextMenu.style.display = 'block';
        this.contextMenu.style.left = `${e.pageX}px`;
        this.contextMenu.style.top = `${e.pageY}px`;

        if (e.target.tagName === 'TH') { // attribute cell
            document.getElementById('copyCell').style.display = 'none';
            document.getElementById('pasteCell').style.display = 'none';
            document.getElementById('copyRow').style.display = 'none';
            document.getElementById('pasteRow').style.display = 'none';
            document.getElementById('editAttribute').style.display = 'block';
        } else {
            if (e.target.dataset.isRowHeader) { // row header cell
                document.getElementById('copyCell').style.display = 'none';
                document.getElementById('pasteCell').style.display = 'none';
                document.getElementById('copyRow').style.display = 'block';
                document.getElementById('pasteRow').style.display = 'block';
                document.getElementById('editAttribute').style.display = 'none';
            }
            else { // common data cell
                document.getElementById('copyCell').style.display = 'block';
                document.getElementById('pasteCell').style.display = 'block';
                document.getElementById('copyRow').style.display = 'none';
                document.getElementById('pasteRow').style.display = 'none';
                document.getElementById('editAttribute').style.display = 'none';
            }
        }
    }

    // copy cell
    copyCell() {
        if (this.state.selectedCell) {
            const input = this.state.selectedCell.querySelector('input');
            this.state.copiedData = input.value;
        }
        else {
            alert('no selected cell');
        }
        this.contextMenu.style.display = 'none';
    }

    // paste cell
    pasteCell() {
        if (this.state.selectedCell && this.state.copiedData !== null) {
            const input = this.state.selectedCell.querySelector('input');
            input.value = this.state.copiedData;
        }
        else if (this.state.copiedData == null) {
            alert('no copied data');
        }
        else {
            alert('no selected cell')
        }
        this.contextMenu.style.display = 'none';
    }

    // copy row
    copyRow() {
        if (this.state.selectedCell && this.state.selectedCell.dataset.isRowHeader) {
            const rowIndex = this.state.selectedCell.dataset.rowIndex;
            const rowData = this.state.rows[rowIndex];
            this.state.copiedRowData = rowData;
        }
        else {
            alert('no selected row cell');
        }
        this.contextMenu.style.display = 'none';
    }

    // paste row
    pasteRow() {
        if (this.state.selectedCell && this.state.selectedCell.dataset.isRowHeader) {
            const rowIndex = this.state.selectedCell.dataset.rowIndex;
            this.state.rows[rowIndex] = this.state.copiedRowData;
            renderTable();
        }
        else {
            alert('no selected row cell');
        }
        this.contextMenu.style.display = 'none';
    }

    // edit attribute
    editAttribute() {
        alert('TODO');
        this.contextMenu.style.display = 'none';
    }

    // add new row at the end
    addNewRow() {
        const newRow = { };
        this.state.attributes.forEach(column => {
            newRow[column.name] = '';
        });
        this.state.rows.push(newRow);
        renderTable();
    }
    
    // insert new row below the selected row
    insertNewRow() {
        if (this.state.selectedCell) {
            const rowIndex = this.state.selectedCell.dataset.rowIndex;
            const newRow = { };
            this.state.attributes.forEach(column => {
                newRow[column.name] = '';
            });
            this.state.rows.splice(rowIndex, 0, newRow);
            renderTable();
        }
        else {
            alert('no selected cell');
        }
    }

    // create attribute
    // - check empty
    // - check duplication
    createAttribute() {
        const attributeNmae = document.getElementById('attributeName').value;
        const dataType = document.getElementById('dataType').value;
        
        if (!attributeNmae) {
            alert('Error: please input column name');
            return;
        }

        const hasDuplicate = false;
        this.state.attributes.forEach(attribute => {
            if (attributeNmae === attribute.name) {
                hasDuplicate = true;
            }
        });
        if (hasDuplicate) {
            alert('Error: has duplicate attribute');
            return;
        }
        
        const newColumnId = 'col' + (this.state.attributes.length + 1);
        this.state.attributes.push({
            name: attributeNmae,
            type: dataType
        });
        this.state.rows.forEach(row => {
            row[attributeNmae] = '';
        });
        this.addAttributeModal.style.display = 'none';
        renderTable();

        document.getElementById('attributeName').value = '';
        document.getElementById('dataType').value = 'string';
    }

    // create version
    // TODO
    createVersion() {
        const versionName = document.getElementById('versionName').value;
        const copyFrom = document.getElementById('copyFrom').value;
        
        if (!versionName) {
            alert('Error: please input version name');
            return;
        }
        
        if (this.state.versions.includes(versionName)) {
            alert('Error: version name duplicates');
            return;
        }
        
        this.state.versions.push(versionName);
        
        // 更新版本选择下拉框
        const option = document.createElement('option');
        option.value = versionName;
        option.textContent = versionName;
        this.versionSelect.appendChild(option);
        
        // 如果选择了从现有版本复制数据
        if (copyFrom !== 'none') {
            this.state.rows.forEach(row => {
                this.state.attributes.forEach(column => {
                    row.data[column.id][versionName] = row.data[column.id][copyFrom] || '';
                });
            });
        } else {
            // 否则初始化空数据
            this.state.rows.forEach(row => {
                this.state.attributes.forEach(column => {
                    row.data[column.id][versionName] = '';
                });
            });
        }
        
        addVersionModal.style.display = 'none';
        
        // 重置表单
        document.getElementById('versionName').value = '';
        document.getElementById('copyFrom').value = 'none';
        
        alert(`版本 "${versionName}" 已创建`);
    }

    initOnElementLoaded() {
        // DOM elements
        this.attributeRow = document.getElementById('attributeRow');
        this.dataTypeRow = document.getElementById('dataTypeRow');
        this.tableBody = document.getElementById('tableBody');
        this.contextMenu = document.getElementById('contextMenu');
        this.addAttributeModal = document.getElementById('addAttributeModal');
        this.addVersionModal = document.getElementById('addVersionModal');
        this.versionSelect = document.getElementById('versionSelect');

        this.renderTable();
        this.setupEventListeners();
    }
}

app = new App();
app.init();
