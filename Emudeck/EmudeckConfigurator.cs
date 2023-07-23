using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using EmudeckPlaynite.Model;
using System.Linq;
using Playnite.SDK;
using Playnite.SDK.Models;

namespace EmudeckPlaynite
{
    public class EmudeckConfigurator
    {

        private string EmudeckInstallDir;
        private Configuration configuration;
        public EmudeckConfigurator(
            string EmudeckInstallDir,
            Configuration configuration
        )
        {
            this.EmudeckInstallDir = EmudeckInstallDir;
            this.configuration = configuration;
        }

        public void RemoveAllEmulators()
        {
            try
            {
                // Remove only emulators configured by this plugin
                var emulatorMap = new Dictionary<string, Guid>();
                API.Instance.Database.Emulators.ForEach(s =>
                {
                    emulatorMap.Add(s.Name, s.Id);
                });
                foreach (var item in configuration.emulators)
                {
                    if (emulatorMap.ContainsKey(item.Name) && emulatorMap[item.Name] != null)
                        API.Instance.Database.Emulators.Remove(emulatorMap[item.Name]);
                }

                // Remove only scanners configured by this plugin
                var scannerMap = new Dictionary<string, Guid>();
                API.Instance.Database.GameScanners.ForEach(s =>
                {
                    scannerMap.Add(s.Name, s.Id);
                });
                var scanners = API.Instance.Database.GameScanners.Select(s => s.Id);
                foreach (var item in configuration.emulators)
                {
                     if (scannerMap.ContainsKey(item.Name) && scannerMap[item.Name] != null)
                        API.Instance.Database.GameScanners.Remove(scannerMap[item.Name]);
                }
            }
            catch (Exception e)
            {
                API.Instance.Notifications.Add(id: "Emudeck" + Guid.NewGuid(), text: "Unable to remove previous Emudeck configurations" + e.Message, type: NotificationType.Error);
            }

        }

        private Dictionary<string, EmulatorDefinitionProfile> GetDefaultProfiles()
        {
            var map = new Dictionary<string, EmulatorDefinitionProfile>();
            API.Instance.Emulation.Emulators.ForEach(s =>
            {
                var defaultProfile = s.Profiles.First();
                if (defaultProfile != null)
                {
                    map.Add(s.Name, s.Profiles.First());
                }
            });
            return map;
        }

        public void AddEmulators()
        {
            try {
                 // Get Platforms
                var platforms = new Dictionary<string, Guid>();

                var savedPlatforms = API.Instance.Database.Platforms.ToList();

                savedPlatforms.ForEach(s =>
                {
                    platforms.Add(s.SpecificationId, new Guid(s.Id.ToString()));
                });



                var emulatorDefaultProfiles = GetDefaultProfiles();
                foreach (var config in configuration.emulators)
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
                                        ImageExtensions = config.FileExtensions != null ? config.FileExtensions : defaultProfile.ImageExtensions,
                                        Name = "Default",
                                        WorkingDirectory = "{EmulatorDir}",
                                    }
                                },
                    };

                    API.Instance.Database.Emulators.Add(emulator);

                    Guid recordGuid = emulator.Id;

                    var scanners = API.Instance.Database.GameScanners;
                    scanners.Add(new GameScannerConfig
                    {
                        EmulatorId = recordGuid,
                        Name = config.Name,
                        Directory = $"{EmudeckInstallDir}\\{config.RomsDir}",
                        InGlobalUpdate = true,
                        ExcludeOnlineFiles = false,
                        UseSimplifiedOnlineFileScan = false,
                        ImportWithRelativePaths = true,
                        ScanSubfolders = true,
                        ScanInsideArchives = true,
                        OverridePlatformId = platformId,
                        EmulatorProfileId = emulator.CustomProfiles[0].Id,
                        PlayActionSettings = ScannerConfigPlayActionSettings.ScannerSettings,
                        MergeRelatedFiles = true,
                    });
                }

                API.Instance.Notifications.Add(id: "Emudeck" + Guid.NewGuid(), text: "Emudeck configuration successfully loaded", NotificationType.Info);
            } catch (Exception e){
                API.Instance.Dialogs.ShowErrorMessage($"There was an error when attempting to configure your emulators: {e.Message}", "EmuDeck Error");
            }
        }
    }
}
