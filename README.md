# AvalonDock.Themes.WPFUI

[![NuGet Version](https://img.shields.io/nuget/v/AvalonDock.Themes.WPFUI)](https://nuget.org/packages/AvalonDock.Themes.WPFUI)

AvalonDock.Themes.WPFUI is a theme library for AvalonDock based on [WPF-UI](https://github.com/lepoco/wpfui).

## Installation
```bash
dotnet add package AvalonDock.Themes.WPFUI
```

## Usage
```xaml
// App.xaml
xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"

<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ui:ThemesDictionary Theme="Unknown" />
            <ui:ControlsDictionary />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

```xaml
// Use WPFUITheme in DockingManager
xmlns:wpfui="clr-namespace:AvalonDock.Themes.WPFUI;assembly=AvalonDock.Themes.WPFUI"

<DockingManager>
    <DockingManager.Theme>
        <wpfui:WPFUITheme />
    </DockingManager.Theme>
</DockingManager>
```

## Screenshots
| Light | Dark |
| ----- | ---- |
| ![image](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/1L.png) | ![image](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/1D.png) |
| ![image](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/2L.png) | ![image](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/2D.png) |
| ![image](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/3L.png) | ![image](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/3D.png) |
| ![image](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/4L.png) | ![image](https://raw.githubusercontent.com/qian-o/AvalonDock.Themes.WPFUI/master/Screenshots/4D.png) |

## References
- [AvalonDock](https://github.com/Dirkster99/AvalonDock)
- [WPF-UI](https://github.com/lepoco/wpfui)
- [AakStudio.Shell.UI.Themes.AvalonDock](https://github.com/Wenveo/AakStudio.Shell.UI.Themes.AvalonDock)

## Version History
- 1.0.5
  - Ajust the drag hit range of AnchorablePaneTitle.
  - Add focus style.

- 1.0.4
  - Ajust the display level of LayoutGridResizerControl, it can be dragged better.

- 1.0.3
  - Ajust the layout of TabHeader in LayoutAnchorablePaneControl.
  - Simplify the style dictionary of TabHeader.
  - Complete the menu style of DocumentPane.
  - Fix some style issues.

- 1.0.2
  - Add more framework support.

- 1.0.1
  - Split style dictionary.
  - Improve NavigatorWindow style.
  - Fix some style issues.

- 1.0.0
  - Initial release.