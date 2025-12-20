
class BooleanCellValue extends CellValue {
    constructor(value=null) {
        super(value);
        if (value != null) {
            this.value = value;
        }
        else {
            this.value = false;
        }
    }

    /*
    convert value to text
    Returns:
        string representation of the value
    */ 
    parse() {
        return this.value ? 'true' : 'false';
    }

    /*
    convert text to value
    Parameters:
        text - string to parse
    Returns:
        boolean | null
    */
    _unparse(text) {
        switch (text) {
            case 'true':
                return true;
            case 'false':
                return false;
            default:
                return null;
        }
    }
}