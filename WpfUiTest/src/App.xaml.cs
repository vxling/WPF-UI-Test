using System.Windows;
using WpfUiTest.Themes;

namespace WpfUiTest;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 启动时跟随系统主题
        var isDark = ThemeWatcher.Instance.IsSystemDarkMode();
        ThemeService.Instance.ApplyTheme(isDark ? "dark" : "light");

        // 监听系统主题变化
        ThemeWatcher.Instance.SystemThemeChanged += isDarkMode =>
        {
            // 更新按钮文字（如果 MainWindow 存在）
            if (MainWindow is Views.MainWindow mw)
                mw.UpdateThemeToggleButton(isDarkMode);
        };

        ThemeWatcher.Instance.Start();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        ThemeWatcher.Instance.Stop();
        base.OnExit(e);
    }
}
