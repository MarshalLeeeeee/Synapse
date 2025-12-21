
class Attribute {
    constructor(uuid, content) {
        this.uuid = uuid;
        this.name = ''; // str: attribute name
        this.dataType = null; // str: data type of the attribute
        this.load(content);
    }

    /* load from dict */
    load(content) {
        if (!content) return;

        this.name = content['name'] ?? '';
        this.dataType = content['dataType'] ?? null;
    }

    /* dump into dict */
    dump() {
        const res = {};
        res['name'] = this.name;
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
        if (!content) return;

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
    }

    /* dump into dict */
    dump() {
        const res = {};
        res['groups'] = structuredClone(this.groups);
        res['values'] = {};
        Object.entries(this.values).forEach(([groupName, valueInstance]) => {
            res['values'][groupName] = valueInstance.value;
        });
        return res;
    }

    //#region value

    /* get value instance, which is an instance of a cell value type */
    _getValueInstance(version) {
        const group = this.version2group[version] ?? null;
        if (group == null) {
            return null;
        }
        return this.values[group] ?? null;
    }

    /* get real value from value instance, return null if value instance does not exist */
    getValue(version) {
        const valueInstance = this._getValueInstance(version);
        if (valueInstance == null) {
            return null;
        }
        return valueInstance.value;
    }
    
    /* set real value for value instance, skip if value instance does not exist */
    setValue(version, v) {
        const valueInstance = this._getValueInstance(version);
        if (valueInstance == null) {
            return;
        }
        valueInstance.value = v;
    }
    
    /* set real value by string text for value instance, skip if value instance does not exist */
    setValueByText(version, text) {
        const valueInstance = this._getValueInstance(version);
        if (valueInstance == null) {
            return;
        }
        valueInstance.updateFromText(text);
    }
    
    /* parse real value into string text from value instance, return null if value instance does not exist */
    parse(version) {
        const valueInstance = this._getValueInstance(version);
        if (valueInstance == null) {
            return null;
        }
        else {
            return valueInstance.parse();
        }
    }

    /* check if the value can be changed into another data type */
    checkChangeDataType(dataType, rowUUid, attributeUUid) {
        for (const [group, cellInstnace] of Object.entries(this.values)) {
            const parsedText = cellInstnace.parse();
            if (parsedText == null) {
                throw Error(`Setting attribute for row ${rowUUid} attribute ${attributeUUid} fails because the origin value of group ${group} is null.`);
            }
            try {
                const v = CellValueFactory.unparse(dataType, parsedText);
                if (v == null) {
                    throw Error(`Setting attribute for row ${rowUUid} attribute ${attributeUUid} fails because the origin value of group ${group} as ${parsedText} cannot to converted into data type ${dataType}.`);
                }
            }
            catch (error) {
                throw Error(`Setting attribute for row ${rowUUid} attribute ${attributeUUid} fails because the origin value of group ${group} as ${parsedText} cannot to converted into data type ${dataType}.`);
            }
        }
    }

    changeDataType(dataType) {
        const newValues = {}
        for (const [group, cellInstnace] of Object.entries(this.values)) {
            const parsedText = cellInstnace.parse();
            const v = CellValueFactory.unparse(dataType, parsedText);
            newValues[group] = v;
        }
        this.values = {};
        Object.entries(newValues).forEach(([groupName, value]) => {
            this.values[groupName] = CellValueFactory.createCellValue(dataType, value);
        });
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
        if (!content) return;

        const rowRawData = content['rowRawData'];
        const cells = rowRawData['cells'] ?? {};
        for (const [attributeUUid, cellRawData] of Object.entries(cells)) {
            const cellD = {
                'cellRawData': cellRawData,
                'attribute': content['attributes'][attributeUUid]
            }
            this.cells[attributeUUid] = new Cell(cellD);    
        }
    }

    /* dump into dict */
    dump() {
        const res = {};
        res['cells'] = {};
        Object.entries(this.cells).forEach(([attributeUUid, cell]) => {
            res['cells'][attributeUUid] = cell.dump();
        });
        return res;
    }

    //#region cell

    /* get cell instance, return null if not exists */
    getCell(attributeUUid) {
        return this.cells[attributeUUid] ?? null;
    }

    //#endregion cell
}


