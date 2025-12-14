
class Config {
    constructor(content=null) {
        this.configContent = null;
        this.setContent(content);
    }

    validateContent(content) {
        if (content == null) {
            return false;
        }
        return true;
    }

    setContent(content) {
        if (!this.validateContent(content)) {
            return false;
        }
        this.configContent = content;
        return true;
    }

    getAttributes() {
        if (!this.isLoaded()) {
            return null;
        }
        return this.configContent['Attributes'];
    }

    getAttributeOrder() {
        if (!this.isLoaded()) {
            return null;
        }
        return this.configContent['AttributeOrder'];
    }
    
    getRows() {
        if (!this.isLoaded()) {
            return null;
        }
        return this.configContent['Rows'];
    }

    getCell(row, attributeName) {
        return row.Cells[attributeName];
    }

    isLoaded() {
        return this.configContent != null;
    }

    save() {

    }
}