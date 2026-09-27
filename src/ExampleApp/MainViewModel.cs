using System;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Appearance;

namespace ExampleApp
{
    public class MainViewModel : ObservableRecipient, IDisposable
    {
        private bool isLightTheme;

        public MainViewModel()
        {
            isLightTheme = ApplicationThemeManager.GetAppTheme() != ApplicationTheme.Dark;
            ApplicationThemeManager.Changed += ApplicationThemeManager_Changed;
        }

        public void Dispose()
        {
            ApplicationThemeManager.Changed -= ApplicationThemeManager_Changed;
        }

        public bool IsLightTheme
        {
            get => isLightTheme;
            set
            {
                if (SetProperty(ref isLightTheme, value))
                {
                    ApplicationThemeManager.Apply(value ? ApplicationTheme.Light : ApplicationTheme.Dark);
                }
            }
        }

        private void ApplicationThemeManager_Changed(ApplicationTheme currentApplicationTheme, Color systemAccent)
        {
            SetProperty(ref isLightTheme, currentApplicationTheme != ApplicationTheme.Dark, nameof(IsLightTheme));
        }
    }
}