/* implementation for management of config */
class Config {
    constructor() {
        this.loaded = false;
        this.attributes = {}; // dict: attribute name -> attribute
        this.attributeOrder = []; // list: ordered list of attribute name
        this.rows = {}; // dict: row uuid -> row
        this.rowOrder = {}; // dict: version -> ordered list of row uuid
    }

    /* load data from json
     * Parameters:
     * - content: dict
     * Return:
     * - bool: if load successfullys
     */
    load(content) {
        if (!content) return;

        // check attribute names are all valid and un-duplicated
        const attributes = content['attributes'] ?? {};
        const attributeOrder = content['attributeOrder'] ?? [];
        if (Object.keys(attributes).length != Object.keys(attributeOrder).length) {
            throw Error(`Attribute order does not match all attributes`);
        }
        const attributeUUidSet = new Set();
        for (const attributeUUid of attributeOrder) {
            if (attributeUUid in attributeUUidSet) {
                throw Error(`Attribute uuid ${attributeUUid} duplicates in attribute order`);
            }
            if (!(attributeUUid in attributes)) {
                throw Error(`Attribute uuid ${attributeUUid} is not in attributes`);
            }
            attributeUUidSet.add(attributeUUid);
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
                    throw Error(`Row uuid ${rowUUid} is not in rows`);
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
            for (const [attributeUUid, cellRawData] of Object.entries(cells)) {
                if (!(attributeUUid in attributes)) {
                    throw Error(`Attribute uuid ${attributeUUid} of row ${rowUUid} is not in attributes`);
                }
                const groups = cellRawData['groups'] ?? {};
                const values = cellRawData['values'] ?? {};
                const version2group = {};
                for (const [groupName, groupVersions] of Object.entries(groups)) {
                    // every group has a value
                    if (!(groupName in values)) {
                        throw Error(`Group ${groupName} of attribute uuid ${attributeUUid} of row ${rowUUid} does not have a value`);
                    }
                    for (const groupVersion of groupVersions) {
                        if (!(rowVersions.includes(groupVersion))) {
                            throw Error(`Version ${groupVersion} in group ${groupName} of attribute uuid ${attributeUUid} of row ${rowUUid} is not a display version`);
                        }
                        if (!(groupVersion in version2group)) version2group[groupVersion] = [];
                        version2group[groupVersion].push(groupName);
                    }
                }
                // intersection of group is void
                for (const [version, versionGroups] of Object.entries(version2group)) {
                    if (versionGroups.length != 1) {
                        throw Error(`Version ${version} of attribute uuid ${attributeUUid} of row ${rowUUid} belongs to multiple groups.`);
                    }
                }
                // concatenation of group is not set of all displayed version
                if (Object.keys(version2group).length != Object.keys(rowVersions).length) {
                    throw Error(`All group versions of attribute uuid ${attributeUUid} of row ${rowUUid} is not set of all displayed version.`);
                }
            }
        }

        this.attributes = {};
        this.attributeOrder = attributeOrder;
        Object.entries(attributes).forEach(([attributeUUid, attributeRawData]) => {
            this.attributes[attributeUUid] = new Attribute(attributeUUid, attributeRawData);
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
        Object.entries(this.attributes).forEach(([attributeUUid, attribute]) => {
            res['attributes'][attributeUUid] = attribute.dump();
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

    getAttribute(attributeUUid) {
        return this.attributes[attributeUUid] ?? null;
    }

    getAttributeOrder() {
        return this.attributeOrder;
    }

    setAttribute(attributeUUid, attributeData) {
        // pre check
        const attribute = this.getAttribute(attributeUUid);
        if (attribute == null) {
            throw Error(`Setting attribute for uuid ${attributeUUid} fails because attribute is not found.`);
        }
        const name = attributeData['name'];
        const dataType = attributeData['dataType'];
        if (attribute.dataType != dataType) {
            for (const [rowUUid, row] of Object.entries(this.rows)) {
                const cell = row.getCell(attributeUUid);
                if (cell == null) {
                    throw Error(`Setting attribute for row ${rowUUid} attribute ${attributeUUid} fails because the cell does not exist.`);
                }
                cell.checkChangeDataType(dataType, rowUUid, attributeUUid);
            }
        }
        // set
        attribute.name = name;
        if (attribute.dataType != dataType) {
            for (const [rowUUid, row] of Object.entries(this.rows)) {
                const cell = row.getCell(attributeUUid);
                cell.changeDataType(dataType);
            }
            attribute.dataType = dataType;
        }

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
}
