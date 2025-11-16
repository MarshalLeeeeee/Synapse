
class StringCellValue {
    constructor(value=null) {
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
    unparse(text) {
        return text;
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