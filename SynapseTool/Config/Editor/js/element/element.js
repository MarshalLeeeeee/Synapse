
class Element {
    constructor(domElement) {
        this.domElement = domElement;
        this.rawStyleDisplay = this.domElement.style.display;
    }

    _getStyleDisplay() {
        return this.rawStyleDisplay;
    }

    setVisible(visible) {
        if (visible) {
            console.log('Setting element visible.', this.rawStyleDisplay);
            this.domElement.style.display = this._getStyleDisplay();
        }
        else {
            this.domElement.style.display = 'none';
        }
    }
}