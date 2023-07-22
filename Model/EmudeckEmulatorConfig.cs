namespace EmudeckPlaynite.Model
{
    public class EmudeckEmulatorConfig
    {
        public string Name { get; set; }
        public string PlatformSpecificiationId { get; set; }
        public string PlayniteEmulatorName { get; set; }
        public string Executable { get; set; }
        public string Arguments { get; set; }
        public string RomsDir { get; set; }
    }
}
