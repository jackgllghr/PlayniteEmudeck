using Playnite.SDK;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmudeckPlaynite
{
    public class EmudeckPlayniteSettings : ObservableObject
    {
        private string emudeckInstallDir = "D:\\Emulation";
        public string EmudeckInstallDir { get => emudeckInstallDir; set => SetValue(ref emudeckInstallDir, value); }
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

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<EmudeckPlayniteSettings>();

            // LoadPluginSettings returns null if no saved data is available.
            if (savedSettings != null)
            {
                Settings = savedSettings;
            }
            else
            {
                Settings = new EmudeckPlayniteSettings();
            }

            
        }

         public RelayCommand<object> BrowseCommand
        {
            get => new RelayCommand<object>((a) =>
            {
                var filePath = Playnite.SDK.API.Instance.Dialogs.SelectFolder();
                settings.EmudeckInstallDir = filePath;
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