using System.Windows;
using AvalonDock.Core;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ExampleApp
{
    public sealed class ExampleTool : ObservableObject, IToolbox
    {
        private bool isOpen;
        private bool isActive;
        private DockZone zone;

        public string Id { get; set; }
        public string Title { get; set; }
        public object Context { get; set; }
        public IDockable Owner { get; set; }
        public IFactory Factory { get; set; }
        public bool CanClose { get; set; } = true;
        public bool CanPin { get; set; } = true;
        public bool CanFloat { get; set; } = true;
        public bool CanDrag { get; set; } = true;
        public bool CanDrop { get; set; } = true;
        public bool IsModified { get; set; }
        public bool IsActive { get => isActive; set => SetProperty(ref isActive, value); }
        public DockState DockState { get; set; }
        public string ToolTipText { get; set; }
        public DockZone Zone { get => zone; set => SetProperty(ref zone, value); }
        public string Shortcut { get; set; }
        public bool IsOpenByDefault { get; set; }
        public bool IsOpen { get => isOpen; set => SetProperty(ref isOpen, value); }
        public object Icon { get; set; }
        public FrameworkElement View { get; set; }
        public bool OnClose() => true;
        public void OnSelected() => IsActive = true;
    }
}