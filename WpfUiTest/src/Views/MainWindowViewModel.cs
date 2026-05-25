using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfUiTest.Views;

public class SyncRecord : INotifyPropertyChanged
{
    private bool _isSelected;
    public string TimestampDisplay { get; set; } = "";
    public string RepoName { get; set; } = "";
    public string FilePath { get; set; } = "";
    public string OperationDisplay { get; set; } = "";
    public string ResultDisplay { get; set; } = "";

    public bool IsSelected
    {
        get => _isSelected;
        set { _isSelected = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class MainWindowViewModel
{
    public List<SyncRecord> SyncRecords { get; } = new()
    {
        new SyncRecord { TimestampDisplay = "2026-05-25 11:30:01", RepoName = "SVNFileBox", FilePath = "src/Views/MainWindow.xaml", OperationDisplay = "Update", ResultDisplay = "成功" },
        new SyncRecord { TimestampDisplay = "2026-05-25 11:28:45", RepoName = "SVNFileBox", FilePath = "src/Services/SyncService.cs", OperationDisplay = "Commit", ResultDisplay = "成功" },
        new SyncRecord { TimestampDisplay = "2026-05-25 11:25:12", RepoName = "RepoA", FilePath = "docs/readme.md", OperationDisplay = "Update", ResultDisplay = "成功" },
        new SyncRecord { TimestampDisplay = "2026-05-25 11:20:03", RepoName = "RepoB", FilePath = "bin/debug/app.exe", OperationDisplay = "Commit", ResultDisplay = "冲突" },
        new SyncRecord { TimestampDisplay = "2026-05-25 11:15:33", RepoName = "RepoB", FilePath = "src/main.cs", OperationDisplay = "Add", ResultDisplay = "成功" },
        new SyncRecord { TimestampDisplay = "2026-05-25 11:10:20", RepoName = "SVNFileBox", FilePath = "src/App.xaml", OperationDisplay = "Update", ResultDisplay = "失败" },
        new SyncRecord { TimestampDisplay = "2026-05-25 11:05:08", RepoName = "RepoA", FilePath = "config/settings.json", OperationDisplay = "Commit", ResultDisplay = "成功" },
        new SyncRecord { TimestampDisplay = "2026-05-25 11:00:55", RepoName = "RepoC", FilePath = "data/db.sqlite", OperationDisplay = "Update", ResultDisplay = "成功" },
        new SyncRecord { TimestampDisplay = "2026-05-25 10:55:41", RepoName = "SVNFileBox", FilePath = "src/Views/SettingsWindow.xaml", OperationDisplay = "Commit", ResultDisplay = "成功" },
        new SyncRecord { TimestampDisplay = "2026-05-25 10:50:27", RepoName = "RepoB", FilePath = "tests/test_main.cs", OperationDisplay = "Add", ResultDisplay = "成功" },
        new SyncRecord { TimestampDisplay = "2026-05-25 10:45:13", RepoName = "RepoC", FilePath = "logs/app.log", OperationDisplay = "Update", ResultDisplay = "冲突" },
        new SyncRecord { TimestampDisplay = "2026-05-25 10:40:00", RepoName = "SVNFileBox", FilePath = "README.md", OperationDisplay = "Commit", ResultDisplay = "成功" },
    };
}