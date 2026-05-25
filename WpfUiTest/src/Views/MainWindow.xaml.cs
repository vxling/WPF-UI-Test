using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using WpfUiTest.Themes;

namespace WpfUiTest.Views;

public partial class MainWindow : Window
{
    private bool _isDarkMode = true;
    private ListViewItem? _lastSelectedItem;
    private ListViewItem? _lastHoverItem;

    private readonly SolidColorBrush _lightBorderBrush = new(Color.FromRgb(0x99, 0xC8, 0xF0));
    private readonly SolidColorBrush _darkBorderBrush = new(Color.FromRgb(0x4A, 0x82, 0xC2));

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        UpdateThemeButton();
    }

    private void SyncRecordList_Loaded(object sender, RoutedEventArgs e)
    {
        // ListView 加载完成后，给每个 ListViewItem 绑定事件并设置初始样式
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
                {
                    // 固定 Padding，全程不变化
                    container.Padding = new Thickness(1);
                    container.MouseEnter -= Item_MouseEnter;
                    container.MouseLeave -= Item_MouseLeave;
                    container.MouseEnter += Item_MouseEnter;
                    container.MouseLeave += Item_MouseLeave;
                    ResetItemStyle(container);
                }
            }
        }
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

    private void Item_MouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is ListViewItem item && item != _lastSelectedItem)
            ApplyHoverStyle(item);
    }

    private void Item_MouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is ListViewItem item && item != _lastSelectedItem)
            ResetItemStyle(item);
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

    private void ThemeToggle_Click(object sender, RoutedEventArgs e)
    {
        _isDarkMode = !_isDarkMode;
        ThemeService.Instance.ApplyTheme(_isDarkMode ? "dark" : "light");
        UpdateThemeButton();
        // 主题切换后，重新生成所有容器并应用样式
        SyncRecordList.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
        SyncRecordList.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        // 强制重新生成容器
        if (SyncRecordList.ItemContainerGenerator.Status ==
            System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
        {
            for (int i = 0; i < SyncRecordList.Items.Count; i++)
            {
                var container = SyncRecordList.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                if (container != null)
                {
                    container.Padding = new Thickness(1);
                    if (container == _lastSelectedItem)
                        ApplySelectedStyle(container);
                    else
                        ResetItemStyle(container);
                }
            }
        }
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

    private void ApplySelectedStyle(ListViewItem item)
    {
        item.Padding = new Thickness(1);
        item.BorderThickness = new Thickness(1);
        item.BorderBrush = _isDarkMode ? _darkBorderBrush : _lightBorderBrush;
        item.Background = _isDarkMode
            ? new SolidColorBrush(Color.FromRgb(0x1F, 0x4A, 0x78))
            : new SolidColorBrush(Color.FromRgb(0xCC, 0xE8, 0xFF));
        item.Foreground = _isDarkMode ? Brushes.White : Brushes.Black;
    }

    private void ApplyHoverStyle(ListViewItem item)
    {
        item.Padding = new Thickness(1);
        item.BorderThickness = new Thickness(1);
        item.BorderBrush = _isDarkMode ? _darkBorderBrush : _lightBorderBrush;
        item.Background = _isDarkMode
            ? new SolidColorBrush(Color.FromRgb(0x38, 0x38, 0x38))
            : new SolidColorBrush(Color.FromRgb(0xF0, 0xF0, 0xF0));
        item.Foreground = _isDarkMode
            ? new SolidColorBrush(Color.FromRgb(0xE5, 0xE5, 0xE5))
            : new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x1A));
    }

    private void ResetItemStyle(ListViewItem item)
    {
        item.Padding = new Thickness(1);
        item.BorderThickness = new Thickness(0);
        item.Background = _isDarkMode
            ? new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E))
            : Brushes.White;
        item.Foreground = _isDarkMode
            ? new SolidColorBrush(Color.FromRgb(0xE5, 0xE5, 0xE5))
            : new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x1A));
    }
}