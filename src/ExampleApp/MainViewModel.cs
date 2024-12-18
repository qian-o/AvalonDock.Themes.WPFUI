using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Appearance;

namespace ExampleApp;

public partial class MainViewModel : ObservableRecipient
{
    [ObservableProperty]
    private bool isLightTheme = true;

    partial void OnIsLightThemeChanged(bool value)
    {
        ApplicationThemeManager.Apply(value ? ApplicationTheme.Light : ApplicationTheme.Dark);
    }
}
