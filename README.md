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

## Floating Window Minimum Size

Floating windows have a minimum outer size of 280 x 180 DIPs for documents and 200 x 140 DIPs for tools. This keeps the title bar and window controls usable when no floating size has been configured or a saved size is too small. Larger window and layout-model minimums are preserved; docked pane constraints and larger floating sizes are unchanged.

## Example Application

[ExampleApp](src/ExampleApp/ExampleApp.csproj) demonstrates document tabs, tool panes, floating windows, auto-hide, and theme switching.

To build the solution, use Windows with the .NET 10 SDK and .NET Framework 4.8 targeting pack. Run the following commands from the repository root:

```powershell
dotnet build AvalonDock.Themes.WPFUI.slnx
dotnet run --project src/ExampleApp/ExampleApp.csproj --framework net10.0-windows
```

## References

- [AvalonDock](https://github.com/Dirkster99/AvalonDock)
- [WPF-UI](https://github.com/lepoco/wpfui)
- [AakStudio.Shell.UI.Themes.AvalonDock](https://github.com/Wenveo/AakStudio.Shell.UI.Themes.AvalonDock)

## License

Licensed under the [MIT License](LICENSE).