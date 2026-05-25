using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfUiTest.Themes;

namespace WpfUiTest.Views;

public partial class MainWindow : Window
{
    private bool _isDarkMode = true;
    private ListViewItem? _lastSelectedItem;

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
                    container.MouseEnter -= Item_MouseEnter;
                    container.MouseLeave -= Item_MouseLeave;
                    container.MouseEnter += Item_MouseEnter;
                    container.MouseLeave += Item_MouseLeave;
                    ResetItemStyle(container);
                }
            }
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
                        ResetItemStyle(container);
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

    private void ApplySelectedStyle(ListViewItem item)
    {
        item.Background = _isDarkMode
            ? new SolidColorBrush(Color.FromRgb(0x1F, 0x4A, 0x78))
            : new SolidColorBrush(Color.FromRgb(0xCC, 0xE8, 0xFF));
        item.Foreground = _isDarkMode ? Brushes.White : Brushes.Black;
        item.BorderBrush = _isDarkMode ? DarkBorderBrush : LightBorderBrush;
        item.BorderThickness = new Thickness(1);
    }

    private void ApplyHoverStyle(ListViewItem item)
    {
        item.Background = _isDarkMode ? DarkBgHover : LightBgHover;
        item.Foreground = _isDarkMode ? DarkFgNormal : LightFgNormal;
        item.BorderBrush = _isDarkMode ? DarkBorderBrush : LightBorderBrush;
        item.BorderThickness = new Thickness(1);
    }

    private void ResetItemStyle(ListViewItem item)
    {
        item.Background = _isDarkMode ? DarkBgNormal : Brushes.White;
        item.Foreground = _isDarkMode ? DarkFgNormal : LightFgNormal;
        item.BorderBrush = Brushes.Transparent;  // 默认无边框
        item.BorderThickness = new Thickness(1);
    }
}