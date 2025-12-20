
class Attribute {
    constructor(name, content) {
        this.name = name; // str: attribute name
        this.dataType = null; // str: data type of the attribute
        this.load(content);
    }

    /* load from dict */
    load(content) {
        if (!content) return false;

        this.dataType = content['dataType'] ?? null;
        return true;
    }

    /* dump into dict */
    dump() {
        const res = {};
        if (this.dataType) {
            res['dataType'] = this.dataType;
        }
        return res;
    }
}

class Cell {
    constructor(content) {
        this.groups = {}; // dict: group name -> list of versions belong to this group
        this.values = {}; // dict: group name -> value
        this.version2group = {}; // dict: version name -> group name
        this.load(content);
    }

    /* load from dict */
    load(content) {
        if (!content) return false;

        const cellRawData = content['cellRawData'];
        this.version2group = {};
        this.groups = cellRawData['groups'] ?? {};
        for (const [groupName, groupVersions] of Object.entries(this.groups)) {
            for (const groupVersion of groupVersions) {
                this.version2group[groupVersion] = groupName;
            }
        }
        this.values = {};
        const dataType = content['attribute']['dataType'];
        Object.entries(cellRawData['values'] ?? {}).forEach(([groupName, value]) => {
            this.values[groupName] = CellValueFactory.createCellValue(dataType, value);
        });
        return true;
    }

    /* dump into dict */
    dump() {
        const res = {};
        res['groups'] = structuredClone(this.groups);
        res['values'] = structuredClone(this.values);
        return res;
    }

    //#region value

    _getValueInstance(version) {
        const group = this.version2group[version] ?? null;
        if (group == null) {
            return null;
        }
        return this.values[group] ?? null;
    }

    getValue(version) {
        const valueInstance = this._getValueInstance(version);
        if (valueInstance == null) {
            return;
        }
        return valueInstance.value;
    }

    setValue(version, v) {
        const valueInstance = this._getValueInstance(version);
        if (valueInstance == null) {
            return;
        }
        valueInstance.value = v;
    }
    
    setValueByText(version, text) {
        const valueInstance = this._getValueInstance(version);
        if (valueInstance == null) {
            return;
        }
        valueInstance.updateFromText(text);
    }

    parse(version) {
        const valueInstance = this._getValueInstance(version);
        if (valueInstance == null) {
            return 'NAN';
        }
        else {
            return valueInstance.parse();
        }
    }

    //#endregoin value

}

class Row {
    constructor(uuid, versions, content) {
        this.uuid = uuid; // str: row uuid
        this.versions = versions // list: version domain
        this.cells = {}; // dict: attribute name -> cell
        this.load(content);
    }

    /* load from dict */
    load(content) {
        if (!content) return false;

        const rowRawData = content['rowRawData'];
        const cells = rowRawData['cells'] ?? {};
        for (const [attributeName, cellRawData] of Object.entries(cells)) {
            const cellD = {
                'cellRawData': cellRawData,
                'attribute': content['attributes'][attributeName]
            }
            this.cells[attributeName] = new Cell(cellD);    
        }
        return true;
    }

    /* dump into dict */
    dump() {
        const res = {};
        res['cells'] = {};
        Object.entries(this.cells).forEach(([attributeName, cell]) => {
            res['cells'][attributeName] = cell.dump();
        });
        return res;
    }

    //#region cell

    getCell(attributeName) {
        return this.cells[attributeName] ?? null;
    }

    //#endregion cell
}


/* implementation for management of config */
class Config {
    constructor(content=null) {
        this.loaded = false;
        this.attributes = {}; // dict: attribute name -> attribute
        this.attributeOrder = []; // list: ordered list of attribute name
        this.rows = {}; // dict: row uuid -> row
        this.rowOrder = {}; // dict: version -> ordered list of row uuid
        this.load(content);
    }

