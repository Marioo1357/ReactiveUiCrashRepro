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
/// The button renders icon on top with text below (vertical layout), matching
/// the tab bar item style.
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
    /// The button uses a vertical layout with the icon on top and text below,
    /// matching tab bar items.  On iOS 26+ <c>FilledButtonConfiguration</c>
    /// causes the system to apply the Liquid Glass material automatically.
    /// </para>
    /// </summary>
    private static void ConfigureAppearance(UIButton button)
    {
        var config = UIButtonConfiguration.FilledButtonConfiguration;
        config.CornerStyle = UIButtonConfigurationCornerStyle.Large;

        // Icon on top, text below (vertical layout matching tab bar items).
        config.ImagePlacement = NSDirectionalRectEdge.Top;
        config.ImagePadding = 4;

        // Compact padding for the pill shape.
        config.ContentInsets = new NSDirectionalEdgeInsets(8, 12, 8, 12);

        // Smaller subtitle-like text.
        config.TitleAlignment = UIButtonConfigurationTitleAlignment.Center;

        // Use preferred symbol configuration for consistent icon sizing.
        config.PreferredSymbolConfigurationForImage =
            UIImageSymbolConfiguration.Create(UIFontDescriptor.PreferredCaption1.PointSize);

        button.Configuration = config;
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

            // Use a smaller font for the label below the icon.
            var attributes = new UIStringAttributes
            {
                Font = UIFont.SystemFontOfSize(10),
            };
            config.TitleTextAttributesTransformer = original =>
            {
                original.Font = UIFont.SystemFontOfSize(10);
                return original;
            };

            PlatformView.Configuration = config;
        }
    }

    /// <summary>
    /// Updates the button icon.
    /// <para>
    /// Icon resolution order:
    /// <list type="number">
    ///   <item>SF Symbol (<c>UIImage.GetSystemImage</c>) – e.g. "plus".</item>
    ///   <item>Bundled image (<c>UIImage.FromFile</c>) – e.g. "my_icon".
    ///         Set as a template image for automatic tint colour support.</item>
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
            var image = UIImage.GetSystemImage(virtualView.Icon);

            if (image == null)
            {
                // Bundled image — render as template so it picks up the tint colour.
                image = UIImage.FromFile(virtualView.Icon)?
                    .ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            }

            config.Image = image;
        }
        else
        {
            config.Image = null;
        }

        PlatformView.Configuration = config;
    }
}
