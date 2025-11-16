/* Json loader
 * load json from filesystem
 */
class JsonLoader {
    async loadJsonFile(jsonPath) {
        const response = await fetch(jsonPath);
        if (!response.ok) {
            throw new Error(`Failed to load JSON file from path "${jsonPath}".`);
        }
        const jsonData = await response.json();
        return jsonData;
    }
}

jsonLoader = new JsonLoader();