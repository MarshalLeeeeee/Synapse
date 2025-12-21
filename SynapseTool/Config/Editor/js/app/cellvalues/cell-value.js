
class CellValue {
    constructor(value=null) {
        this.value = value;
    }

    /*
    convert value to text
    Returns:
        string representation of the value
    */ 
    parse() {
        return null;
    }

    /*
    convert text to value
    Parameters:
        text - string to parse
    Returns:
        string | null
    */
    static unparse(text) {
        return null;
    }

    _unparse(text) {
        return CellValue.unparse(text);
    }

    /*
    update value from text
    Parameters:
        text - string to parse
    */
    updateFromText(text) {
        const res = this._unparse(text);
        if (res == null) {
            return false;
        }
        this.value = res;
        return true;
    }
}