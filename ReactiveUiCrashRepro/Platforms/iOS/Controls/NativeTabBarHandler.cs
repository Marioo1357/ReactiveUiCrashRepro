using UIKit;

namespace ReactiveUiCrashRepro.Controls;

/// <summary>
/// iOS handler for <see cref="NativeTabBar"/>.
/// <para>
/// <b>How to add your own icons from MAUI resources:</b>
/// <list type="number">
///   <item>
///     Place an SVG (or PNG) in <c>Resources/Images/</c> – e.g. <c>my_icon.svg</c>.
///     MAUI compiles it into the iOS app bundle as <c>my_icon.png</c>.
///   </item>
///   <item>
///     Set <see cref="TabItem.Icon"/> to the <b>filename without extension</b>:
///     <c>new TabItem { Icon = "my_icon" }</c>.
///     The handler resolves it via <c>UIImage.FromFile</c> (see
///     <see cref="CreateTabBarItem"/>).
///   </item>
///   <item>
///     For SF Symbols (iOS 13+) you can use the symbol name directly:
///     <c>new TabItem { Icon = "house.fill" }</c>.
///   </item>
/// </list>
/// </para>
/// <para>
/// <b>How to customise tint colours from MAUI resources:</b>
/// <code>
/// // In your page code-behind, after obtaining the native UITabBar:
/// //   var uiTabBar = (UITabBar)nativeTabBarControl.Handler!.PlatformView!;
/// //
/// // Set the tint for selected icons:
/// //   var accentColor = (Color)Application.Current!.Resources["Primary"];
/// //   uiTabBar.TintColor = accentColor.ToPlatform();   // converts MAUI Color → UIColor
/// //
/// // Set the unselected item colour:
/// //   uiTabBar.UnselectedItemTintColor = UIColor.SystemGray;
/// //
/// // You can also set the bar tint (overall background tint):
/// //   uiTabBar.BarTintColor = UIColor.FromRGBA(240, 244, 248, 220);
/// </code>
/// </para>
/// </summary>
public partial class NativeTabBarHandler
{
    // ── Handler lifecycle ────────────────────────────────────────────────────

    protected override UITabBar CreatePlatformView()
    {
        var tabBar = new UITabBar();
        ConfigureAppearance(tabBar);

        _delegate = new TabBarDelegate(this);
        tabBar.Delegate = _delegate;

        return tabBar;
    }

    protected override void DisconnectHandler(UITabBar platformView)
    {
        platformView.Delegate = null;
        _delegate = null;
        base.DisconnectHandler(platformView);
    }

    // ── Appearance ───────────────────────────────────────────────────────────

    private TabBarDelegate? _delegate;

    /// <summary>
    /// Configures the UITabBar appearance.
    /// <para>
    /// On iOS 26+ <c>ConfigureWithDefaultBackground()</c> causes the system to
    /// apply the Liquid Glass material automatically.  On earlier versions you
    /// get the standard translucent system blur.
    /// </para>
    /// <para>
    /// <b>To set custom colours</b>, modify this method.  Examples:
    /// <code>
    /// // Selected icon/text tint – pull from MAUI resource:
    /// //   var accent = ((Color)Application.Current!.Resources["Primary"]).ToPlatform();
    /// //   tabBar.TintColor = accent;
    ///
    /// // Bar background tint:
    /// //   tabBar.BarTintColor = UIColor.FromRGBA(240, 244, 248, 220);
    ///
    /// // Unselected item colour:
    /// //   tabBar.UnselectedItemTintColor = UIColor.SystemGray;
    ///
    /// // Per-state text/icon colours via UITabBarItemAppearance:
    /// //   var itemAppearance = new UITabBarItemAppearance();
    /// //   itemAppearance.Selected.TitleTextAttributes = new UIStringAttributes
    /// //       { ForegroundColor = accent };
    /// //   itemAppearance.Normal.TitleTextAttributes = new UIStringAttributes
    /// //       { ForegroundColor = UIColor.SystemGray };
    /// //   appearance.StackedLayoutAppearance = itemAppearance;
    /// </code>
    /// </para>
    /// </summary>
    private static void ConfigureAppearance(UITabBar tabBar)
    {
        // Translucent = true enables the system blur / glass treatment.
        // On iOS 26+ the system automatically applies the Liquid Glass material
        // when ConfigureWithDefaultBackground() is used.
        tabBar.Translucent = true;

        var appearance = new UITabBarAppearance();
        appearance.ConfigureWithDefaultBackground();   // system blur / Liquid Glass

        tabBar.StandardAppearance = appearance;

        // ScrollEdgeAppearance available from iOS 15.
        if (OperatingSystem.IsIOSVersionAtLeast(15))
            tabBar.ScrollEdgeAppearance = appearance;
    }

    // ── Property updaters ────────────────────────────────────────────────────

    private void UpdateTabItems(NativeTabBar virtualView)
    {
        var items = virtualView.Items;
        if (items == null || items.Count == 0)
        {
            PlatformView.SetItems(Array.Empty<UITabBarItem>(), false);
            return;
        }

        var tabBarItems = new UITabBarItem[items.Count];
        for (int i = 0; i < items.Count; i++)
            tabBarItems[i] = CreateTabBarItem(items[i], i);

        PlatformView.SetItems(tabBarItems, false);
        UpdateSelectedIndex(virtualView);
    }

    /// <summary>
    /// Creates a <see cref="UITabBarItem"/> from a <see cref="TabItem"/>.
    /// <para>
    /// Icon resolution order:
    /// <list type="number">
    ///   <item>SF Symbol (<c>UIImage.GetSystemImage</c>) – e.g. "house.fill".</item>
    ///   <item>Bundled image (<c>UIImage.FromFile</c>) – works for files placed
    ///         in <c>Resources/Images/</c> (MAUI bundles them into the iOS app).
    ///         Use the filename <b>without extension</b> (e.g. "tab_home").</item>
    /// </list>
    /// </para>
    /// </summary>
    private static UITabBarItem CreateTabBarItem(TabItem item, int tag)
    {
        UIImage? image = null;
        if (!string.IsNullOrEmpty(item.Icon))
        {
            // SF Symbol (iOS 13+) first; fall back to a bundled image file.
            image = UIImage.GetSystemImage(item.Icon)
                    ?? UIImage.FromFile(item.Icon);
        }

        var tabBarItem = new UITabBarItem(item.Title, image, tag);

        if (item.BadgeCount > 0)
            tabBarItem.BadgeValue = item.BadgeCount.ToString();

        return tabBarItem;
    }

    private void UpdateSelectedIndex(NativeTabBar virtualView)
    {
        var items = PlatformView.Items;
        var index = virtualView.SelectedIndex;
        if (items != null && index >= 0 && index < items.Length)
            PlatformView.SelectedItem = items[index];
    }

    // ── Delegate ─────────────────────────────────────────────────────────────

    private sealed class TabBarDelegate : UITabBarDelegate
    {
        private readonly NativeTabBarHandler _handler;

        public TabBarDelegate(NativeTabBarHandler handler) => _handler = handler;

        public override void ItemSelected(UITabBar tabBar, UITabBarItem item)
        {
            var items = tabBar.Items;
            if (items == null) return;

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == item)
                {
                    _handler.VirtualView?.NotifyTabSelected(i);
                    return;
                }
            }
        }
    }
}
