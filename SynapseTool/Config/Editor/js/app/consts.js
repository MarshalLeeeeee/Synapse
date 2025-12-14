
const VERSION_CONST = 'const/versions.json';

class Consts {
    constructor() {
        this.versions = []; // list of all version names
    }

    async load() {
        this.versions = await jsonLoader.loadJsonFile(VERSION_CONST);
    }
}

consts = new Consts();