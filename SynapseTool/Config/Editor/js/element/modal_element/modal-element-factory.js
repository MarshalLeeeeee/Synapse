
class ModalElementFactory {
    static createElement(domElement, config) {
        const elementType = config['type'];
        switch (elementType) {
            case 'text-input':
                return new TextInputModalElement(
                    domElement,
                    config['title'] || '',
                    config['placeholder'] || '',
                    config['description'] || '',
                    config['onChange'] || null
                );
            default:
                console.log(`Unknown modal element type: ${elementType}`);
                return null;
        }
    }
}
