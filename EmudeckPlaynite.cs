using EmudeckPlaynite.Model;
using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Plugins;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using YamlDotNet.Serialization.NamingConventions;

namespace EmudeckPlaynite
{
    public class EmudeckPlaynite : GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        private EmudeckPlayniteSettingsViewModel settings { get; set; }
        private EmudeckConfigurator configurator { get; set; }

        public override Guid Id { get; } = Guid.Parse("82cf60ec-8091-488d-9c85-63836ebee151");

        private Configuration GetConfiguration() {
            var configPath =  Path.Combine(GetExtensionInstallPath(), "Resources", "emulators.yml");

            try{
                var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();
                var file = File.ReadAllText(configPath);
                return deserializer.Deserialize<Configuration>(file);            

            } catch(Exception e){
                PlayniteApi.Dialogs.ShowErrorMessage("Unable to read configuration file: " + configPath + "\n" + e.Message, "Emudeck");
                return new Configuration();
            }
            
        }

        private string GetExtensionInstallPath(){
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public EmudeckPlaynite(IPlayniteAPI api) : base(api)
        {
            
            settings = new EmudeckPlayniteSettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
            
            settings.Settings.PropertyChanged += Reset;
        }

        public void Reset(object sender, PropertyChangedEventArgs args){
            configurator = new EmudeckConfigurator(
                settings.Settings.EmudeckInstallDir, 
                GetConfiguration()
            );
            configurator.RemoveAllEmulators();
            configurator.AddEmulators();
        }

        public override void OnGameInstalled(OnGameInstalledEventArgs args)
        {
            // Add code to be executed when game is finished installing.
        }

        public override void OnGameStarted(OnGameStartedEventArgs args)
        {
            // Add code to be executed when game is started running.
        }

        public override void OnGameStarting(OnGameStartingEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameStopped(OnGameStoppedEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameUninstalled(OnGameUninstalledEventArgs args)
        {
            // Add code to be executed when game is uninstalled.
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            // Add code to be executed when Playnite is initialized.
        }

        public override void OnApplicationStopped(OnApplicationStoppedEventArgs args)
        {
            // Add code to be executed when Playnite is shutting down.
        }

        public override void OnLibraryUpdated(OnLibraryUpdatedEventArgs args)
        {
            // Add code to be executed when library is updated.
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new EmudeckPlayniteSettingsView();
        }
    }
}