using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using ControlzEx.Theming;
using Wpf.Ui.Appearance;

namespace ExampleApp
{
    public class MainViewModel : ObservableRecipient
    {
        private bool isLightTheme = true;

        public MainViewModel()
        {
            ApplicationThemeManager.Changed += ApplicationThemeManager_Changed;
        }

        ~MainViewModel()
        {
            ApplicationThemeManager.Changed -= ApplicationThemeManager_Changed;
        }

        public bool IsLightTheme
        {
            get => isLightTheme;
            set
            {
                if (IsLightTheme != value)
                {
                    SetProperty(ref isLightTheme, value);

                    ApplicationThemeManager.Apply(value ? ApplicationTheme.Light : ApplicationTheme.Dark);

                    ThemeManager.Current.ChangeThemeBaseColor(Application.Current, value ? "Light" : "Dark");
                }
            }
        }

        private void ApplicationThemeManager_Changed(ApplicationTheme currentApplicationTheme, Color systemAccent)
        {
            IsLightTheme = currentApplicationTheme == ApplicationTheme.Light;

            ThemeManager.Current.SyncTheme(ThemeSyncMode.SyncAll);
        }
    }
}
