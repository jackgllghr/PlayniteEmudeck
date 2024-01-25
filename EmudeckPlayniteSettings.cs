using Playnite.SDK;
using Playnite.SDK.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace EmudeckPlaynite
{
    public class EmudeckPlayniteSettings : ObservableObject
    {
        private string pluginPreviouslyInstalledVersion = "";
        public string PluginPreviouslyInstalledVersion { get => pluginPreviouslyInstalledVersion; set => SetValue(ref pluginPreviouslyInstalledVersion, value); }

        private string emudeckInstallDir = "";
        public string EmudeckInstallDir { get => emudeckInstallDir; set => SetValue(ref emudeckInstallDir, value); }

        private List<string> disabledEmulators = new List<string>();
        public List<string> DisabledEmulators { get => disabledEmulators; set => SetValue(ref disabledEmulators, value); }


         public ObservableCollection<CheckboxListItem> EnabledEmulators = new ObservableCollection<CheckboxListItem>();

        public int EnabledEmulatorsCount
        {
            get => EnabledEmulators.Count;
        }
    }

    public class CheckboxListItem
    {
        private string name;
        private bool isChecked;

        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                // OnPropertyChanged();
            }
        }
        public string Name
        {
            get => name;
            set
            {
                name = value;
                // OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            var symbol = IsChecked ? "✔" : "❌";
            return $"{symbol} {Name}";
        }
    }

    public class EmudeckPlayniteSettingsViewModel : ObservableObject, ISettings
    {
        private readonly EmudeckPlaynite plugin;
        private EmudeckPlayniteSettings editingClone { get; set; }

        private EmudeckPlayniteSettings settings;
        public EmudeckPlayniteSettings Settings
        {
            get => settings;
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }

       


        public EmudeckPlayniteSettingsViewModel(EmudeckPlaynite plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // EnabledEmulators = new ObservableCollection<CheckboxListItem>();
            // EnabledEmulators.Clear();


            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<EmudeckPlayniteSettings>();

            // LoadPluginSettings returns null if no saved data is available.
            if (savedSettings != null)
            {
                Settings = savedSettings;

              
                // API.Instance.Dialogs.ShowErrorMessage($"Emualtors checked: {emulators.FindAll(e => e.IsChecked).Count}", "EmuDeck Count");

            }
            else
            {
                Settings = new EmudeckPlayniteSettings();
            }

            
            EmulatorConfigurationDefinitionService.GetConfiguration().emulators.ForEach(emu =>
            {
                Settings.EnabledEmulators.Add(new CheckboxListItem
                {
                    IsChecked = true,
                    Name = emu.Name
                });
            });
              Settings.EnabledEmulators.ForEach(e =>
                {
                    if (savedSettings.DisabledEmulators.Find(i => i == e.Name) != null)
                    {
                        e.IsChecked = false;
                    }
                });



        }

        public RelayCommand<object> BrowseCommand
        {
            get => new RelayCommand<object>((a) =>
            {
                var filePath = Playnite.SDK.API.Instance.Dialogs.SelectFolder();
                settings.EmudeckInstallDir = filePath;
            });
        }
        public RelayCommand<object> ReloadCommand
        {
            get => new RelayCommand<object>((a) =>
            {
                settings.EmudeckInstallDir = $"{settings.EmudeckInstallDir}";
            });
        }


        public void BeginEdit()
        {
            // Code executed when settings view is opened and user starts editing values.
            editingClone = Serialization.GetClone(Settings);
        }

        public void CancelEdit()
        {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            // This method should revert any changes made to Option1 and Option2.
            Settings = editingClone;
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.
            plugin.SavePluginSettings(Settings);
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            return true;
        }
    }
}