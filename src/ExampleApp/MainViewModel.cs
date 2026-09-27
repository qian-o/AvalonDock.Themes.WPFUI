using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Appearance;

namespace ExampleApp
{
    public class MainViewModel : ObservableRecipient
    {
        private bool isLightTheme;

        public MainViewModel()
        {
            Update();

            ApplicationThemeManager.Changed += (_, __) => Update();

            void Update()
            {
                IsLightTheme = ApplicationThemeManager.GetAppTheme() != ApplicationTheme.Dark;
            }
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
    }
}
