
class ConfigTable extends Element {
    constructor(domElement, title='', options=null, onSave=null, onSelectVersion=null, onSelectTh=null, onSelectTd=null) {
        super(domElement);
        this.onSave = onSave;
        this.onSelectVersion = onSelectVersion;
        this.onSelectTh = onSelectTh;
        this.onSelectTd = onSelectTd;
        this._render(title);
        this._initialize(options);
    }

    setTitle(title) {
        this._render(title);
    }

    setOptions(options) {
        this.versionSelector.setOptions(options);
    }

    setButtons(configs) {
        this.buttonContainer.setChildren(
            configs,
            () => this._onSetButtons(configs)
        );
    }

    _onSetButtons(configs) {
        this.buttons = {};
        configs.forEach(config => {
            const buttonId = config['id'];
            if (!buttonId) {
                return;
            }
            const buttonElement = document.getElementById(buttonId);
            if (buttonElement) {
                this.buttons[buttonId] = new Btn(
                    buttonElement,
                    config['text'],
                    config['callback']
                );
            }
        });
    }

    _initialize(options) {
        this.btnSave = new BtnFas(
            document.getElementById('configSaveBtn'),
            'Save Config',
            'fa-save',
            () => this._onBtnSaveClick()
        );
        this.versionSelector = new Selector(
            document.getElementById('versionSelector'),
            options,
            (version) => this._onSelectVersion(version)
        );
        this.buttonContainer = new Container(
            document.getElementById('configDynamicToolbar')
        );
        this.configTableHeader = document.getElementById('configTableHeader');
        this.configTableBody = document.getElementById('configTableBody');
    }

    _render(title) {
        document.getElementById('configFileName').innerText = title;
    }

    renderTable(config) {
        // Clear table
        this.configTableHeader.innerHTML = '';
        this.configTableBody.innerHTML = '';

        // Load from config datas
        if (!config.isLoaded()) {
            return;
        }
        const version = this.versionSelector.getOption();
        const attributeOrder = config.getAttributeOrder();
        const rowOrder = config.getRowOrder();

        // Add Attribute header
        const attributeHeader = document.createElement('th');
        attributeHeader.className = 'config-attribute-header config-cell-selectable';
        attributeHeader.textContent = 'Attribute';
        attributeHeader.dataset.attributeName = '';
        attributeHeader.addEventListener('click', () => this._onSelectTh(attributeHeader));
        this.configTableHeader.appendChild(attributeHeader);
        
        // Add attribute columns
        attributeOrder.forEach(attributeName => {
            const attribute = config.getAttribute(attributeName);
            const th = document.createElement('th');
            if (attribute != null) {
                th.className = 'config-cell-selectable';
                const attributeDataType = attribute.dataType;
                th.dataset.attributeName = attributeName;
                th.innerHTML = ` ${attributeName} <span class="data-type-tag ${attributeDataType}"> ${attributeDataType} </span>`;
                th.addEventListener('click', () => this._onSelectTh(th));
            }
            else {
                th.className = 'config-cell-selectable';
                th.dataset.attributeName = '';
                th.innerHTML = 'NAN';
            }
            this.configTableHeader.appendChild(th);
        });
        
        // Create data rows
        (rowOrder[version] ?? []).forEach((rowUUid, rowIndex) => {
            const row = config.getRow(rowUUid);
            if (row == null) return;

            const tr = document.createElement('tr');
            tr.dataset.rowIndex = rowIndex;
            
            // Add row header (Row X)
            const rowHeader = document.createElement('td');
            rowHeader.className = 'config-attribute-cell config-cell-selectable';
            rowHeader.textContent = `Row ${rowIndex + 1}`;
            rowHeader.dataset.rowIndex = rowIndex;
            rowHeader.dataset.attributeName = '';
            rowHeader.addEventListener('click', () => this._onSelectTd(rowHeader));
            tr.appendChild(rowHeader);
            
            // Add cell values for each attribute
            attributeOrder.forEach(attributeName => {
                const cell = row.getCell(attributeName);
                const td = document.createElement('td');
                td.className = 'config-cell-selectable';
                td.dataset.rowIndex = rowIndex;
                td.dataset.attributeName = attributeName;
                if (cell != null) {
                    td.textContent = cell.parse(version);
                    td.addEventListener('click', () => this._onSelectTd(td));
                }
                else {
                    td.textContent = 'NAN';
                }
                tr.appendChild(td);
            });
            
            this.configTableBody.appendChild(tr);
        });
    }

    _onSelectTh(th) {
        if (this.onSelectTh) {
            this.onSelectTh(th)
        }
    }

    _onSelectTd(td) {
        if (this.onSelectTd) {
            this.onSelectTd(td)
        }
    }

    _onBtnSaveClick() {
        if (this.onSave) {
            this.onSave();
        }
    }

    _onSelectVersion(version) {
        if (this.onSelectVersion) {
            this.onSelectVersion(version);
        }
    }
}