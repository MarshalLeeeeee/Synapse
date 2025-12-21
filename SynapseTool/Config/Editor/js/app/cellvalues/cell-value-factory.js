
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

    static unparse(dataType, text) {
        switch (dataType) {
            case 'type-int':
                return IntCellValue.unparse(text);
            case 'type-float':
                return FloatCellValue.unparse(text);
            case 'type-string':
                return StringCellValue.unparse(text);
            case 'type-boolean':
                return BooleanCellValue.unparse(text);
            case 'type-list-int':
                return ListIntCellValue.unparse(text);
            default:
                throw new Error(`Unsupported data type: ${dataType}`);
        }
    }
}