using System.Windows.Input;

namespace ReactiveUiCrashRepro.Controls;

/// <summary>
/// A self-contained navigation bar control that automatically picks the best platform
/// implementation at runtime:
/// <list type="bullet">
///   <item><b>iOS 26+</b> – <see cref="NativeNavigationBar"/> (native <c>UINavigationBar</c>
///         with automatic Liquid Glass).</item>
///   <item><b>iOS &lt; 26</b> – <see cref="MauiGlassNavigationBar"/> (pure MAUI frosted-glass
///         look-alike).</item>
///   <item><b>Android</b> – <see cref="MauiGlassNavigationBar"/> (100 % MAUI frosted-glass
///         appearance).</item>
/// </list>
/// <para>
/// Consumers only need to set <see cref="Title"/>, configure the back button, and optionally
/// set <see cref="Action1Icon"/>/<see cref="Action2Icon"/> with their commands.
/// The page does not need to know which implementation is used internally.
/// </para>
/// <para>
/// Style properties (<see cref="AccentColor"/>, <see cref="TextColor"/>) are forwarded
/// to the MAUI glass navigation bar.  On iOS 26+ the native <c>UINavigationBar</c> handles
/// its own appearance via Liquid Glass.
/// </para>
/// </summary>
public class NavigationBar : ContentView
{
    // ── Bindable properties ─────────────────────────────────────────────────

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(NavigationBar),
            defaultValue: string.Empty);

    public static readonly BindableProperty ShowBackButtonProperty =
        BindableProperty.Create(
            nameof(ShowBackButton),
            typeof(bool),
            typeof(NavigationBar),
            defaultValue: true);

    public static readonly BindableProperty BackCommandProperty =
        BindableProperty.Create(
            nameof(BackCommand),
            typeof(ICommand),
            typeof(NavigationBar),
            defaultValue: null);

    public static readonly BindableProperty Action1IconProperty =
        BindableProperty.Create(
            nameof(Action1Icon),
            typeof(string),
            typeof(NavigationBar),
            defaultValue: null);

    public static readonly BindableProperty Action1CommandProperty =
        BindableProperty.Create(
            nameof(Action1Command),
            typeof(ICommand),
            typeof(NavigationBar),
            defaultValue: null);

    public static readonly BindableProperty Action2IconProperty =
        BindableProperty.Create(
            nameof(Action2Icon),
            typeof(string),
            typeof(NavigationBar),
            defaultValue: null);

    public static readonly BindableProperty Action2CommandProperty =
        BindableProperty.Create(
            nameof(Action2Command),
            typeof(ICommand),
            typeof(NavigationBar),
            defaultValue: null);

    public static readonly BindableProperty AccentColorProperty =
        BindableProperty.Create(
            nameof(AccentColor),
            typeof(Color),
            typeof(NavigationBar),
            defaultValue: Color.FromArgb("#007AFF"));

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(NavigationBar),
            defaultValue: null);

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>Page title displayed in the centre of the bar.</summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>Whether the back button is visible on the left side.</summary>
    public bool ShowBackButton
    {
        get => (bool)GetValue(ShowBackButtonProperty);
        set => SetValue(ShowBackButtonProperty, value);
    }

    /// <summary>Command executed when the back button is tapped.</summary>
    public ICommand? BackCommand
    {
        get => (ICommand?)GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }

    /// <summary>
    /// Icon for the first action button on the right side.
    /// On iOS this is an SF Symbol name; on Android a drawable resource name;
    /// for MAUI glass an image filename.
    /// </summary>
    public string? Action1Icon
    {
        get => (string?)GetValue(Action1IconProperty);
        set => SetValue(Action1IconProperty, value);
    }

    /// <summary>Command executed when action button 1 is tapped.</summary>
    public ICommand? Action1Command
    {
        get => (ICommand?)GetValue(Action1CommandProperty);
        set => SetValue(Action1CommandProperty, value);
    }

    /// <summary>
    /// Icon for the second action button on the right side.
    /// On iOS this is an SF Symbol name; on Android a drawable resource name;
    /// for MAUI glass an image filename.
    /// </summary>
    public string? Action2Icon
    {
        get => (string?)GetValue(Action2IconProperty);
        set => SetValue(Action2IconProperty, value);
    }

    /// <summary>Command executed when action button 2 is tapped.</summary>
    public ICommand? Action2Command
    {
        get => (ICommand?)GetValue(Action2CommandProperty);
        set => SetValue(Action2CommandProperty, value);
    }

    /// <summary>
    /// Accent colour for the MAUI glass implementation.
    /// Defaults to iOS system blue (<c>#007AFF</c>).
    /// </summary>
    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    /// <summary>
    /// Title text colour for the MAUI glass implementation.
    /// When <c>null</c> (the default) a theme-appropriate colour is used.
    /// </summary>
    public Color? TextColor
    {
        get => (Color?)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    /// <summary>Raised when the back button is tapped.</summary>
    public event EventHandler? BackButtonClicked;

    /// <summary>Raised when action button 1 is tapped.</summary>
    public event EventHandler? Action1Clicked;

    /// <summary>Raised when action button 2 is tapped.</summary>
    public event EventHandler? Action2Clicked;

    // ── Constructor ──────────────────────────────────────────────────────────

    public NavigationBar()
    {
        Content = CreateInnerNavigationBar();
    }

    // ── Implementation selection ─────────────────────────────────────────────

    private View CreateInnerNavigationBar()
    {
#if IOS
        if (OperatingSystem.IsIOSVersionAtLeast(26))
            return CreateNativeNavigationBar();
#endif
        return CreateMauiGlassNavigationBar();
    }

#if IOS
    private NativeNavigationBar CreateNativeNavigationBar()
    {
        var native = new NativeNavigationBar();
        native.SetBinding(NativeNavigationBar.TitleProperty,
            new Binding(nameof(Title), source: this));
        native.SetBinding(NativeNavigationBar.ShowBackButtonProperty,
            new Binding(nameof(ShowBackButton), source: this));
        native.SetBinding(NativeNavigationBar.BackCommandProperty,
            new Binding(nameof(BackCommand), source: this));
        native.SetBinding(NativeNavigationBar.Action1IconProperty,
            new Binding(nameof(Action1Icon), source: this));
        native.SetBinding(NativeNavigationBar.Action1CommandProperty,
            new Binding(nameof(Action1Command), source: this));
        native.SetBinding(NativeNavigationBar.Action2IconProperty,
            new Binding(nameof(Action2Icon), source: this));
        native.SetBinding(NativeNavigationBar.Action2CommandProperty,
            new Binding(nameof(Action2Command), source: this));

        native.BackButtonClicked += (_, e) => BackButtonClicked?.Invoke(this, e);
        native.Action1Clicked += (_, e) => Action1Clicked?.Invoke(this, e);
        native.Action2Clicked += (_, e) => Action2Clicked?.Invoke(this, e);
        return native;
    }
#endif

    private MauiGlassNavigationBar CreateMauiGlassNavigationBar()
    {
        var glass = new MauiGlassNavigationBar();

        // Data bindings
        glass.SetBinding(MauiGlassNavigationBar.TitleProperty,
            new Binding(nameof(Title), source: this));
        glass.SetBinding(MauiGlassNavigationBar.ShowBackButtonProperty,
            new Binding(nameof(ShowBackButton), source: this));
        glass.SetBinding(MauiGlassNavigationBar.BackCommandProperty,
            new Binding(nameof(BackCommand), source: this));
        glass.SetBinding(MauiGlassNavigationBar.Action1IconProperty,
            new Binding(nameof(Action1Icon), source: this));
        glass.SetBinding(MauiGlassNavigationBar.Action1CommandProperty,
            new Binding(nameof(Action1Command), source: this));
        glass.SetBinding(MauiGlassNavigationBar.Action2IconProperty,
            new Binding(nameof(Action2Icon), source: this));
        glass.SetBinding(MauiGlassNavigationBar.Action2CommandProperty,
            new Binding(nameof(Action2Command), source: this));

        // Style bindings
        glass.SetBinding(MauiGlassNavigationBar.AccentColorProperty,
            new Binding(nameof(AccentColor), source: this));
        glass.SetBinding(MauiGlassNavigationBar.TextColorProperty,
            new Binding(nameof(TextColor), source: this));

        glass.BackButtonClicked += (_, e) => BackButtonClicked?.Invoke(this, e);
        glass.Action1Clicked += (_, e) => Action1Clicked?.Invoke(this, e);
        glass.Action2Clicked += (_, e) => Action2Clicked?.Invoke(this, e);
        return glass;
    }
}
