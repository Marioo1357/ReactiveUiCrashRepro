using System.Windows.Input;
using ReactiveUI;
using ReactiveUiCrashRepro.Controls;

namespace ReactiveUiCrashRepro;

public class MainPageViewModel : ReactiveObject
{
    private int _selectedTabIndex;

    public MainPageViewModel()
    {
        ClickCommand1 = ReactiveCommand.CreateFromTask(ClickCommand1Execute);
        ClickCommand2 = ReactiveCommand.CreateFromTask(ClickCommand2Execute);

        TabItems = new List<TabItem>
        {
            new TabItem { Title = "Home",    Icon = "house.fill"         },
            new TabItem { Title = "Search",  Icon = "magnifyingglass"    },
            new TabItem { Title = "Explore", Icon = "star.fill"          },
            new TabItem { Title = "Profile", Icon = "person.circle.fill" },
        };
    }

    // ── Existing commands ────────────────────────────────────────────────────

    public ICommand ClickCommand1 { get; }
    public ICommand ClickCommand2 { get; }

    // ── Tab bar ──────────────────────────────────────────────────────────────

    /// <summary>Items shown in the NativeTabBar.</summary>
    public IList<TabItem> TabItems { get; }

    /// <summary>Currently selected tab (two-way bound to NativeTabBar.SelectedIndex).</summary>
    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedTabIndex, value);
            this.RaisePropertyChanged(nameof(SelectedTabTitle));
        }
    }

    /// <summary>Title of the currently selected tab, for demo display.</summary>
    public string SelectedTabTitle =>
        TabItems is { Count: > 0 } && SelectedTabIndex >= 0 && SelectedTabIndex < TabItems.Count
            ? TabItems[SelectedTabIndex].Title
            : string.Empty;

    // ── Private helpers ──────────────────────────────────────────────────────

    private async Task ClickCommand1Execute()
    {
        Console.WriteLine("Clicked 1");
        await Task.Delay(5);
        Console.WriteLine("Finished 1");
    }

    private async Task ClickCommand2Execute()
    {
        Console.WriteLine("Clicked 2");
        await Task.Delay(5);
        Console.WriteLine("Finished 2");
    }
}