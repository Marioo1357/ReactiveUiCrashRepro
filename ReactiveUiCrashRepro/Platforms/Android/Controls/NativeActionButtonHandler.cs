using Android.Graphics.Drawables;
using Android.Views;

namespace ReactiveUiCrashRepro.Controls;

/// <summary>
/// Android handler for <see cref="NativeActionButton"/>.
/// <para>
/// Renders a vertical layout (icon on top, text below) inside a frosted-glass
/// styled container, matching the tab bar item appearance.
/// </para>
/// </summary>
public partial class NativeActionButtonHandler
{
    // ── Handler lifecycle ────────────────────────────────────────────────────

    protected override Android.Widget.Button CreatePlatformView()
    {
        // We create a dummy Button as the base (required by the handler type).
        // The actual content is a vertical LinearLayout added as an overlay.
        var button = new Android.Widget.Button(Context!);
        ConfigureAppearance(button);

        button.Click += OnButtonClicked;
        return button;
    }

    protected override void ConnectHandler(Android.Widget.Button platformView)
    {
        base.ConnectHandler(platformView);

        // Build the vertical layout (icon + text) inside the button.
        // Android's Button extends TextView, so we hide its text and overlay a layout.
        platformView.Text = string.Empty;
        platformView.SetMinimumWidth(0);
        platformView.SetMinWidth(0);
        platformView.SetMinimumHeight(0);
        platformView.SetMinHeight(0);

        // Set gravity so internal compound drawables are centered.
        platformView.Gravity = GravityFlags.Center;
    }

    protected override void DisconnectHandler(Android.Widget.Button platformView)
    {
        platformView.Click -= OnButtonClicked;
        base.DisconnectHandler(platformView);
    }

    // ── Appearance ───────────────────────────────────────────────────────────

    /// <summary>
    /// Configures the Button with a semi-transparent frosted-glass appearance
    /// and vertical icon-above-text layout using compound drawables.
    /// </summary>
    private static void ConfigureAppearance(Android.Widget.Button button)
    {
        // Frosted-glass look: semi-transparent background + rounded corners + elevation.
        var drawable = new GradientDrawable();
        drawable.SetColor(Android.Graphics.Color.Argb(230, 249, 249, 249));
        drawable.SetCornerRadius(44f);
        button.Background = drawable;
        button.Elevation = 8f;
        button.SetTextColor(Android.Graphics.Color.Argb(255, 0, 122, 255));
        button.SetAllCaps(false);

        // Smaller text size to match tab bar labels.
        var density = button.Context?.Resources?.DisplayMetrics?.Density ?? 1f;
        button.SetTextSize(Android.Util.ComplexUnitType.Sp, 10);

        // Compact padding for the pill shape.
        int hPad = (int)(12 * density);
        int vPad = (int)(8 * density);
        button.SetPadding(hPad, vPad, hPad, vPad);

        // Center text under icon.
        button.Gravity = GravityFlags.Center;
    }

    // ── Event handling ───────────────────────────────────────────────────────

    private void OnButtonClicked(object? sender, EventArgs e)
    {
        VirtualView?.NotifyClicked();
    }

    // ── Property updaters ────────────────────────────────────────────────────

    private void UpdateText(NativeActionButton virtualView)
    {
        PlatformView.Text = virtualView.Text;
    }

    /// <summary>
    /// Updates the button icon from an Android drawable resource.
    /// <para>
    /// The icon is placed <b>above</b> the text (top compound drawable)
    /// to match the tab bar item layout.
    /// </para>
    /// </summary>
    private void UpdateIcon(NativeActionButton virtualView)
    {
        if (string.IsNullOrEmpty(virtualView.Icon)
            || Context?.Resources == null
            || Context.PackageName == null)
        {
            PlatformView.SetCompoundDrawablesWithIntrinsicBounds(null, null, null, null);
            return;
        }

        int resId = Context.Resources.GetIdentifier(
            virtualView.Icon, "drawable", Context.PackageName);

        if (resId != 0)
        {
            var icon = Context.Resources.GetDrawable(resId, Context.Theme);

            // Scale icon to a reasonable size (22dp × 22dp).
            var density = Context.Resources.DisplayMetrics?.Density ?? 1f;
            int iconSize = (int)(22 * density);
            icon?.SetBounds(0, 0, iconSize, iconSize);

            // Top position: icon above text.
            PlatformView.SetCompoundDrawablesRelative(null, icon, null, null);
            PlatformView.CompoundDrawablePadding = (int)(2 * density);
        }
    }
}
