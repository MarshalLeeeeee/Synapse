
class IntCellValue {
    constructor(value=null) {
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
    unparse(text) {
        const res = parseInt(text);
        if (isNaN(res)) {
            return null;
        }
        return res;
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