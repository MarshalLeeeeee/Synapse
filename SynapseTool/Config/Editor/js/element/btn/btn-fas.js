
class BtnFas extends Btn {
    constructor(domElement, text='', faIcon='', callback=null) {
        super(domElement, text, callback);
        this._renderIcon(faIcon);
    }

    setIcon(faIcon) {
        this._renderIcon(faIcon);
    }

    _renderIcon(faIcon) {
        this.domElement.querySelector('i').className = `fas ${faIcon}`;
    }
}
