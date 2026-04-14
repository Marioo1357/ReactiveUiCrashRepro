using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;

namespace ReactiveUiCrashRepro.Controls;

/// <summary>
/// A pure-MAUI action button that mimics the iOS 26 Liquid Glass appearance.
/// <para>
/// Used on <b>Android</b> (always) and on <b>iOS &lt; 26</b> (where the native Liquid
/// Glass material is not available).  The control renders entirely with MAUI
/// components – no platform-specific code is required.
/// </para>
/// <para>
/// Visual design:
/// <list type="bullet">
///   <item>Semi-transparent frosted-glass background with a subtle gradient.</item>
///   <item>Rounded pill container with a thin glass-edge stroke and a soft shadow.</item>
///   <item>Icon (optional) + text laid out in a horizontal stack.</item>
/// </list>
/// </para>
/// <para>
/// <b>Icons</b> – each button can provide icons in several ways
/// (checked in this order):
/// <list type="number">
///   <item><see cref="IconGeometry"/> – SVG path data rendered as a
///         <c>Shapes.Path</c> with full fill-colour control.</item>
///   <item><see cref="MauiIconSource"/> – any MAUI <c>ImageSource</c>
///         (file, font-glyph, URI).</item>
///   <item><see cref="Icon"/> – treated as a <c>FileImageSource</c> filename.</item>
/// </list>
/// </para>
/// </summary>
public partial class MauiGlassActionButton : ContentView
{
    // ── Bindable properties ─────────────────────────────────────────────────

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(MauiGlassActionButton),
            defaultValue: string.Empty,
            propertyChanged: (b, _, _) => ((MauiGlassActionButton)b).RebuildContent());

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(
            nameof(Icon),
            typeof(string),
            typeof(MauiGlassActionButton),
            defaultValue: null,
            propertyChanged: (b, _, _) => ((MauiGlassActionButton)b).RebuildContent());

    public static readonly BindableProperty AccentColorProperty =
        BindableProperty.Create(
            nameof(AccentColor),
            typeof(Color),
            typeof(MauiGlassActionButton),
            defaultValue: Color.FromArgb("#007AFF"),
            propertyChanged: (b, _, _) => ((MauiGlassActionButton)b).UpdateVisuals());

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(MauiGlassActionButton),
            defaultValue: null,
            propertyChanged: (b, _, _) => ((MauiGlassActionButton)b).UpdateVisuals());

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(
            nameof(Command),
            typeof(ICommand),
            typeof(MauiGlassActionButton),
            defaultValue: null);

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(
            nameof(CommandParameter),
            typeof(object),
            typeof(MauiGlassActionButton),
            defaultValue: null);

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>The button label text.</summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Fallback icon filename.  Checked last after <see cref="IconGeometry"/>
    /// and <see cref="MauiIconSource"/>.
    /// </summary>
    public string? Icon
    {
        get => (string?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Accent colour used as the default content colour when <see cref="TextColor"/>
    /// is not explicitly set.  Defaults to iOS system blue (<c>#007AFF</c>).
    /// </summary>
    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    /// <summary>
    /// Colour for button text and icon.  When <c>null</c> (the default)
    /// <see cref="AccentColor"/> is used.
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

    // ── Private state ────────────────────────────────────────────────────────

    private View? _iconView;
    private Label? _label;
    private Brush _glassBrush = Brush.Transparent;
    private Color _glassStroke = Colors.Transparent;

    // ── Constructor ──────────────────────────────────────────────────────────

    public MauiGlassActionButton()
    {
        ResolveThemeColors();
        InitializeComponent();
        ApplyThemeToGlassBorder();
        RebuildContent();

        // Re-resolve colours when the theme changes at runtime.
        if (Application.Current != null)
            Application.Current.RequestedThemeChanged += (_, _) =>
            {
                ResolveThemeColors();
                ApplyThemeToGlassBorder();
                UpdateVisuals();
            };

        // Tap gesture.
        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        GlassBorder.GestureRecognizers.Add(tap);
    }

    // ── Layout ───────────────────────────────────────────────────────────────

    private void ApplyThemeToGlassBorder()
    {
        GlassBorder.Background = _glassBrush;
        GlassBorder.Stroke = new SolidColorBrush(_glassStroke);
    }

    // ── Theme helpers ────────────────────────────────────────────────────────

    private void ResolveThemeColors()
    {
        bool isDark = Application.Current?.RequestedTheme == AppTheme.Dark;

        _glassBrush = isDark
            ? new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Color.FromArgb("#D02A2A30"), 0f),
                    new GradientStop(Color.FromArgb("#C81E1E24"), 1f),
                },
            }
            : new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Color.FromArgb("#DEF0F4F8"), 0f),
                    new GradientStop(Color.FromArgb("#D0E4E8EE"), 1f),
                },
            };

        _glassStroke = isDark
            ? Color.FromArgb("#20FFFFFF")
            : Color.FromArgb("#40FFFFFF");
    }

    // ── Content building ─────────────────────────────────────────────────────

    private void RebuildContent()
    {
        ContentStack.Children.Clear();
        _iconView = null;
        _label = null;

        Color contentColor = GetEffectiveContentColor();

        // ── Icon ─────────────────────────────────────────────────────────────
        _iconView = CreateIcon(contentColor);
        if (_iconView != null)
            ContentStack.Children.Add(_iconView);

        // ── Label ────────────────────────────────────────────────────────────
        if (!string.IsNullOrEmpty(Text))
        {
            _label = new Label
            {
                Text = Text,
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
                TextColor = contentColor,
            };
            ContentStack.Children.Add(_label);
        }
    }

    private View? CreateIcon(Color color)
    {
        // 3) Fallback: treat Icon string as a filename.
        if (!string.IsNullOrEmpty(Icon))
        {
            return new Image
            {
                Source = Icon,
                WidthRequest = 20,
                HeightRequest = 20,
                VerticalOptions = LayoutOptions.Center,
            };
        }

        // 4) Nothing – no icon.
        return null;
    }

    // ── Visuals ──────────────────────────────────────────────────────────────

    /// <summary>Updates visuals when accent colour, text colour, or theme changes.</summary>
    private void UpdateVisuals()
    {
        Color contentColor = GetEffectiveContentColor();

        // Icon colour
        if (_iconView is Microsoft.Maui.Controls.Shapes.Path path)
            path.Fill = new SolidColorBrush(contentColor);

        // Label colour
        if (_label != null)
            _label.TextColor = contentColor;
    }

    // ── Colour helpers ──────────────────────────────────────────────────────

    /// <summary>Returns the effective content colour, respecting explicit overrides.</summary>
    private Color GetEffectiveContentColor() =>
        TextColor ?? AccentColor;

    // ── Tap handler ─────────────────────────────────────────────────────────

    private void OnTapped(object? sender, TappedEventArgs e)
    {
        Clicked?.Invoke(this, EventArgs.Empty);
        if (Command?.CanExecute(CommandParameter) == true)
            Command.Execute(CommandParameter);
    }
}
