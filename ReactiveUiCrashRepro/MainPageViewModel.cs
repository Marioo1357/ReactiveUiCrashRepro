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
    private const string HomeGeometry =
        "M10 20v-6h4v6h5v-8h3L12 3 2 12h3v8z";

    private const string SearchGeometry =
        "M15.5 14h-.79l-.28-.27A6.471 6.471 0 0 0 16 9.5 6.5 6.5 0 1 0 9.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 4.99L20.49 19l-4.99-5zM9.5 14C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z";

    private const string StarGeometry =
        "M12 17.27L18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z";

    private const string ProfileGeometry =
        "M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z";

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
            new TabItem { Title = "Home",    Icon = "house.fill",         IconGeometry = HomeGeometry    },
            new TabItem { Title = "Search",  Icon = "magnifyingglass",    IconGeometry = SearchGeometry  },
            new TabItem { Title = "Explore", Icon = "star.fill",          IconGeometry = StarGeometry    },
            new TabItem { Title = "Profile", Icon = "person.circle.fill", IconGeometry = ProfileGeometry },
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