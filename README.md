# AvalonDock.Themes.WPFUI

[![NuGet Version](https://img.shields.io/nuget/v/AvalonDock.Themes.WPFUI)](https://nuget.org/packages/AvalonDock.Themes.WPFUI)

Fluent-style themes for [AvalonDock](https://github.com/Dirkster99/AvalonDock), built with [WPF UI](https://github.com/lepoco/wpfui).

The theme styles document tabs, tool panes, auto-hide tabs, floating windows, and docking guides using WPF UI's light and dark palettes. Docking and layout management remain provided by AvalonDock.

## Requirements

The current source targets Windows WPF applications using:

- .NET Framework 4.8 or .NET 10 (`net10.0-windows`).
- Dirkster.AvalonDock 5.0.0 and WPF-UI 4.3.0, referenced by the theme package.

## Getting Started

### 1. Install the Package

Run in your WPF application project directory:

```powershell
dotnet add package AvalonDock.Themes.WPFUI
```

### 2. Register WPF UI Resources

Merge the WPF UI theme and control dictionaries into [App.xaml](src/ExampleApp/App.xaml). In the example below, replace `YourApp` with your application's namespace and keep your existing startup settings and other resources.

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
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

Use `Theme="Dark"` for a dark palette.

### 3. Apply the AvalonDock Theme

Set `DockingManager.Theme` to `WPFUITheme`. This minimal example creates a single document; you can keep your existing AvalonDock layout instead.

```xaml
<dock:DockingManager
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:dock="clr-namespace:AvalonDock;assembly=AvalonDock"
    xmlns:layout="clr-namespace:AvalonDock.Layout;assembly=AvalonDock"
    xmlns:wpfui="clr-namespace:AvalonDock.Themes.WPFUI;assembly=AvalonDock.Themes.WPFUI">
    <dock:DockingManager.Theme>
        <wpfui:WPFUITheme />
    </dock:DockingManager.Theme>
    <layout:LayoutRoot>
        <layout:LayoutPanel>
            <layout:LayoutDocumentPane>
                <layout:LayoutDocument Title="Document 1">
                    <TextBlock Margin="16" Text="Document content" />
                </layout:LayoutDocument>
            </layout:LayoutDocumentPane>
        </layout:LayoutPanel>
    </layout:LayoutRoot>
</dock:DockingManager>
```

## Example Application

[ExampleApp](src/ExampleApp/ExampleApp.csproj) demonstrates document tabs, tool panes, floating windows, auto-hide, and theme switching.

To build the solution, use Windows with the .NET 10 SDK and .NET Framework 4.8 targeting pack. Run the following commands from the repository root:

```powershell
dotnet build AvalonDock.Themes.WPFUI.slnx
dotnet run --project src/ExampleApp/ExampleApp.csproj --framework net10.0-windows
```

## Screenshots

| Light | Dark |
| ----- | ---- |
| ![Light theme, example 1](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/1L.png) | ![Dark theme, example 1](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/1D.png) |
| ![Light theme, example 2](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/2L.png) | ![Dark theme, example 2](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/2D.png) |
| ![Light theme, example 3](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/3L.png) | ![Dark theme, example 3](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/3D.png) |
| ![Light theme, example 4](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/4L.png) | ![Dark theme, example 4](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/4D.png) |

## References

- [AvalonDock](https://github.com/Dirkster99/AvalonDock)
- [WPF-UI](https://github.com/lepoco/wpfui)
- [AakStudio.Shell.UI.Themes.AvalonDock](https://github.com/Wenveo/AakStudio.Shell.UI.Themes.AvalonDock)

## Version History

- 1.1.0
  - Refactored styles to support more customization.

- 1.0.8
  - Updated WPF-UI.
  - Adjusted the auto-hide tab style to resemble Visual Studio.

- 1.0.7
  - Fixed overlapping sidebar display areas.

- 1.0.6
  - Updated WPF-UI.

- 1.0.5
  - Adjusted the tool pane title's drag hit area.
  - Added a focus style.

- 1.0.4
  - Adjusted splitter stacking order to improve dragging.

- 1.0.3
  - Adjusted tool pane tab header layout and simplified its style dictionary.
  - Completed document pane menu styles.
  - Fixed styling issues.

- 1.0.2
  - Added support for more target frameworks.

- 1.0.1
  - Split the style dictionary.
  - Improved navigator window styles.
  - Fixed styling issues.

- 1.0.0
  - Initial release.

## License

Licensed under the [MIT License](LICENSE).