# AvalonDock.Themes.WPFUI

[![NuGet](https://img.shields.io/nuget/v/AvalonDock.Themes.WPFUI)](https://www.nuget.org/packages/AvalonDock.Themes.WPFUI)

A light and dark [WPF UI](https://github.com/lepoco/wpfui) theme for [AvalonDock](https://github.com/Dirkster99/AvalonDock), supporting both `DockingManager` and `ToggleDockingManager`.

Targets .NET Framework 4.8 and .NET 10 WPF. Dependencies: AvalonDock 5.0.0 and WPF UI 4.3.0.

## Preview

| Layout | Light | Dark |
| :--- | :---: | :---: |
| Docking | ![Docking light](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/docked-light.png) | ![Docking dark](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/docked-dark.png) |
| ToggleDocking | ![ToggleDocking light](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/toggle-docking-light.png) | ![ToggleDocking dark](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/toggle-docking-dark.png) |
| Floating | ![Floating light](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/floating-light.png) | ![Floating dark](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/floating-dark.png) |
| Docking preview | ![Docking preview light](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/docking-preview-light.png) | ![Docking preview dark](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/docking-preview-dark.png) |
| Toggle preview | ![Toggle preview light](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/toggle-preview-light.png) | ![Toggle preview dark](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/toggle-preview-dark.png) |

## Usage

```powershell
dotnet add package AvalonDock.Themes.WPFUI
```

Merge the dictionaries at application scope in `App.xaml`:

```xaml
<Application x:Class="YourApp.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"
             StartupUri="MainWindow.xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ui:ThemesDictionary Theme="Light" />
                <ui:ControlsDictionary />
                <ResourceDictionary Source="pack://application:,,,/AvalonDock.Themes.WPFUI;component/Theme.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

Replace `YourApp` with your namespace, then apply `WPFUITheme` to your manager:

```xaml
<dock:DockingManager xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:dock="clr-namespace:AvalonDock;assembly=AvalonDock"
                     xmlns:layout="clr-namespace:AvalonDock.Layout;assembly=AvalonDock"
                     xmlns:theme="clr-namespace:AvalonDock.Themes.WPFUI;assembly=AvalonDock.Themes.WPFUI">
    <dock:DockingManager.Theme>
        <theme:WPFUITheme />
    </dock:DockingManager.Theme>
    <layout:LayoutRoot>
        <layout:LayoutPanel>
            <layout:LayoutDocumentPane>
                <layout:LayoutDocument Title="Document 1"
                                       ContentId="document-1">
                    <TextBlock Margin="24"
                               Text="Document content" />
                </layout:LayoutDocument>
            </layout:LayoutDocumentPane>
        </layout:LayoutPanel>
    </layout:LayoutRoot>
</dock:DockingManager>
```

Use the same theme with `ToggleDockingManager`. Complete layouts are in [MainWindow.xaml](https://github.com/qian-o/AvalonDock.Themes.WPFUI/blob/master/src/ExampleApp/MainWindow.xaml).

Switch between `Light` and `Dark` through WPF UI:

```csharp
using Wpf.Ui.Appearance;

ApplicationThemeManager.Apply(ApplicationTheme.Dark);
```

## Example

The static UI showcase includes theme and docking-mode switches. Build on Windows with the .NET 10 SDK and .NET Framework 4.8 targeting pack:

```powershell
dotnet build AvalonDock.Themes.WPFUI.slnx --configuration Release
dotnet run --project src/ExampleApp/ExampleApp.csproj --framework net10.0-windows
```

## License

[MIT](https://github.com/qian-o/AvalonDock.Themes.WPFUI/blob/master/LICENSE).

Thanks to [AakStudio.Shell.UI.Themes.AvalonDock](https://github.com/Wenveo/AakStudio.Shell.UI.Themes.AvalonDock).
