using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Appearance;

namespace ExampleApp
{
    public class MainViewModel : ObservableRecipient
    {
        private bool isLightTheme = true;

        public bool IsLightTheme
        {
            get => isLightTheme;
            set
            {
                if (IsLightTheme != value)
                {
                    SetProperty(ref isLightTheme, value);

                    ApplicationThemeManager.Apply(value ? ApplicationTheme.Light : ApplicationTheme.Dark);
                }
            }
        }
    }
}
