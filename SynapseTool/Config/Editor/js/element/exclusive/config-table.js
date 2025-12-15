
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
        const attributes = config.getAttributes();
        const attributeOrder = config.getAttributeOrder();
        const rows = config.getRows();

        // Add Attribute header
        const attributeHeader = document.createElement('th');
        attributeHeader.className = 'config-attribute-header config-cell-selectable';
        attributeHeader.textContent = 'Attribute';
        attributeHeader.dataset.attribute = '';
        attributeHeader.addEventListener('click', () => this._onSelectTh(attributeHeader));
        this.configTableHeader.appendChild(attributeHeader);
        
        // Add attribute columns
        attributeOrder.forEach(attrName => {
            const attr = attributes[attrName];
            const th = document.createElement('th');
            th.className = 'config-cell-selectable';
            th.dataset.attribute = attrName;
            th.innerHTML = ` ${attrName} <span class="data-type-tag ${attr.dataType}"> ${attr.dataType} </span>`;
            th.addEventListener('click', () => this._onSelectTh(th));
            this.configTableHeader.appendChild(th);
        });
        
        // Create data rows
        rows.forEach((row, rowIndex) => {
            const tr = document.createElement('tr');
            tr.dataset.rowIndex = rowIndex;
            
            // Add row header (Row X)
            const rowHeader = document.createElement('td');
            rowHeader.className = 'config-attribute-cell config-cell-selectable';
            rowHeader.textContent = `Row ${rowIndex + 1}`;
            rowHeader.dataset.rowIndex = rowIndex;
            rowHeader.dataset.attribute = '';
            rowHeader.addEventListener('click', () => this._onSelectTd(rowHeader));
            tr.appendChild(rowHeader);
            
            // Add cell values for each attribute
            attributeOrder.forEach(attrName => {
                const cell = config.getCell(row, attrName);
                const td = document.createElement('td');
                td.className = 'config-cell-selectable';
                td.dataset.rowIndex = rowIndex;
                td.dataset.attribute = attrName;
                
                // Determine which value to show based on current version
                let displayValue = '';
                let groupForVersion = null;
                
                // Find which group contains the current version
                for (const [group, versions] of Object.entries(cell.groups)) {
                    if (versions.includes(version)) {
                        groupForVersion = group;
                        break;
                    }
                }
                
                // Get the value for that group
                if (groupForVersion && cell.values[groupForVersion] !== undefined) {
                    displayValue = this._formatValue(cell.values[groupForVersion]);
                } else if (cell.values.default !== undefined) {
                    // Fallback to default if version not found
                    displayValue = this._formatValue(cell.values.default);
                }
                
                td.textContent = displayValue;
                td.addEventListener('click', () => this._onSelectTd(td));
                tr.appendChild(td);
            });
            
            this.configTableBody.appendChild(tr);
        });
    }

    _formatValue(value) {
        if (Array.isArray(value)) {
            return `[${value.join(', ')}]`;
        }
        if (typeof value === 'boolean') {
            return value ? 'true' : 'false';
        }
        return String(value);
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