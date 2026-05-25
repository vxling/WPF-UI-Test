#nullable enable

using System;
using System.Windows;

namespace WpfUiTest.Themes;

public class ThemeService
{
    private static ThemeService? _instance;
    public static ThemeService Instance => _instance ??= new ThemeService();

    private string _currentTheme = "dark";

    public event EventHandler<string>? ThemeChanged;

    public void ApplyTheme(string theme)
    {
        _currentTheme = theme;
        LoadTheme(theme);
        ThemeChanged?.Invoke(this, theme);
    }

    private void LoadTheme(string configTheme)
    {
        var fileName = configTheme == "dark" ? "Win11DarkTheme.xaml" : "Win11LightTheme.xaml";

        try
        {
            var dict = new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/WpfUiTest;component/Themes/{fileName}")
            };

            // 移除旧的 Win11 主题字典
            var toRemove = new List<ResourceDictionary>();
            foreach (var d in Application.Current.Resources.MergedDictionaries)
            {
                if (d.Source?.OriginalString.Contains("Win11") == true)
                    toRemove.Add(d);
            }
            foreach (var d in toRemove)
                Application.Current.Resources.MergedDictionaries.Remove(d);

            // 插入新主题到最前面（优先命中）
            Application.Current.Resources.MergedDictionaries.Insert(0, dict);

            // 强制让所有窗口重新读取样式
            // WPF 的 DynamicResource 在字典更换后会自动更新，但需要触发一下 visual 更新
            foreach (Window w in Application.Current.Windows)
            {
                w.Dispatcher.InvokeAsync(() =>
                {
                    w.InvalidateVisual();
                    w.InvalidateProperty(Window.BackgroundProperty);
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Theme] Failed to load theme: {ex.Message}");
        }
    }

    public string GetCurrentTheme() => _currentTheme;
}