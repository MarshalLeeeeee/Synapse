
const VERSION_CONST = 'const/versions.json';

/* load and save global consts */
class Consts {
    constructor() {
        this.versions = []; // list of all version names
    }

    /* load necessary const locally */
    async load() {
        this.versions = await jsonLoader.loadJsonFile(VERSION_CONST);
    }
}

consts = new Consts();
