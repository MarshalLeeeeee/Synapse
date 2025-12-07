
class Btn extends Element {
    constructor(domElement, callback=null) {
        super(domElement);
        this._initialize(callback);
    }

    setCallback(callback) {
        this.callback = callback;
    }

    _initialize(callback) {
        this.callback = callback;
        console.log('Dom element: ', this.domElement);
        this.domElement.addEventListener('click', () => this._onClick());
    }

    _onClick() {
        if (this.callback) {
            this.callback();
        }
    }
}
