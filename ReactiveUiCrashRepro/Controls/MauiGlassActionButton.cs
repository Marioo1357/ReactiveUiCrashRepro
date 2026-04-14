using System.Windows.Input;
using CommunityToolkit.Maui.Behaviors;
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
///   <item>Icon (optional) + text laid out in a vertical stack (icon on top, text below).</item>
/// </list>
/// </para>
/// <para>
/// <b>Icons</b> – each button can provide icons in several ways
/// (checked in this order):
/// <list type="number">
///   <item><see cref="IconGeometry"/> – SVG path data rendered as a
///         <c>Shapes.Path</c> with full fill-colour control.</item>
///   <item><see cref="MauiIconSource"/> – any MAUI <c>ImageSource</c>
///         (file, font-glyph, URI).  Tinted with <see cref="AccentColor"/>
///         (or <see cref="TextColor"/>) via <c>IconTintColorBehavior</c>.</item>
///   <item><see cref="Icon"/> – treated as a <c>FileImageSource</c> filename,
///         also tinted via <c>IconTintColorBehavior</c>.</item>
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

    public static readonly BindableProperty IconGeometryProperty =
        BindableProperty.Create(
            nameof(IconGeometry),
            typeof(string),
            typeof(MauiGlassActionButton),
            defaultValue: null,
            propertyChanged: (b, _, _) => ((MauiGlassActionButton)b).RebuildContent());

    public static readonly BindableProperty MauiIconSourceProperty =
        BindableProperty.Create(
            nameof(MauiIconSource),
            typeof(ImageSource),
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
    /// SVG path-data string for rendering the icon as a MAUI <c>Shapes.Path</c>.
    /// Gives full fill-colour control.
    /// </summary>
    public string? IconGeometry
    {
        get => (string?)GetValue(IconGeometryProperty);
        set => SetValue(IconGeometryProperty, value);
    }

    /// <summary>
    /// Optional MAUI <see cref="ImageSource"/> for the icon.
    /// Checked after <see cref="IconGeometry"/>.
    /// </summary>
    public ImageSource? MauiIconSource
    {
        get => (ImageSource?)GetValue(MauiIconSourceProperty);
        set => SetValue(MauiIconSourceProperty, value);
    }

    /// <summary>
    /// Accent colour used as the default content colour when <see cref="TextColor"/>
    /// is not explicitly set.  Defaults to iOS system blue (<c>#007AFF</c>).
    /// Also used as the tint colour for image-based icons.
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
    private IconTintColorBehavior? _tintBehavior;
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
        _tintBehavior = null;

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
                FontSize = 10,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = contentColor,
            };
            ContentStack.Children.Add(_label);
        }
    }

    private View? CreateIcon(Color color)
    {
        // 1) Prefer IconGeometry (SVG path data → Shapes.Path with fill-colour control).
        if (!string.IsNullOrEmpty(IconGeometry))
        {
            var converter = new PathGeometryConverter();
            var geometry = (Geometry?)converter.ConvertFromInvariantString(IconGeometry);

            return new Microsoft.Maui.Controls.Shapes.Path
            {
                Data = geometry,
                Fill = new SolidColorBrush(color),
                WidthRequest = 22,
                HeightRequest = 22,
                Aspect = Stretch.Uniform,
                HorizontalOptions = LayoutOptions.Center,
            };
        }

        // 2) MauiIconSource (any ImageSource – file, font-glyph, URI).
        //    Tinted via IconTintColorBehavior from CommunityToolkit.Maui.
        if (MauiIconSource != null)
        {
            var image = new Image
            {
                Source = MauiIconSource,
                WidthRequest = 22,
                HeightRequest = 22,
                HorizontalOptions = LayoutOptions.Center,
            };
            ApplyTintBehavior(image, color);
            return image;
        }

        // 3) Fallback: treat Icon string as a filename.
        //    Also tinted via IconTintColorBehavior.
        if (!string.IsNullOrEmpty(Icon))
        {
            var image = new Image
            {
                Source = Icon,
                WidthRequest = 22,
                HeightRequest = 22,
                HorizontalOptions = LayoutOptions.Center,
            };
            ApplyTintBehavior(image, color);
            return image;
        }

        // 4) Nothing – no icon.
        return null;
    }

    /// <summary>
    /// Attaches an <see cref="IconTintColorBehavior"/> to the given image
    /// so that MAUI resource images (e.g. <c>tab_home.png</c>) are tinted
    /// with the current content colour.
    /// </summary>
    private void ApplyTintBehavior(Image image, Color tintColor)
    {
        _tintBehavior = new IconTintColorBehavior { TintColor = tintColor };
        image.Behaviors.Add(_tintBehavior);
    }

    // ── Visuals ──────────────────────────────────────────────────────────────

    /// <summary>Updates visuals when accent colour, text colour, or theme changes.</summary>
    private void UpdateVisuals()
    {
        Color contentColor = GetEffectiveContentColor();

        // Icon colour
        if (_iconView is Microsoft.Maui.Controls.Shapes.Path path)
            path.Fill = new SolidColorBrush(contentColor);
        else if (_tintBehavior != null)
            _tintBehavior.TintColor = contentColor;

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
