namespace ICTAce.FileHub.MyTheme
{
    public class ThemeInfo : ITheme
    {
        public Oqtane.Models.Theme Theme => new Oqtane.Models.Theme
        {
            Name = "MyTheme",
            Version = "1.0.0",
            PackageName = "ICTAce.FileHub",
            ThemeSettingsType = "ICTAce.FileHub.MyTheme.ThemeSettings, ICTAce.FileHub.Client.Oqtane",
            ContainerSettingsType = "ICTAce.FileHub.MyTheme.ContainerSettings, ICTAce.FileHub.Client.Oqtane",
            Resources = new List<Resource>()
            {
                new Stylesheet(Constants.BootstrapStylesheetUrl, Constants.BootstrapStylesheetIntegrity, "anonymous"),
                new Stylesheet("~/Theme.css"),
                new Script(Constants.BootstrapScriptUrl, Constants.BootstrapScriptIntegrity, "anonymous")
            }
        };
    }
}
