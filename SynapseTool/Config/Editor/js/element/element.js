
class Element {
    constructor(domElement) {
        this.domElement = domElement;
    }

    setVisible(visible) {
        if (visible) {
            this.domElement.style.display = 'flex';
        }
        else {
            this.domElement.style.display = 'none';
        }
    }
}