using UIKit;

namespace ReactiveUiCrashRepro.Controls;

public partial class NativeNavigationBarHandler
{
    private UINavigationItem? _navItem;

    // ── Handler lifecycle ────────────────────────────────────────────────────

    protected override UINavigationBar CreatePlatformView()
    {
        var navBar = new UINavigationBar();
        ConfigureAppearance(navBar);

        _navItem = new UINavigationItem(VirtualView?.Title ?? "");
        navBar.SetItems(new[] { _navItem }, false);

        return navBar;
    }

    protected override void ConnectHandler(UINavigationBar platformView)
    {
        base.ConnectHandler(platformView);
        UpdateNavigationItem();
    }

    protected override void DisconnectHandler(UINavigationBar platformView)
    {
        _navItem = null;
        base.DisconnectHandler(platformView);
    }

    // ── Appearance ───────────────────────────────────────────────────────────

    private static void ConfigureAppearance(UINavigationBar navBar)
    {
        // Translucent = true enables the system blur / glass treatment.
        // On iOS 26+ the system automatically applies the Liquid Glass material
        // when ConfigureWithDefaultBackground() is used.
        navBar.Translucent = true;

        var appearance = new UINavigationBarAppearance();
        appearance.ConfigureWithDefaultBackground();   // system blur / Liquid Glass

        navBar.StandardAppearance = appearance;

        // ScrollEdgeAppearance available from iOS 15.
        if (OperatingSystem.IsIOSVersionAtLeast(15))
            navBar.ScrollEdgeAppearance = appearance;
    }

    // ── Property updaters ────────────────────────────────────────────────────

    private void UpdateTitle(NativeNavigationBar virtualView)
    {
        if (_navItem != null)
            _navItem.Title = virtualView.Title;
    }

    private void UpdateShowBackButton(NativeNavigationBar virtualView)
    {
        UpdateNavigationItem();
    }

    private void UpdateActionIcons(NativeNavigationBar virtualView)
    {
        UpdateNavigationItem();
    }

    private void UpdateNavigationItem()
    {
        if (_navItem == null || VirtualView == null) return;

        // ── Back button ─────────────────────────────────────────────────────
        if (VirtualView.ShowBackButton)
        {
            var backImage = UIImage.GetSystemImage("chevron.backward");
            var backButton = new UIBarButtonItem(
                backImage,
                UIBarButtonItemStyle.Plain,
                (_, _) => VirtualView.NotifyBackButtonClicked());
            _navItem.LeftBarButtonItems = new[] { backButton };
        }
        else
        {
            _navItem.LeftBarButtonItems = Array.Empty<UIBarButtonItem>();
        }

        // ── Action buttons ──────────────────────────────────────────────────
        // Note: rightBarButtonItems are displayed right-to-left, so Action2
        // (right-most) is added first.
        var rightItems = new List<UIBarButtonItem>();

        if (!string.IsNullOrEmpty(VirtualView.Action2Icon))
        {
            var image = UIImage.GetSystemImage(VirtualView.Action2Icon)
                        ?? UIImage.FromFile(VirtualView.Action2Icon);
            if (image != null)
            {
                rightItems.Add(new UIBarButtonItem(
                    image,
                    UIBarButtonItemStyle.Plain,
                    (_, _) => VirtualView.NotifyAction2Clicked()));
            }
        }

        if (!string.IsNullOrEmpty(VirtualView.Action1Icon))
        {
            var image = UIImage.GetSystemImage(VirtualView.Action1Icon)
                        ?? UIImage.FromFile(VirtualView.Action1Icon);
            if (image != null)
            {
                rightItems.Add(new UIBarButtonItem(
                    image,
                    UIBarButtonItemStyle.Plain,
                    (_, _) => VirtualView.NotifyAction1Clicked()));
            }
        }

        _navItem.RightBarButtonItems = rightItems.ToArray();
    }
}
