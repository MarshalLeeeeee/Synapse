
const VERSION_CONFIG = 'const/versions.json';

class Config {
    constructor() {
        this.versions = []; // list of all version names
    }

    async load() {
        this.versions = await jsonLoader.loadJsonFile(VERSION_CONFIG);
    }
}

config = new Config();