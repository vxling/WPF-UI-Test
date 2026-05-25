using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WpfUiTest.Themes;

namespace WpfUiTest.Views;

public partial class MainWindow : Window
{
    private bool _isDarkMode = true;
    private ListViewItem? _lastSelectedItem;
    private ListViewItem? _lastHoverItem;
    private readonly Dictionary<ListViewItem, RowBackgroundAdorner> _adornerMap = new();

    private static readonly SolidColorBrush LightBorderBrush = new(Color.FromRgb(0x99, 0xC8, 0xF0));
    private static readonly SolidColorBrush DarkBorderBrush = new(Color.FromRgb(0x4A, 0x82, 0xC2));
    private static readonly SolidColorBrush LightBgHover = new(Color.FromRgb(0xF0, 0xF0, 0xF0));
    private static readonly SolidColorBrush DarkBgHover = new(Color.FromRgb(0x38, 0x38, 0x38));
    private static readonly SolidColorBrush DarkBgNormal = new(Color.FromRgb(0x1E, 0x1E, 0x1E));
    private static readonly SolidColorBrush DarkFgNormal = new(Color.FromRgb(0xE5, 0xE5, 0xE5));
    private static readonly SolidColorBrush LightFgNormal = new(Color.FromRgb(0x1A, 0x1A, 0x1A));

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        UpdateThemeButton();
    }

    private void SyncRecordList_Loaded(object sender, RoutedEventArgs e)
    {
        SyncRecordList.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        SyncRecordList.PreviewMouseMove += SyncRecordList_PreviewMouseMove;
    }

    private void ItemContainerGenerator_StatusChanged(object? sender, EventArgs e)
    {
        if (SyncRecordList.ItemContainerGenerator.Status ==
            System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
        {
            for (int i = 0; i < SyncRecordList.Items.Count; i++)
            {
                var container = SyncRecordList.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                if (container != null)
                    AttachAdorner(container);
            }
        }
    }

    private void AttachAdorner(ListViewItem container)
    {
        var layer = AdornerLayer.GetAdornerLayer(container);
        if (layer == null) return;

        // 移除旧 adorner
        if (_adornerMap.TryGetValue(container, out var old))
        {
            layer.Remove(old);
            _adornerMap.Remove(container);
        }

        // 添加新 adorner
        var bg = container.Background ?? Brushes.Transparent;
        var adorner = new RowBackgroundAdorner(container, bg);
        layer.Add(adorner);
        _adornerMap[container] = adorner;

        // 设置基础样式
        container.Padding = new Thickness(1);
        container.BorderThickness = new Thickness(1);
        container.BorderBrush = Brushes.Transparent;
        container.Foreground = _isDarkMode ? DarkFgNormal : LightFgNormal;
    }

    private void SyncRecordList_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        var dep = e.OriginalSource as DependencyObject;
        var item = FindAncestor<ListViewItem>(dep);
        if (item != _lastHoverItem)
        {
            if (_lastHoverItem != null && _lastHoverItem != _lastSelectedItem)
                ResetItemStyle(_lastHoverItem);
            if (item != null && item != _lastSelectedItem)
                ApplyHoverStyle(item);
            _lastHoverItem = item;
        }
    }

    private void ThemeToggle_Click(object sender, RoutedEventArgs e)
    {
        _isDarkMode = !_isDarkMode;
        ThemeService.Instance.ApplyTheme(_isDarkMode ? "dark" : "light");
        UpdateThemeButton();
        SyncRecordList.Dispatcher.BeginInvoke(new Action(() =>
        {
            if (SyncRecordList.ItemContainerGenerator.Status ==
                System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                for (int i = 0; i < SyncRecordList.Items.Count; i++)
                {
                    var container = SyncRecordList.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                    if (container != null)
                        AttachAdorner(container);
                }
            }
        }), System.Windows.Threading.DispatcherPriority.Loaded);
    }

    private void UpdateThemeButton()
    {
        ThemeToggleButton.Content = _isDarkMode ? "🌙 切换浅色" : "☀️ 切换深色";
    }

    private void SyncRecordList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_lastSelectedItem != null)
            ResetItemStyle(_lastSelectedItem);

        if (SyncRecordList.SelectedItem != null)
        {
            var container = SyncRecordList.ItemContainerGenerator.ContainerFromItem(SyncRecordList.SelectedItem) as ListViewItem;
            if (container != null)
            {
                ApplySelectedStyle(container);
                _lastSelectedItem = container;
            }
        }
    }

    private static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
    {
        while (current != null)
        {
            if (current is T result)
                return result;
            current = VisualTreeHelper.GetParent(current);
        }
        return null;
    }

    private void UpdateAdornerBackground(ListViewItem item, Brush bg)
    {
        if (_adornerMap.TryGetValue(item, out var adorner))
        {
            adorner.UpdateBackground(bg);
        }
    }

    private void ApplySelectedStyle(ListViewItem item)
    {
        var bg = _isDarkMode
            ? new SolidColorBrush(Color.FromRgb(0x1F, 0x4A, 0x78))
            : new SolidColorBrush(Color.FromRgb(0xCC, 0xE8, 0xFF));
        item.Background = bg;
        item.Foreground = _isDarkMode ? Brushes.White : Brushes.Black;
        item.BorderBrush = _isDarkMode ? DarkBorderBrush : LightBorderBrush;
        item.BorderThickness = new Thickness(1);
        UpdateAdornerBackground(item, bg);
    }

    private void ApplyHoverStyle(ListViewItem item)
    {
        var bg = _isDarkMode ? DarkBgHover : LightBgHover;
        item.Background = bg;
        item.Foreground = _isDarkMode ? DarkFgNormal : LightFgNormal;
        item.BorderBrush = _isDarkMode ? DarkBorderBrush : LightBorderBrush;
        item.BorderThickness = new Thickness(1);
        UpdateAdornerBackground(item, bg);
    }

    private void ResetItemStyle(ListViewItem item)
    {
        var bg = _isDarkMode ? DarkBgNormal : Brushes.White;
        item.Background = bg;
        item.Foreground = _isDarkMode ? DarkFgNormal : LightFgNormal;
        item.BorderBrush = Brushes.Transparent;
        item.BorderThickness = new Thickness(1);
        UpdateAdornerBackground(item, bg);
    }
}
