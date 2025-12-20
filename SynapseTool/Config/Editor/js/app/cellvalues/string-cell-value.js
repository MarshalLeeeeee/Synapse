
class StringCellValue extends CellValue {
    constructor(value=null) {
        super(value);
        if (value != null) {
            this.value = value;
        }
        else {
            this.value = '';
        }
    }

    /*
    convert value to text
    Returns:
        string representation of the value
    */ 
    parse() {
        return this.value;
    }

    /*
    convert text to value
    Parameters:
        text - string to parse
    Returns:
        string | null
    */
    _unparse(text) {
        return text;
    }
}