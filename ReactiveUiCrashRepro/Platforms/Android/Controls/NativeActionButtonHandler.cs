using Android.Graphics.Drawables;

namespace ReactiveUiCrashRepro.Controls;

/// <summary>
/// Android handler for <see cref="NativeActionButton"/>.
/// <para>
/// Renders a styled <c>Button</c> with a semi-transparent frosted-glass surface colour
/// and elevation shadow, consistent with the tab bar's frosted-glass treatment.
/// </para>
/// </summary>
public partial class NativeActionButtonHandler
{
    // ── Handler lifecycle ────────────────────────────────────────────────────

    protected override Android.Widget.Button CreatePlatformView()
    {
        var button = new Android.Widget.Button(Context!);
        ConfigureAppearance(button);

        button.Click += OnButtonClicked;
        return button;
    }

    protected override void DisconnectHandler(Android.Widget.Button platformView)
    {
        platformView.Click -= OnButtonClicked;
        base.DisconnectHandler(platformView);
    }

    // ── Appearance ───────────────────────────────────────────────────────────

    /// <summary>
    /// Configures the Button with a semi-transparent frosted-glass appearance.
    /// <para>
    /// Android does not expose a per-view "blur behind" API without a 3rd-party
    /// library.  The semi-transparent tint combined with Material elevation
    /// creates an appearance similar to frosted glass, which is the standard
    /// native pattern on Android (used by Google's own apps).
    /// </para>
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

        // Padding for comfortable touch target.
        var density = button.Context?.Resources?.DisplayMetrics?.Density ?? 1f;
        int hPad = (int)(24 * density);
        int vPad = (int)(12 * density);
        button.SetPadding(hPad, vPad, hPad, vPad);
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
    /// Set <see cref="NativeActionButton.Icon"/> to the drawable resource name
    /// (e.g. "ic_add") located in <c>Resources/drawable</c>.
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
            PlatformView.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);
            PlatformView.CompoundDrawablePadding =
                (int)(8 * (Context.Resources.DisplayMetrics?.Density ?? 1f));
        }
    }
}
