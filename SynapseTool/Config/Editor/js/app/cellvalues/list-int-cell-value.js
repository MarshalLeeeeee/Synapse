
class ListIntCellValue {
    constructor(value=null) {
        if (value != null) {
            this.value = value;
        }
        else {
            this.value = [];
        }
    }

    /*
    convert value to text
    Returns:
        string representation of the value
    */ 
    parse() {
        let res = '';
        for (const v of this.value) {
            res += v.toString() + ',';
        }
        res = res.slice(0, -1); // remove last comma
        return res;
    }

    /*
    convert text to value
    Parameters:
        text - string to parse
    Returns:
        List[int] | null
    */
    unparse(text) {
        if (!text.trim()) return [];
        const res = []
        const textSplit = text.split(',');
        for (const textSeg of textSplit) {
            const item = parseInt(textSeg);
            if (isNaN(item)) {
                return null;
            }
            res.append(item);
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