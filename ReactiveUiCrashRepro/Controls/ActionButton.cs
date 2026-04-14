using System.Windows.Input;

namespace ReactiveUiCrashRepro.Controls;

/// <summary>
/// A self-contained action button control that automatically picks the best platform
/// implementation at runtime:
/// <list type="bullet">
///   <item><b>iOS 26+</b> – <see cref="NativeActionButton"/> (native <c>UIButton</c> with
///         automatic Liquid Glass).</item>
///   <item><b>iOS &lt; 26</b> – <see cref="MauiGlassActionButton"/> (pure MAUI frosted-glass
///         look-alike).</item>
///   <item><b>Android</b> – <see cref="MauiGlassActionButton"/> (100 % MAUI frosted-glass
///         appearance).</item>
/// </list>
/// <para>
/// Consumers only need to set <see cref="Text"/>, optionally <see cref="Icon"/> or
/// <see cref="IconGeometry"/>, and bind <see cref="Command"/>.
/// The page does not need to know which implementation is used internally.
/// </para>
/// <para>
/// Style properties (<see cref="AccentColor"/>, <see cref="TextColor"/>) are forwarded
/// to the MAUI glass action button.  On iOS 26+ the native <c>UIButton</c> handles its
/// own appearance via Liquid Glass.
/// </para>
/// </summary>
public class ActionButton : ContentView
{
    // ── Bindable properties ─────────────────────────────────────────────────

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(ActionButton),
            defaultValue: string.Empty);

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(
            nameof(Icon),
            typeof(string),
            typeof(ActionButton),
            defaultValue: null);

    public static readonly BindableProperty AccentColorProperty =
        BindableProperty.Create(
            nameof(AccentColor),
            typeof(Color),
            typeof(ActionButton),
            defaultValue: Color.FromArgb("#007AFF"));

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(ActionButton),
            defaultValue: null);

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(
            nameof(Command),
            typeof(ICommand),
            typeof(ActionButton),
            defaultValue: null);

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(
            nameof(CommandParameter),
            typeof(object),
            typeof(ActionButton),
            defaultValue: null);

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>The button label text.</summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Platform-native icon identifier.
    /// On iOS this is an SF Symbol name (e.g. "plus", "square.and.arrow.up").
    /// On Android this is a drawable resource name (e.g. "ic_add").
    /// Also used by <see cref="MauiGlassActionButton"/> as a fallback filename when
    /// <see cref="IconGeometry"/> and <see cref="MauiIconSource"/> are not set.
    /// </summary>
    public string? Icon
    {
        get => (string?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    /// <summary>
    /// Accent colour used as the default content colour for the MAUI glass implementation.
    /// Defaults to iOS system blue (<c>#007AFF</c>).
    /// </summary>
    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    /// <summary>
    /// Colour for button text and icon in the MAUI glass implementation.
    /// When <c>null</c> (the default) <see cref="AccentColor"/> is used.
    /// </summary>
    public Color? TextColor
    {
        get => (Color?)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    /// <summary>Command executed when the button is tapped.</summary>
    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>Parameter passed to <see cref="Command"/>.</summary>
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>Raised when the user taps the button.</summary>
    public event EventHandler? Clicked;

    // ── Constructor ──────────────────────────────────────────────────────────

    public ActionButton()
    {
        Content = CreateInnerButton();
    }

    // ── Implementation selection ─────────────────────────────────────────────

    private View CreateInnerButton()
    {
#if IOS
        if (OperatingSystem.IsIOSVersionAtLeast(26))
            return CreateNativeActionButton();
#endif
        return CreateMauiGlassActionButton();
    }

#if IOS
    private NativeActionButton CreateNativeActionButton()
    {
        var native = new NativeActionButton();
        native.SetBinding(NativeActionButton.TextProperty,
            new Binding(nameof(Text), source: this));
        native.SetBinding(NativeActionButton.IconProperty,
            new Binding(nameof(Icon), source: this));
        native.SetBinding(NativeActionButton.CommandProperty,
            new Binding(nameof(Command), source: this));
        native.SetBinding(NativeActionButton.CommandParameterProperty,
            new Binding(nameof(CommandParameter), source: this));

        native.Clicked += (_, e) => Clicked?.Invoke(this, e);
        return native;
    }
#endif

    private MauiGlassActionButton CreateMauiGlassActionButton()
    {
        var glass = new MauiGlassActionButton();

        // Data bindings
        glass.SetBinding(MauiGlassActionButton.TextProperty,
            new Binding(nameof(Text), source: this));
        glass.SetBinding(MauiGlassActionButton.IconProperty,
            new Binding(nameof(Icon), source: this));

        // Style bindings
        glass.SetBinding(MauiGlassActionButton.AccentColorProperty,
            new Binding(nameof(AccentColor), source: this));
        glass.SetBinding(MauiGlassActionButton.TextColorProperty,
            new Binding(nameof(TextColor), source: this));

        // Command bindings
        glass.SetBinding(MauiGlassActionButton.CommandProperty,
            new Binding(nameof(Command), source: this));
        glass.SetBinding(MauiGlassActionButton.CommandParameterProperty,
            new Binding(nameof(CommandParameter), source: this));

        glass.Clicked += (_, e) => Clicked?.Invoke(this, e);
        return glass;
    }
}
