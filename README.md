# AvalonDock.Themes.WPFUI

[![NuGet](https://img.shields.io/nuget/v/AvalonDock.Themes.WPFUI)](https://www.nuget.org/packages/AvalonDock.Themes.WPFUI)

A Fluent-style theme for [AvalonDock](https://github.com/Dirkster99/AvalonDock), using the light and dark palettes from [WPF UI](https://github.com/lepoco/wpfui).

One theme supports both `DockingManager` and `ToggleDockingManager`: document tabs, tool panes, auto-hide tabs, toggle sidebars, floating windows, the window navigator, and docking guides. AvalonDock provides the docking behavior and layout management; this package provides their appearance.

## Preview

Screenshots of the current [example application](https://github.com/qian-o/AvalonDock.Themes.WPFUI/tree/master/src/ExampleApp), captured at 150% display scaling. Document and tool content is static sample UI, not an editor, terminal, or other feature supplied by the theme. Select an image to view it at full size.

| Layout | Light | Dark |
| :--- | :---: | :---: |
| Docking | [![Light document tabs, docked tools, and auto-hide tabs](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/docked-light.png)](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/docked-light.png) | [![Dark document tabs, docked tools, and auto-hide tabs](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/docked-dark.png)](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/docked-dark.png) |
| ToggleDocking | [![Light toggle sidebars with Explorer and Terminal panes open](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/toggle-docking-light.png)](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/toggle-docking-light.png) | [![Dark toggle sidebars with Explorer and Terminal panes open](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/toggle-docking-dark.png)](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/toggle-docking-dark.png) |
| Floating windows | [![Light floating document and tool windows](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/floating-light.png)](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/floating-light.png) | [![Dark floating document and tool windows](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/floating-dark.png)](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/floating-dark.png) |
| Docking preview | [![Light docking guides and translucent placement preview](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/docking-preview-light.png)](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/docking-preview-light.png) | [![Dark docking guides and translucent placement preview](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/docking-preview-dark.png)](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/docking-preview-dark.png) |

## Compatibility

| Component | Current source |
| :--- | :--- |
| Platform | Windows desktop, WPF |
| Target frameworks | .NET Framework 4.8 and .NET 10 (`net10.0-windows`) |
| AvalonDock | `Dirkster.AvalonDock` 5.0.0 |
| WPF UI | `WPF-UI` 4.3.0 |

AvalonDock and WPF UI are NuGet dependencies of the theme package. For an older published version, check that version's dependencies on NuGet.

## Quick Start

### 1. Install

Run from your WPF application project directory:

```powershell
dotnet add package AvalonDock.Themes.WPFUI
```

### 2. Register Resources

In your application's `App.xaml`, merge these dictionaries in this order. Keep your existing startup configuration and other application resources.

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

Replace `YourApp` with your namespace. Use `Theme="Dark"` for an initially dark palette.

Keep the AvalonDock theme dictionary at **application scope**, even when setting the manager's `Theme` below. Dynamically created Toggle sidebar buttons also need these styles during creation and reparenting, when manager-scoped resources may not be available.

### 3. Apply the Theme

Place a docking manager in your window and set its `Theme` to `WPFUITheme`. Existing AvalonDock layouts can remain unchanged.

```xaml
<dock:DockingManager
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:dock="clr-namespace:AvalonDock;assembly=AvalonDock"
    xmlns:layout="clr-namespace:AvalonDock.Layout;assembly=AvalonDock"
    xmlns:theme="clr-namespace:AvalonDock.Themes.WPFUI;assembly=AvalonDock.Themes.WPFUI">
    <dock:DockingManager.Theme>
        <theme:WPFUITheme />
    </dock:DockingManager.Theme>
    <layout:LayoutRoot>
        <layout:LayoutPanel>
            <layout:LayoutDocumentPane>
                <layout:LayoutDocument Title="Document 1" ContentId="document-1">
                    <TextBlock Margin="24" Text="Document content" />
                </layout:LayoutDocument>
            </layout:LayoutDocumentPane>
        </layout:LayoutPanel>
    </layout:LayoutRoot>
</dock:DockingManager>
```

## ToggleDocking

Use `dock:ToggleDockingManager` instead of `dock:DockingManager` for sidebar buttons that open and collapse tool panes. The same resource registration and `WPFUITheme` apply; document tabs share the normal docking style.

AvalonDock's `ButtonSize`, `DefaultDockWidth`, `DefaultDockHeight`, and `LayoutPriority` configure the layout. The example uses a 40-DIP button size. Set `ToggleDock.Icon` on a tool to provide sidebar content, or use `ToggleDock.IconTemplate` for a custom template:

```xaml
<layout:LayoutAnchorable
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:layout="clr-namespace:AvalonDock.Layout;assembly=AvalonDock"
    xmlns:controls="clr-namespace:AvalonDock.Controls;assembly=AvalonDock"
    xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"
    Title="Explorer"
    ContentId="explorer">
    <controls:ToggleDock.Icon>
        <ui:SymbolIcon FontSize="20" Symbol="Folder20" />
    </controls:ToggleDock.Icon>
    <TextBlock Margin="16" Text="Explorer content" />
</layout:LayoutAnchorable>
```

Place this tool inside a `LayoutAnchorGroup` on a layout side, such as `LayoutRoot.LeftSide`. Image icons, custom icon content, and text-only buttons are supported. For a complete layout with left and bottom tool panes, see [MainWindow.xaml](https://github.com/qian-o/AvalonDock.Themes.WPFUI/blob/master/src/ExampleApp/MainWindow.xaml).

## Theme Switching

Use WPF UI's theme manager to update the palette at runtime:

```csharp
using Wpf.Ui.Appearance;

ApplicationThemeManager.Apply(ApplicationTheme.Light);
// Or: ApplicationThemeManager.Apply(ApplicationTheme.Dark);
```

To follow Windows theme changes, register your window with `SystemThemeWatcher.Watch(this)` after `InitializeComponent()`. These are WPF UI APIs; no additional theme-specific service is required. See the [WPF UI theme documentation](https://wpfui.lepo.co/documentation/themes.html) for its theme and backdrop options.

## Example

The example has two switches in its title bar: **Light / Dark** and **Docking / ToggleDocking**. Each mode has its own directly declared XAML layout; the mode switch changes which manager is visible. It is not a layout conversion or migration feature.

Documents, lists, trees, and tool messages are static content. The example showcases AvalonDock's built-in docking, floating, and auto-hide interactions without implementing file editing, search, source control, or a terminal.

To build both targets, use Windows with the .NET 10 SDK and the .NET Framework 4.8 targeting pack. From the repository root:

```powershell
dotnet build AvalonDock.Themes.WPFUI.slnx --configuration Release
dotnet run --project src/ExampleApp/ExampleApp.csproj --framework net10.0-windows
```

To run the .NET Framework example, use `--framework net48` instead.

## Integration Notes

- **Floating window minimums:** document windows have a minimum outer size of 280 x 180 DIPs; tool windows use 200 x 140 DIPs. Larger window or layout-model minimums are preserved. Docked pane constraints are not changed.
- **Selector hosts:** AvalonDock 5.0.0 can raise a read-only `IsSelectionActive` property error when a manager inside a `Selector`, such as a `TabItem` host, transfers focus to a floating window. Hosting the manager outside the enclosing selector avoids that trigger. The example uses sibling managers, and the theme does not patch AvalonDock's property synchronization.
- **Application content:** the theme does not supply document editors, tool implementations, persistence, or mode-switch lifecycle management. Those remain the responsibility of the application and AvalonDock.

## Acknowledgements

- [AvalonDock](https://github.com/Dirkster99/AvalonDock)
- [WPF UI](https://github.com/lepoco/wpfui)
- [AakStudio.Shell.UI.Themes.AvalonDock](https://github.com/Wenveo/AakStudio.Shell.UI.Themes.AvalonDock)

## License

[MIT](https://github.com/qian-o/AvalonDock.Themes.WPFUI/blob/master/LICENSE).
