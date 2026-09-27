using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Appearance;

namespace ExampleApp
{
    public class MainViewModel : ObservableRecipient
    {
        private ApplicationTheme theme;
        private bool showToggleDocking;

        public MainViewModel()
        {
            Update();

            ApplicationThemeManager.Changed += (_, __) => Update();

            void Update()
            {
                Theme = ApplicationThemeManager.GetAppTheme();
            }
        }

        public ApplicationTheme Theme
        {
            get => theme;
            set
            {
                if (SetProperty(ref theme, value))
                {
                    OnPropertyChanged(nameof(IsLightTheme));
                    ApplicationThemeManager.Apply(value);
                }
            }
        }

        public bool IsLightTheme
        {
            get => Theme == ApplicationTheme.Light;
            set => Theme = value ? ApplicationTheme.Light : ApplicationTheme.Dark;
        }

        public bool ShowToggleDocking
        {
            get => showToggleDocking;
            set => SetProperty(ref showToggleDocking, value);
        }
    }
}
