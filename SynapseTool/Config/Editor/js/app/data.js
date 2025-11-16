
class AttributeData {
    constructor(name, dataType, isKey=false) {
        this.name = name; // attribute name
        this.dataType = dataType; // data type name
        this.isKey = isKey; // is key attribute (for data exporter)
    }
}

class CellData {
    /*
    Initialize groups and values with a default group
    Parameters:
    - value: any, raw value, not CellValue
    */
    constructor(dataType, value=null) {
        this.dataType = dataType;
        this.version2group = {};
        this.groups = { 'default': config.versions };
        this.values = { 'default': this._getCellValue(value) };
        this._updateVersion2Group();
    }

    /*
    Update groups and values
    Parameters:
    - groups: group name -> list of version name
    - values: group name -> value
    */
    setData(groups, values) {
        if (!this._checkGroupsAndValues(groups, values)) {
            throw new Error('Invalid groups or values');
        }
        this.groups = groups;
        this.values = this._getCellValues(values);
        this._updateVersion2Group();
    }

    /*
    Set value for a specific group
    Parameters:
    - groupName: string
    - text: string
    */
    updateGroupValueByText(groupName, text) {
        if (!(groupName in this.groups)) {
            return;
        }
        this.values[groupName].update(text);
    }

    /*
    Set value for a specific version
    Parameters:
    - version: string
    - text: string
    */
    updateVersionValueByText(version, text) {
        if (!(version in this.version2group)) {
            return;
        }
        this.updateGroupValueByText(this.version2group[version], text);
    }

    /*
    Get cell value by given version
    Paramters:
    - version: str, name of the version
    Returns:
    - CellValue | null
    */
    getCellValueByVersion(version) {
        if (!(version in this.version2group)) {
            return null;
        }
        const cellValue = this.values[this.version2group[version]];
        return cellValue;
    }

    /*
    Check if groups and values are valid
    - no version exists in multiple groups
    - every group has a value
    Parameters:
    - newGroups: group name -> list of version name
    - newValues: group name -> value
    Returns:
    - boolean
    */
    _checkGroupsAndValues(groups, values) {
        const version2Group = {};
        for (const [groupName, versionList] of Object.entries(groups)) {
            for (const version of versionList) {
                if (version in version2Group) {
                    return false;
                }
                version2Group[version] = groupName;
            }
        }
        const groupKeys = Object.keys(groups);
        const valueKeys = Object.keys(values);
        if (groupKeys.size !== valueKeys.size ||
            ![...groupKeys].every(v => valueKeys.has(v)) ||
            ![...valueKeys].every(v => groupKeys.has(v))) {
            return false;
        }
        return true;
    }

    /*
    Turn raw value to cell value
    Parameter:
    - value: any
    Returns:
    - cell value: CellValue
    */
    _getCellValue(value) {
        return CellValueFactory.createCellValue(this.dataType, value)
    }

    /*
    Turn raw values to cell values
    Paramters:
    - values: group name -> raw type
    Returns:
    - values: group name -> CellValue
    */
    _getCellValues(values) {
        const res = {};
        for (const [groupName, value] of Object.entries(values)) {
            res[groupName] = this._getCellValue(value);
        }
        return res;
    }

    /* consrtuct mapping from version to group name */
    _updateVersion2Group() {
        const res = {};
        for (const [groupName, versions] of Object.entries(this.groups)) {
            for (const version of versions) {
                res[version] = groupName;
            }
        }
        this.version2group = res;
    }
}

class RowData {
    constructor(attributes) {
        this.cellValues = {}; // attribute name -> CellData
        for (const attributeData of attributes) {
            this.updateAttributeValue(attributeData);
        }
    }

    updateAttributeValue(attributeData, value=null) {
        const attributeName = attributeData.name;
        const attributeDataType = attributeData.dataType;
        this.cellValues[attributeName] = new CellData(attributeDataType, value);
    }

    copyFromCellValues(other) {
        this.cellValues = other.cellValues;
    }
}