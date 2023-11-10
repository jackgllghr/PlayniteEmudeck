# Playnite Emudeck

A plugin for Playnite to automatically configure Playnite to use Emudeck via Playnites built-in emulator launching and library scanning tools.

## Usage
Install from Playnite Add-on Browser or by visiting [this link](https://playnite.link/addons.html#EmudeckPlaynite_82cf60ec-8091-488d-9c85-63836ebee151).

On first boot, you will be prompted to set your Emudeck installation path, select the folder and all will be configured for you.

### Manual
Open `Add-ons > Extension Settings > Generic > Emudeck` and point the Add-on to your Emudeck installation directory. You will receive a notification notifying that your Emulator configuration was updated. 

Then, simply hit `Update Game Library > Update Emulated Folders > Update All` and Playnite will begin scanning the Emudeck ROMs folders for your ROMs. 


## Developing
Clone the repository and set up your environment in VSCode as per the [Playnite Docs](https://api.playnite.link/docs/tutorials/extensions/intro.html)

In VSCode, `Run Task > Publish` to build the extension, and enable it in `Settings > For developers` in Playnite. 

Run `./scripts/pack.bat` to package the add-on for distribution. 

### Emulator Defintions
The definitions for the emulators are stored in `Resources/emulators.yml`, which is read by the Add-on at runtime to configure Playnite's emulator functions. 
The majority of these definitions are generated by the NodeJS script in `scripts/SteamRomManagerUserConfigParser` which parses the `userConfiguration.json` for Steam Rom Manager provided by Emudeck. 

To run this:
```
cd scripts/SteamRomManagerUserConfigParser/
npm i
node index.js
```
and copy the output to `Resources/emulators.yml`.

Ultimately, the entire `emulators.yml` file will be generated via this method.

## Roadmap
The plan for this add-on is simply to match the functionality provided by Steam Rom Manager through Emudeck, and to maintain compatibility. 

- [x] Support for popular emulators (PS/PS2/PSP/GC/Wii/WiiU/Switch/NDS/3DS)
- [x] Support for Retroarch emulators
- [ ] Support for PS3 (RPCS3)
- [ ] Ability to switch on/off which emulators to configure, same as SRM
- [x] Automatic update of emulator configuration on Add-on update
- [ ] Improved controller support out-of-the-box