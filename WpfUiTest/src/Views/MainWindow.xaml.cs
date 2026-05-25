using System.Windows;
using System.Windows.Controls;
using WpfUiTest.Themes;

namespace WpfUiTest.Views;

public partial class MainWindow : Window
{
    private bool _isDarkMode = true;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        UpdateThemeButton();
    }

    private void ThemeToggle_Click(object sender, RoutedEventArgs e)
    {
        _isDarkMode = !_isDarkMode;
        ThemeService.Instance.ApplyTheme(_isDarkMode ? "dark" : "light");
        UpdateThemeButton();
    }

    private void UpdateThemeButton()
    {
        ThemeToggleButton.Content = _isDarkMode ? "🌙 切换浅色" : "☀️ 切换深色";
    }
}
