using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AvalonDock;
using AvalonDock.Core;
using AvalonDock.Layout;
using Wpf.Ui.Controls;
using Button = System.Windows.Controls.Button;
using TextBlock = System.Windows.Controls.TextBlock;
using TextBox = System.Windows.Controls.TextBox;
using TreeViewItem = System.Windows.Controls.TreeViewItem;

namespace ExampleApp
{
    internal sealed class SampleWorkspace
    {
        private readonly DockingManager manager;
        private readonly Dictionary<string, string> files;
        private readonly Dictionary<string, TextBox> editors = new Dictionary<string, TextBox>();

        private SampleWorkspace(DockingManager manager, bool toggle)
        {
            this.manager = manager;
            var type = toggle ? "ToggleDockingManager" : "DockingManager";
            files = new Dictionary<string, string>
            {
                ["Workspace.xaml"] = "<" + type + ">\n    <" + type + ".Theme>\n        <themes:WPFUITheme />\n    </" + type + ".Theme>\n\n    <LayoutRoot>\n        <LayoutPanel>\n            <LayoutDocumentPane>\n                <LayoutDocument Title=\"Workspace.xaml\" />\n                <LayoutDocument Title=\"App.xaml\" />\n            </LayoutDocumentPane>\n        </LayoutPanel>\n    </LayoutRoot>\n</" + type + ">",
                ["App.xaml"] = "<Application.Resources>\n    <ResourceDictionary>\n        <ResourceDictionary.MergedDictionaries>\n            <ui:ThemesDictionary Theme=\"Light\" />\n            <ui:ControlsDictionary />\n        </ResourceDictionary.MergedDictionaries>\n    </ResourceDictionary>\n</Application.Resources>",
                ["MainWindow.xaml.cs"] = "using System.Windows;\n\nnamespace ExampleApp\n{\n    public partial class MainWindow : Window\n    {\n        public MainWindow()\n        {\n            InitializeComponent();\n        }\n    }\n}",
                ["README.md"] = "# Example workspace\n\nAvalonDock with the WPF UI theme.\n\nThese files are an in-memory example."
            };
        }

        public static void Populate(DockingManager manager, bool toggle) => new SampleWorkspace(manager, toggle).Populate(toggle);

        private void Populate(bool toggle)
        {
            foreach (var document in manager.Layout.Descendents().OfType<LayoutDocument>()) document.Content = Editor(document.Title);
            foreach (var tool in manager.Layout.Descendents().OfType<LayoutAnchorable>())
            {
                var view = ToolContent(tool.Title);
                if (!toggle) { tool.Content = view; continue; }
                var bottom = tool.Title == "Terminal" || tool.Title == "Problems";
                tool.Content = new ExampleTool
                {
                    Id = tool.ContentId, Title = tool.Title, ToolTipText = tool.Title,
                    Zone = bottom ? DockZone.BottomLeft : DockZone.LeftTop,
                    IsOpen = tool.Title == "Explorer" || tool.Title == "Terminal",
                    Shortcut = tool.Title == "Explorer" ? "Ctrl+Shift+E" : tool.Title == "Search" ? "Ctrl+Shift+F" : null,
                    Icon = Icon(tool.Title), View = view
                };
            }
        }

