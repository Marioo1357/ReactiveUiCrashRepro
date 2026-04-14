using System.Windows.Input;
using ReactiveUI;
using ReactiveUiCrashRepro.Controls;

namespace ReactiveUiCrashRepro;

public class MainPageViewModel : ReactiveObject
{
    private int _selectedTabIndex;

    // ── Material Design icon path data (24×24 view-box) ─────────────────────
    // These paths are used by MauiGlassTabBar (Shapes.Path) which gives full
    // fill-colour control.  You can replace them with any SVG path data.

    public MainPageViewModel()
    {
        ClickCommand1 = ReactiveCommand.CreateFromTask(ClickCommand1Execute);
        ClickCommand2 = ReactiveCommand.CreateFromTask(ClickCommand2Execute);

        TabItems = new List<TabItem>
        {
            // Icon          → SF Symbol name, used by the native iOS UITabBar.
            // IconGeometry   → SVG path data, used by MauiGlassTabBar (colour-tintable).
            // MauiIconSource → alternative: "tab_home.png" (SVG in Resources/Images,
            //                  MAUI converts to PNG at build time).
            new TabItem { Title = "Home",    Icon = "tab_home.png",        },
            new TabItem { Title = "Search",  Icon = "tab_search.png",   },
            new TabItem { Title = "Explore", Icon = "tab_star.png",         },
            new TabItem { Title = "Profile", Icon = "tab_profile.png" },
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