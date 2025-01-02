using System.Windows;
using System.Windows.Media;
using ControlzEx.Theming;
using Wpf.Ui.Appearance;

namespace ExampleApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;

            ApplicationThemeManager.Changed += ApplicationThemeManager_Changed;
        }

        private void ApplicationThemeManager_Changed(ApplicationTheme currentApplicationTheme, Color systemAccent)
        {
            ThemeManager.Current.SyncTheme();
        }
    }
}
