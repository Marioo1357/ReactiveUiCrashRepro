using Android.Views;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Navigation;

namespace ReactiveUiCrashRepro.Controls;

public partial class NativeTabBarHandler
{
    // ── Handler lifecycle ────────────────────────────────────────────────────

    protected override BottomNavigationView CreatePlatformView()
    {
        var bottomNav = new BottomNavigationView(Context!);
        ConfigureAppearance(bottomNav);

        _listener = new BottomNavListener(this);
        bottomNav.SetOnItemSelectedListener(_listener);

        return bottomNav;
    }

    protected override void DisconnectHandler(BottomNavigationView platformView)
    {
        platformView.SetOnItemSelectedListener(null);
        _listener = null;
        base.DisconnectHandler(platformView);
    }

    // ── Appearance ───────────────────────────────────────────────────────────

    private BottomNavListener? _listener;

    private static void ConfigureAppearance(BottomNavigationView bottomNav)
    {
        // Frosted-glass look: semi-transparent surface colour + elevation shadow.
        // Android does not expose a per-view "blur behind" API without a 3rd-party
        // library.  The semi-transparent tint combined with Material elevation
        // creates an appearance similar to frosted glass, which is the standard
        // native pattern on Android (used by Google's own apps).
        bottomNav.SetBackgroundColor(Android.Graphics.Color.Argb(230, 249, 249, 249));
        bottomNav.Elevation = 16f;   // dp – casts Material shadow
    }

    // ── Property updaters ────────────────────────────────────────────────────

    private void UpdateTabItems(NativeTabBar virtualView)
    {
        var items = virtualView.Items;
        var menu = PlatformView.Menu;
        if (menu == null) return;

        menu.Clear();

        if (items == null || items.Count == 0) return;

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var menuItem = menu.Add(0, i, i, item.Title);

            // Resolve icon from the app's drawable resources.
            if (!string.IsNullOrEmpty(item.Icon)
                && Context?.Resources != null
                && Context.PackageName != null)
            {
                int resId = Context.Resources.GetIdentifier(
                    item.Icon, "drawable", Context.PackageName);
                if (resId != 0)
                    menuItem?.SetIcon(resId);
            }

            // Badge support (Material Design badges).
            if (item.BadgeCount > 0)
            {
                var badge = PlatformView.GetOrCreateBadge(i);
                badge.SetVisible(true);
                badge.Number = item.BadgeCount;
            }
        }

        UpdateSelectedIndex(virtualView);
    }

    private void UpdateSelectedIndex(NativeTabBar virtualView)
    {
        var menu = PlatformView.Menu;
        var index = virtualView.SelectedIndex;
        if (menu != null && index >= 0 && index < menu.Size())
        {
            var item = menu.GetItem(index);
            if (item != null)
                PlatformView.SelectedItemId = item.ItemId;
        }
    }

    // ── Selection listener ───────────────────────────────────────────────────

    private sealed class BottomNavListener
        : Java.Lang.Object, NavigationBarView.IOnItemSelectedListener
    {
        private readonly NativeTabBarHandler _handler;

        public BottomNavListener(NativeTabBarHandler handler) => _handler = handler;

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            var menu = _handler.PlatformView.Menu;
            if (menu == null) return true;

            for (int i = 0; i < menu.Size(); i++)
            {
                if (menu.GetItem(i)?.ItemId == item.ItemId)
                {
                    _handler.VirtualView?.NotifyTabSelected(i);
                    return true;
                }
            }

            return true;
        }
    }
}
