using UIKit;

namespace ReactiveUiCrashRepro.Controls;

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
