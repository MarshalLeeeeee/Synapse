
class ListIntCellValue extends CellValue {
    constructor(value=null) {
        super(value);
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
    static unparse(text) {
        if (!text.trim()) return null;
        const res = []
        const textSplit = text.split(',');
        for (const textSeg of textSplit) {
            const item = parseInt(textSeg);
            if (isNaN(item)) {
                return null;
            }
            res.push(item);
        }
        return res;
    }

    _unparse(text) {
        return ListIntCellValue.unparse(text);
    }
}