        private FrameworkElement Editor(string name)
        {
            var editor = new TextBox
            {
                Text = files[name], FontFamily = new FontFamily("Cascadia Code, Consolas"), FontSize = 13,
                AcceptsReturn = true, AcceptsTab = true, BorderThickness = new Thickness(0),
                Background = Brushes.Transparent, Padding = new Thickness(22, 20, 22, 20),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            editor.SetResourceReference(Control.ForegroundProperty, "TextFillColorPrimaryBrush");
            editor.TextChanged += (sender, args) => files[name] = editor.Text;
            editors[name] = editor;
            return editor;
        }

        private void OpenDocument(string name)
        {
            var document = manager.Layout.Descendents().OfType<LayoutDocument>().FirstOrDefault(item => item.Title == name);
            if (document == null)
            {
                document = new LayoutDocument { Title = name, ContentId = "file-" + name, Content = Editor(name) };
                manager.Layout.Descendents().OfType<LayoutDocumentPane>().First().Children.Add(document);
            }
            document.IsActive = true;
        }

        private FrameworkElement ToolContent(string title)
        {
            switch (title)
            {
                case "Explorer":
                case "Solution Explorer": return Explorer();
                case "Search": return Search();
                case "Git":
                case "Source Control": return SourceControl();
                case "Run and Debug": return Lines("No debug session", "ExampleApp", ".NET 10 / .NET Framework 4.8");
                case "Terminal": return Output("PS > dotnet build", "Build succeeded.", "    0 Warning(s)", "    0 Error(s)", "", "PS >");
                case "Output": return Output("Build started...", "AvalonDock.Themes.WPFUI -> net10.0-windows", "ExampleApp -> net10.0-windows", "Build succeeded. 0 warnings, 0 errors.");
                case "Errors":
                case "Problems": return Lines("No problems detected in the example workspace.");
                case "Properties": return Lines("Workspace.xaml", "Build action     Page", "Encoding         UTF-8", "Copy to output   Do not copy");
                default: return Lines("Controls", "Button", "TextBlock", "TreeView", "DataGrid", "ContentPresenter");
            }
        }

        private FrameworkElement Explorer()
        {
            var tree = new TreeView { BorderThickness = new Thickness(0), Background = Brushes.Transparent, Padding = new Thickness(8) };
            var root = new TreeViewItem { Header = "EXAMPLE WORKSPACE", IsExpanded = true, FontSize = 12 };
            var source = new TreeViewItem { Header = "src", IsExpanded = true };
            foreach (var name in files.Keys)
            {
                var row = new StackPanel { Orientation = Orientation.Horizontal };
                row.Children.Add(new SymbolIcon { Symbol = name.EndsWith(".md") ? SymbolRegular.DocumentText20 : SymbolRegular.Document20, FontSize = 16, Margin = new Thickness(0, 0, 8, 0) });
                row.Children.Add(new TextBlock { Text = name, VerticalAlignment = VerticalAlignment.Center });
                var item = new TreeViewItem { Header = row, Padding = new Thickness(4, 6, 4, 6), Tag = name };
                item.MouseDoubleClick += (sender, args) => { OpenDocument(name); args.Handled = true; };
                item.KeyDown += (sender, args) => { if (args.Key == Key.Enter) { OpenDocument(name); args.Handled = true; } };
                source.Items.Add(item);
            }
            root.Items.Add(source);
            tree.Items.Add(root);
            return tree;
        }

        private FrameworkElement Search()
        {
            var panel = new DockPanel { Margin = new Thickness(12) };
            var query = new TextBox { Margin = new Thickness(0, 0, 0, 12) };
            System.Windows.Automation.AutomationProperties.SetName(query, "Search workspace");
            DockPanel.SetDock(query, Dock.Top);
            panel.Children.Add(query);
            var results = new ListBox { BorderThickness = new Thickness(0), Background = Brushes.Transparent };
            query.TextChanged += (sender, args) =>
            {
                results.Items.Clear();
                if (string.IsNullOrWhiteSpace(query.Text)) return;
                foreach (var file in files)
                {
                    var lines = file.Value.Split('\n');
                    for (var index = 0; index < lines.Length; index++)
                        if (lines[index].IndexOf(query.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                            results.Items.Add(new ListBoxItem { Content = file.Key + ":" + (index + 1) + "  " + lines[index].Trim(), Tag = Tuple.Create(file.Key, index), ToolTip = lines[index].Trim() });
                }
            };
            Action openResult = () =>
            {
                if (!(results.SelectedItem is ListBoxItem item) || !(item.Tag is Tuple<string, int> result)) return;
                OpenDocument(result.Item1);
                editors[result.Item1].ScrollToLine(result.Item2);
            };
            results.MouseDoubleClick += (sender, args) => openResult();
            results.KeyDown += (sender, args) => { if (args.Key == Key.Enter) openResult(); };
            panel.Children.Add(results);
            return panel;
        }

        private FrameworkElement SourceControl()
        {
            var panel = new StackPanel { Margin = new Thickness(16) };
            panel.Children.Add(new TextBlock { Text = "main", FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 0, 0, 16) });
            panel.Children.Add(new TextBlock { Text = "Example changes", Margin = new Thickness(0, 0, 0, 10) });
            foreach (var name in new[] { "Workspace.xaml", "App.xaml" })
            {
                var button = new Button { Content = "M   " + name, HorizontalContentAlignment = HorizontalAlignment.Left, Margin = new Thickness(0, 2, 0, 2) };
                button.Click += (sender, args) => OpenDocument(name);
                panel.Children.Add(button);
            }
            return panel;
        }

        private static FrameworkElement Output(params string[] lines)
        {
            var text = new TextBox { Text = string.Join(Environment.NewLine, lines), IsReadOnly = true, AcceptsReturn = true, FontFamily = new FontFamily("Cascadia Code, Consolas"), FontSize = 12, Padding = new Thickness(16, 12, 16, 12), BorderThickness = new Thickness(0), Background = Brushes.Transparent, VerticalScrollBarVisibility = ScrollBarVisibility.Auto, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto };
            text.SetResourceReference(Control.ForegroundProperty, "TextFillColorPrimaryBrush");
            return text;
        }

        private static FrameworkElement Lines(params string[] values)
        {
            var panel = new StackPanel { Margin = new Thickness(16) };
            foreach (var value in values) panel.Children.Add(new TextBlock { Text = value, Margin = new Thickness(0, 0, 0, 12), TextWrapping = TextWrapping.Wrap });
            return panel;
        }

        private static SymbolIcon Icon(string title)
        {
            var symbol = SymbolRegular.Document20;
            switch (title)
            {
                case "Explorer": symbol = SymbolRegular.Folder20; break;
                case "Search": symbol = SymbolRegular.Search20; break;
                case "Source Control": symbol = SymbolRegular.Branch20; break;
                case "Run and Debug": symbol = SymbolRegular.Play20; break;
                case "Terminal": symbol = SymbolRegular.Code20; break;
                case "Problems": symbol = SymbolRegular.Warning20; break;
            }
            return new SymbolIcon { Symbol = symbol, FontSize = 20 };
        }
    }
}