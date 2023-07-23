const fs = require('fs');
const YAML = require('yaml');
const Fuse = require('fuse.js');

/**
 * Find the best match for platform spec
 * @param {string} name 
 * @param {{ name: string, slug: string}[]} platforms 
 * @returns 
 */
function getPlatformSpec(name, platforms){ 
    const ignoreList = ["Sharp X68000", "DooM"];
    if(ignoreList.indexOf(name) > -1){
        return null;
    }

    const fuse = new Fuse(platforms, {	
        isCaseSensitive: false,
        includeScore: true,
        shouldSort: true,
        threshold: 0.5,
        ignoreLocation: false,
        keys: [ 'slug','name', 'aliases']
      });
    let results = fuse.search(name);
    if(results.length > 0){
        // console.log("Searched for " + name + " and got: " + JSON.stringify(results[0].item.slug)); 
        // console.log(" with a score " + results[0].score);
        return results[0].item;
    }
    return null;
}

try {
    const data = fs.readFileSync('userConfigurations.json', 'utf8');
    var config = JSON.parse(data);
    const platforms = [];

    const playnitePlatforms = fs.readFileSync('playnite-platforms.yml', 'utf8');
    const playnitePlatformsMap = YAML.parse(playnitePlatforms);

    config.forEach(i => {
        if(i.executableArgs.includes("retroarch")){
            var name = i.configTitle;
            var platformSlug = i.romDirectory.substring(17);

            // let regex = new RegExp('(\/k start \/min "Emu" "C:\\Windows\\System32\\cmd.exe" \/K ""C:\\Emulation\\tools\\launchers\\retroarch.bat" "-L" "\${os:win\|cores\|\${os:mac\|\${racores}\|\${os:linux\|\${racores}}}}\${\/})(\w+\.)')
            let regex = /(\$\{\/\})(\w+\.)/g
            const coreTemp = i.executableArgs.match(regex)[0].substring(4);
            const core = coreTemp.substring(0, coreTemp.length -1);

            var fileExtsTemp = i.parserInputs.glob.substring(13);
            var fileExtsTemp2 = fileExtsTemp.substring(0, fileExtsTemp.length -1);
            let exts = fileExtsTemp2.split("|").map(ext => ext.substring(1));
            

            let platformName = i.steamCategory.substring(0, i.steamCategory.length -1).substring(2);
            let platformSpec = getPlatformSpec(platformName, playnitePlatformsMap.platforms);

            if(!!platformSpec){
                platforms.push({
                    name: name,
                    romsDir: "roms\\" + platformSlug,
                    arguments: "-L " + core + ".dll",
                    fileExtensions: exts,
                    playniteEmulatorName: "RetroArch",
                    executable: "tools\\launchers\\retroarch.bat",
                    platformSpecificiationId: platformSpec.slug
                });
            }
           
        }
       
    });
    const doc = new YAML.Document();
    doc.contents = { emulators: platforms};
    console.log(doc.toString());
  } catch (err) {
    console.error(err);
  }