    /* load data from json
     * Parameters:
     * - content: dict
     * Return:
     * - bool: if load successfullys
     */
    load(content) {
        if (!content) return false;

        // check attribute names are all valid and un-duplicated
        const attributes = content['attributes'] ?? {};
        const attributeOrder = content['attributeOrder'] ?? [];
        if (Object.keys(attributes).length != Object.keys(attributeOrder).length) {
            return false;
        }
        const attributeNameSet = new Set();
        for (const attributeName of attributeOrder) {
            if (attributeName in attributeNameSet) {
                return false
            }
            if (!(attributeName in attributes)) {
                return false
            }
        }

        // check row version info is corrent
        // - rowUUid in rowOrder is valid
        // - concatenation of every group of cell data is same as row order
        // - intersection of every group of cell data is void
        // - every group of cell data has a value
        const row2versions = {}
        const rows = content['rows'] ?? {};
        const rowOrder = content['rowOrder'] ?? {};
        for (const [version, rowUUids] of Object.entries(rowOrder)) {
            for (const rowUUid of rowUUids) {
                // check row uuid in row order is valid
                if (!(rowUUid in rows)) {
                    return false;
                }
                if (!(rowUUid in row2versions)) row2versions[rowUUid] = [];
                row2versions[rowUUid].push(version);
            }
        }
        for (const [rowUUid, rowRawData] of Object.entries(rows)) {
            if (!(rowUUid in row2versions)) {
                continue;
            }
            // check cell raw data
            const rowVersions = row2versions[rowUUid];
            const cells = rowRawData['cells'] ?? {};
            for (const [attributeName, cellRawData] of Object.entries(cells)) {
                if (!(attributeName in attributes)) {
                    return false;
                }
                const groups = cellRawData['groups'] ?? {};
                const values = cellRawData['values'] ?? {};
                const version2group = {};
                for (const [groupName, groupVersions] of Object.entries(groups)) {
                    // every group has a value
                    if (!(groupName in values)) {
                        return false;
                    }
                    for (const groupVersion of groupVersions) {
                        if (!(rowVersions.includes(groupVersion))) {
                            return false;
                        }
                        if (!(groupVersion in version2group)) version2group[groupVersion] = [];
                        version2group[groupVersion].push(groupName);
                    }
                }
                // intersection of group is void
                for (const [version, versionGroups] of Object.entries(version2group)) {
                    if (versionGroups.length != 1) {
                        return false;
                    }
                }
                // concatenation of group is 
                if (Object.keys(version2group).length != Object.keys(rowVersions).length) {
                    return false;
                }
            }
        }

        this.attributes = {};
        this.attributeOrder = attributeOrder;
        Object.entries(attributes).forEach(([attributeName, attributeRawData]) => {
            this.attributes[attributeName] = new Attribute(attributeName, attributeRawData);
        });

        this.rows = {};
        this.rowOrder = rowOrder;
        Object.entries(rows).forEach(([rowUUid, rowRawData]) => {
            const rowD = {
                'rowRawData': rowRawData,
                'attributes': { ...this.attributes }
            }
            this.rows[rowUUid] = new Row(rowUUid, row2versions[rowUUid], rowD);
        });
        this.loaded = true;
        return true;
    }

    /* dump into dict */
    dump() {
        const res = {};
        res['attributes'] = {};
        Object.entries(this.attributes).forEach(([attributeName, attribute]) => {
            res['attributes'][attributeName] = attribute.dump();
        });
        res['attributeOrder'] = structuredClone(this.attributeOrder);
        res['rows'] = {};
        Object.entries(this.rows).forEach(([rowUUid, row]) => {
            res['rows'][rowUUid] = row.dump();
        });
        res['rowOrder'] = structuredClone(this.rowOrder);
        return res;
    }

    isLoaded() {
        return this.loaded;
    }

    //#region attribute

    getAttribute(attributeName) {
        return this.attributes[attributeName] ?? null;
    }

    getAttributeOrder() {
        return this.attributeOrder;
    }

    //#endregion attribute

    //#region row
    
    getRow(rowUUid) {
        return this.rows[rowUUid] ?? null;
    }

    getRowOrder() {
        return this.rowOrder;
    }

    //#endregion row

    //#region cell

    getCell(rowUUid, attributeName) {
        if (!(rowUUid in this.rows)) return null;
        const row = this.rows[rowUUid];
        return row.GetCell(attributeName);
    }

    //#endregion cell
    
}
