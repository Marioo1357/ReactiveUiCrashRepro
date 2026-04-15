using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;

namespace ReactiveUiCrashRepro.Controls;

/// <summary>
/// Android handler for <see cref="NativeNavigationBar"/>.
/// <para>
/// Renders a styled <c>LinearLayout</c> toolbar with a semi-transparent frosted-glass
/// surface colour and elevation shadow, consistent with the tab bar and action button
/// frosted-glass treatment.
/// </para>
/// </summary>
public partial class NativeNavigationBarHandler
{
    private Android.Widget.ImageButton? _backButton;
    private TextView? _titleView;
    private Android.Widget.ImageButton? _action1Button;
    private Android.Widget.ImageButton? _action2Button;

    // ── Handler lifecycle ────────────────────────────────────────────────────

    protected override LinearLayout CreatePlatformView()
    {
        var layout = new LinearLayout(Context!)
        {
            Orientation = Android.Widget.Orientation.Horizontal,
        };

        ConfigureAppearance(layout);
        BuildLayout(layout);

        return layout;
    }

    protected override void DisconnectHandler(LinearLayout platformView)
    {
        if (_backButton != null) _backButton.Click -= OnBackClicked;
        if (_action1Button != null) _action1Button.Click -= OnAction1Clicked;
        if (_action2Button != null) _action2Button.Click -= OnAction2Clicked;
        _backButton = null;
        _titleView = null;
        _action1Button = null;
        _action2Button = null;
        base.DisconnectHandler(platformView);
    }

    // ── Appearance ───────────────────────────────────────────────────────────

    private static void ConfigureAppearance(LinearLayout layout)
    {
        // Frosted-glass look: semi-transparent surface colour + elevation shadow.
        var drawable = new GradientDrawable();
        drawable.SetColor(Android.Graphics.Color.Argb(230, 249, 249, 249));
        drawable.SetCornerRadius(44f);
        layout.Background = drawable;
        layout.Elevation = 8f;

        var density = layout.Context?.Resources?.DisplayMetrics?.Density ?? 1f;
        int hPad = (int)(16 * density);
        int vPad = (int)(8 * density);
        layout.SetPadding(hPad, vPad, hPad, vPad);
        layout.SetGravity(GravityFlags.CenterVertical);
    }

    private void BuildLayout(LinearLayout layout)
    {
        var density = Context?.Resources?.DisplayMetrics?.Density ?? 1f;
        int buttonSize = (int)(44 * density);

        // ── Back button ─────────────────────────────────────────────────────
        _backButton = new Android.Widget.ImageButton(Context!);
        _backButton.SetBackgroundColor(Android.Graphics.Color.Transparent);
        _backButton.SetImageResource(Android.Resource.Drawable.IcMenuRevert);
        _backButton.SetColorFilter(Android.Graphics.Color.Argb(255, 0, 122, 255));
        _backButton.Click += OnBackClicked;
        layout.AddView(_backButton, new LinearLayout.LayoutParams(buttonSize, buttonSize));

        // ── Title ───────────────────────────────────────────────────────────
        _titleView = new TextView(Context!);
        _titleView.Gravity = GravityFlags.Center;
        _titleView.SetTextColor(Android.Graphics.Color.Argb(255, 0, 0, 0));
        _titleView.TextSize = 18f;
        _titleView.SetTypeface(null, TypefaceStyle.Bold);
        layout.AddView(_titleView, new LinearLayout.LayoutParams(
            0, ViewGroup.LayoutParams.WrapContent, 1f)
        {
            Gravity = GravityFlags.CenterVertical
        });

        // ── Action button 1 ────────────────────────────────────────────────
        _action1Button = new Android.Widget.ImageButton(Context!);
        _action1Button.SetBackgroundColor(Android.Graphics.Color.Transparent);
        _action1Button.SetColorFilter(Android.Graphics.Color.Argb(255, 0, 122, 255));
        _action1Button.Click += OnAction1Clicked;
        _action1Button.Visibility = ViewStates.Gone;
        layout.AddView(_action1Button, new LinearLayout.LayoutParams(buttonSize, buttonSize));

        // ── Action button 2 ────────────────────────────────────────────────
        _action2Button = new Android.Widget.ImageButton(Context!);
        _action2Button.SetBackgroundColor(Android.Graphics.Color.Transparent);
        _action2Button.SetColorFilter(Android.Graphics.Color.Argb(255, 0, 122, 255));
        _action2Button.Click += OnAction2Clicked;
        _action2Button.Visibility = ViewStates.Gone;
        layout.AddView(_action2Button, new LinearLayout.LayoutParams(buttonSize, buttonSize));
    }

    // ── Event handlers ──────────────────────────────────────────────────────

    private void OnBackClicked(object? sender, EventArgs e) =>
        VirtualView?.NotifyBackButtonClicked();

    private void OnAction1Clicked(object? sender, EventArgs e) =>
        VirtualView?.NotifyAction1Clicked();

    private void OnAction2Clicked(object? sender, EventArgs e) =>
        VirtualView?.NotifyAction2Clicked();

    // ── Property updaters ────────────────────────────────────────────────────

    private void UpdateTitle(NativeNavigationBar virtualView)
    {
        if (_titleView != null)
            _titleView.Text = virtualView.Title;
    }

    private void UpdateShowBackButton(NativeNavigationBar virtualView)
    {
        if (_backButton != null)
            _backButton.Visibility = virtualView.ShowBackButton
                ? ViewStates.Visible
                : ViewStates.Gone;
    }

    private void UpdateActionIcons(NativeNavigationBar virtualView)
    {
        UpdateActionButton(_action1Button, virtualView.Action1Icon);
        UpdateActionButton(_action2Button, virtualView.Action2Icon);
    }

    private void UpdateActionButton(Android.Widget.ImageButton? button, string? icon)
    {
        if (button == null) return;

        if (string.IsNullOrEmpty(icon)
            || Context?.Resources == null
            || Context.PackageName == null)
        {
            button.Visibility = ViewStates.Gone;
            return;
        }

        int resId = Context.Resources.GetIdentifier(
            icon, "drawable", Context.PackageName);

        if (resId != 0)
        {
            button.SetImageResource(resId);
            button.Visibility = ViewStates.Visible;
        }
        else
        {
            button.Visibility = ViewStates.Gone;
        }
    }
}
