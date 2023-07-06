using Playnite.SDK.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using EmudeckPlaynite.Model;
using System.Linq;

namespace EmudeckPlaynite
{
    public class EmudeckConfigurator
    {

        private string EmudeckInstallDir;
        public EmudeckConfigurator(
            // string PlayniteInstallDir,
            string EmudeckInstallDir
        )
        {
            // this.PlayniteInstallDir = PlayniteInstallDir;
            this.EmudeckInstallDir = EmudeckInstallDir;
        }

        public void RemoveAllEmulators()
        {
            var emulators = Playnite.SDK.API.Instance.Database.Emulators.Select(s => s.Id);
            foreach (var emu in emulators)
            {
                Playnite.SDK.API.Instance.Database.Emulators.Remove(emu);
            }
            var scanners = Playnite.SDK.API.Instance.Database.GameScanners.Select(s => s.Id);
            foreach (var emu in scanners)
            {
                Playnite.SDK.API.Instance.Database.GameScanners.Remove(emu);
            }
        }

        private Dictionary<string, EmulatorDefinitionProfile> GetDefaultProfiles (){
            var map = new Dictionary<string, EmulatorDefinitionProfile>();
            Playnite.SDK.API.Instance.Emulation.Emulators.ForEach(s => {
                var defaultProfile = s.Profiles.First();
                if(defaultProfile != null){
                    map.Add(s.Name,s.Profiles.First());
                }
            });
            return map;
        }

        public void AddEmulators()
        {
            
            // Get Platforms
            var platforms = new Dictionary<string, Guid>();

            var savedPlatforms = Playnite.SDK.API.Instance.Database.Platforms.ToList();

            savedPlatforms.ForEach(s =>
            {
                platforms.Add(s.SpecificationId, new Guid(s.Id.ToString()));
            });
            
            

            var emudeckConfigs = GetEmudeckConfigs();
            var emulatorDefaultProfiles = GetDefaultProfiles();
            foreach (var config in emudeckConfigs)
            {
                // Get platform
                Guid platformId;
                platforms.TryGetValue(config.PlatformSpecificiationId, out platformId);
                if (platformId == null)
                    continue;

                
                var defaultProfile = emulatorDefaultProfiles[config.PlayniteEmulatorName];
                // Insert emulator 
                var emulator = new Emulator
                {
                    Name = config.Name,
                    BuiltinProfiles = new ObservableCollection<BuiltInEmulatorProfile>(),
                    InstallDir = $"{this.EmudeckInstallDir}\\tools\\launchers",
                    CustomProfiles = new ObservableCollection<CustomEmulatorProfile>{
                                new CustomEmulatorProfile{
                                    Platforms = new List<Guid>{platformId},
                                    Arguments= "\"{ImagePath}\"" + $" {config.Arguments}",
                                    Executable = $"{this.EmudeckInstallDir}\\{config.Executable}",
                                    TrackingMode =  0,
                                    ImageExtensions = defaultProfile.ImageExtensions,
                                    Name = "Default",
                                    WorkingDirectory = "{EmulatorDir}",
                                }
                            },
                };

                Playnite.SDK.API.Instance.Database.Emulators.Add(emulator);

                Guid recordGuid = emulator.Id;

                var scanners = Playnite.SDK.API.Instance.Database.GameScanners;
                scanners.Add(new GameScannerConfig
                {
                    EmulatorId = recordGuid,
                    Name = config.Name,
                    Directory = $"{EmudeckInstallDir}\\roms\\{config.Name}",
                    InGlobalUpdate = true,
                    ExcludeOnlineFiles = false,
                    UseSimplifiedOnlineFileScan = false,
                    ImportWithRelativePaths = true,
                    ScanSubfolders = true,
                    ScanInsideArchives = true,
                    OverridePlatformId = new Guid(),
                    EmulatorProfileId = emulator.CustomProfiles[0].Id,
                    PlayActionSettings = ScannerConfigPlayActionSettings.ScannerSettings,
                    MergeRelatedFiles = true,
                });
            }
         
            // Playnite.SDK.API.Instance.Dialogs.ShowMessage("All Emulators for Emudeck added into the library.");
        }

        private IEnumerable<EmudeckEmulatorConfig> GetEmudeckConfigs() =>
            // TODO Deserialise from YAML
            new List<EmudeckEmulatorConfig>{
                new EmudeckEmulatorConfig{
                    Name = "psx",
                    Arguments = "-fullscreen",
                    PlayniteEmulatorName = "DuckStation",
                    Executable = "tools\\launchers\\duckstation.bat",
                    PlatformSpecificiationId = "sony_playstation"
                }
            };

    }
}
