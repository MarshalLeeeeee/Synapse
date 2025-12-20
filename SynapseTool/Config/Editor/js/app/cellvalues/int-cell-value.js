
class IntCellValue extends CellValue {
    constructor(value=null) {
        super(value);
        if (value != null) {
            this.value = value;
        }
        else {
            this.value = 0.0;
        }
    }

    /*
    convert value to text
    Returns:
        string representation of the value
    */ 
    parse() {
        return this.value.toString();
    }

    /*
    convert text to value
    Parameters:
        text - string to parse
    Returns:
        int | null
    */
    _unparse(text) {
        const res = parseInt(text);
        if (isNaN(res)) {
            return null;
        }
        return res;
    }
}