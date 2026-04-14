using UIKit;

namespace ReactiveUiCrashRepro.Controls;

/// <summary>
/// iOS handler for <see cref="NativeActionButton"/>.
/// <para>
/// Uses <c>UIButtonConfiguration</c> (iOS 15+) for the modern button appearance.
/// On iOS 26+ the system automatically applies the Liquid Glass material to configured
/// buttons, matching the system-wide Liquid Glass treatment.
/// </para>
/// <para>
/// <b>Icon resolution:</b>
/// <list type="number">
///   <item>SF Symbol (<c>UIImage.GetSystemImage</c>) – e.g. "plus".</item>
///   <item>Bundled image (<c>UIImage.FromFile</c>) – works for files placed
///         in <c>Resources/Images/</c>.  Use the filename <b>without extension</b>.</item>
/// </list>
/// </para>
/// </summary>
public partial class NativeActionButtonHandler
{
    // ── Handler lifecycle ────────────────────────────────────────────────────

    public static UIButton GetUIButton(NativeActionButton virtualView)
    {
        var handler = new NativeActionButtonHandler();
        return handler.CreatePlatformView();
    }

    protected override UIButton CreatePlatformView()
    {
        var button = new UIButton(UIButtonType.System);
        ConfigureAppearance(button);

        button.TouchUpInside += OnButtonTapped;
        return button;
    }

    protected override void DisconnectHandler(UIButton platformView)
    {
        platformView.TouchUpInside -= OnButtonTapped;
        base.DisconnectHandler(platformView);
    }

    // ── Appearance ───────────────────────────────────────────────────────────

    /// <summary>
    /// Configures the UIButton appearance using UIButtonConfiguration (iOS 15+).
    /// <para>
    /// On iOS 26+ <c>FilledButtonConfiguration</c> causes the system to
    /// apply the Liquid Glass material automatically.  On earlier versions you
    /// get a standard filled button appearance.
    /// </para>
    /// </summary>
    private static void ConfigureAppearance(UIButton button)
    {
        var config = UIButtonConfiguration.PlainButtonConfiguration;
        config.Background.BackgroundColor = UIColor.Clear;
        config.CornerStyle = UIButtonConfigurationCornerStyle.Capsule;
        config.ImagePlacement = NSDirectionalRectEdge.Top;
        config.ImagePadding = 4;
        config.TitleAlignment = UIButtonConfigurationTitleAlignment.Center;
        button.Configuration = config;
        
        // var blur = UIBlurEffect.FromStyle(UIBlurEffectStyle.SystemUltraThinMaterial);
        // var blurView = new UIVisualEffectView(blur);
        //
        // blurView.Frame = button.Bounds;
        // blurView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
        //
        // button.InsertSubview(blurView, 0);
        //
        button.Layer.CornerRadius = 24;
        button.ClipsToBounds = true;
    }

    // ── Event handling ───────────────────────────────────────────────────────

    private void OnButtonTapped(object? sender, EventArgs e)
    {
        VirtualView?.NotifyClicked();
    }

    // ── Property updaters ────────────────────────────────────────────────────

    private void UpdateText(NativeActionButton virtualView)
    {
        var config = PlatformView.Configuration;
        if (config != null)
        {
            config.Title = virtualView.Text;
            PlatformView.Configuration = config;
        }
    }

    /// <summary>
    /// Updates the button icon.
    /// <para>
    /// Icon resolution order:
    /// <list type="number">
    ///   <item>SF Symbol (<c>UIImage.GetSystemImage</c>) – e.g. "plus".</item>
    ///   <item>Bundled image (<c>UIImage.FromFile</c>) – e.g. "my_icon".</item>
    /// </list>
    /// </para>
    /// </summary>
    private void UpdateIcon(NativeActionButton virtualView)
    {
        var config = PlatformView.Configuration;
        if (config == null) return;

        if (!string.IsNullOrEmpty(virtualView.Icon))
        {
            // SF Symbol (iOS 13+) first; fall back to a bundled image file.
            var image = UIImage.GetSystemImage(virtualView.Icon)
                        ?? UIImage.FromFile(virtualView.Icon);
            config.Image = image;
            config.ImagePadding = 8;
        }
        else
        {
            config.Image = null;
        }

        PlatformView.Configuration = config;
    }
}
