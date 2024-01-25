using System.IO;
using System.Reflection;
using EmudeckPlaynite.Model;
using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Plugins;
using YamlDotNet.Serialization.NamingConventions;

namespace EmudeckPlaynite{
    class EmulatorConfigurationDefinitionService{
        public static Configuration GetConfiguration(){
            var configPath =  Path.Combine(GetExtensionInstallPath(), "Resources", "emulators.yml");

            try{
                var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();
                var file = File.ReadAllText(configPath);
                return deserializer.Deserialize<Configuration>(file);            

            } catch(System.Exception e){
                // PlayniteApi.Dialogs.ShowErrorMessage("Unable to read configuration file: " + configPath + "\n" + e.Message, "Emudeck");
                return new Configuration();
            }
            
        }
        
        private static string GetExtensionInstallPath(){
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}