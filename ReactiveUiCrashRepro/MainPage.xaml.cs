using ReactiveUiCrashRepro.Controls;

namespace ReactiveUiCrashRepro;

public partial class MainPage
{
    public MainPage()
    {
        BindingContext = new MainPageViewModel();
        InitializeComponent();
        InjectTabBar();
    }

    /// <summary>
    /// Chooses the tab bar implementation based on the current platform and OS version:
    /// <list type="bullet">
    ///   <item><b>iOS 26+</b> – <see cref="NativeTabBar"/> (native <c>UITabBar</c> with
    ///         automatic Liquid Glass).</item>
    ///   <item><b>iOS &lt; 26</b> – <see cref="MauiGlassTabBar"/> (pure MAUI frosted-glass
    ///         look-alike, since the native Liquid Glass material is not available).</item>
    ///   <item><b>Android</b> – <see cref="MauiGlassTabBar"/> (100% MAUI components with a
    ///         blurred/frosted-glass appearance).</item>
    /// </list>
    /// </summary>
    private void InjectTabBar()
    {
        View tabBar;

#if IOS
        if (OperatingSystem.IsIOSVersionAtLeast(26))
        {
            // iOS 26+ gets the real Liquid Glass from the native UITabBar.
            var native = new NativeTabBar();
            native.SetBinding(NativeTabBar.ItemsProperty,
                new Binding(nameof(MainPageViewModel.TabItems)));
            native.SetBinding(NativeTabBar.SelectedIndexProperty,
                new Binding(nameof(MainPageViewModel.SelectedTabIndex),
                            BindingMode.TwoWay));
            tabBar = native;
        }
        else
        {
            // iOS < 26 – use the MAUI glass tab bar.
            tabBar = CreateMauiGlassTabBar();
        }
#else
        // Android (and any other platform) – pure MAUI glass tab bar.
        tabBar = CreateMauiGlassTabBar();
#endif

        TabBarContainer.Content = tabBar;
    }

    private static MauiGlassTabBar CreateMauiGlassTabBar()
    {
        var glass = new MauiGlassTabBar();
        glass.SetBinding(MauiGlassTabBar.ItemsProperty,
            new Binding(nameof(MainPageViewModel.TabItems)));
        glass.SetBinding(MauiGlassTabBar.SelectedIndexProperty,
            new Binding(nameof(MainPageViewModel.SelectedTabIndex),
                        BindingMode.TwoWay));

        // ── Customising colours ──────────────────────────────────────────────
        // You can bind or set AccentColor to any colour from your MAUI resources:
        //
        //   glass.AccentColor = (Color)Application.Current!.Resources["Primary"];
        //
        // Or set it directly:
        //
        //   glass.AccentColor = Colors.DeepPink;
        //
        // The default is iOS system blue (#007AFF).

        return glass;
    }
}