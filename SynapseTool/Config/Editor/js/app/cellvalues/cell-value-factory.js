
class CellValueFactory {
    static createCellValue(dataType, value) {
        switch (dataType) {
            case 'type-int':
                return new IntCellValue(value);
            case 'type-float':
                return new FloatCellValue(value);
            case 'type-string':
                return new StringCellValue(value);
            case 'type-boolean':
                return new BooleanCellValue(value);
            case 'type-list-int':
                return new ListIntCellValue(value);
            default:
                throw new Error(`Unsupported data type: ${dataType}`);
        }
    }

    static getDisplayName(dataType) {
        switch (dataType) {
            case 'type-int':
                return "int number";
            case 'type-float':
                return "float number";
            case 'type-string':
                return "string text";
            case 'type-boolean':
                return "true or false";
            case 'type-list-int':
                return "list of int number";
            default:
                throw new Error(`Unsupported data type: ${dataType}`);
        }
    }
}