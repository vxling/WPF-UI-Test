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

            ResourceDictionary? toRemove = null;
            foreach (var d in Application.Current.Resources.MergedDictionaries)
            {
                if (d.Source?.OriginalString.Contains("Win11") == true)
                {
                    toRemove = d;
                    break;
                }
            }
            if (toRemove != null)
                Application.Current.Resources.MergedDictionaries.Remove(toRemove);

            Application.Current.Resources.MergedDictionaries.Insert(0, dict);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Theme] Failed to load theme: {ex.Message}");
        }
    }

    public string GetCurrentTheme() => _currentTheme;
}