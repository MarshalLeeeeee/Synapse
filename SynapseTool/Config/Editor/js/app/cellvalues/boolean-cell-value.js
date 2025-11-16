
class BooleanCellValue {
    constructor(value=null) {
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
    unparse(text) {
        switch (text) {
            case 'true':
                return true;
            case 'false':
                return false;
            default:
                return null;
        }
    }

    /*
    update value from text
    Parameters:
        text - string to parse
    */
    update(text) {
        const res = this.unparse(text);
        if (res != null) {
            this.value = res;
        }
    }
}