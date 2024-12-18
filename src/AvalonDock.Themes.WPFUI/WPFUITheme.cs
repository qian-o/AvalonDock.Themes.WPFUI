using System;

namespace AvalonDock.Themes.WPFUI
{
    public class WPFUITheme : Theme
    {
        public override Uri GetResourceUri()
        {
            return new Uri("/AvalonDock.Themes.WPFUI;component/Theme.xaml", UriKind.Relative);
        }
    }
}