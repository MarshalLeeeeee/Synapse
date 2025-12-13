
class Element {
    constructor(domElement) {
        this.domElement = domElement;
        this.rawStyleDisplay = this.domElement.style.display;
    }

    setVisible(visible) {
        if (visible) {
            this.domElement.style.display = this.rawStyleDisplay;
        }
        else {
            this.domElement.style.display = 'none';
        }
    }
}