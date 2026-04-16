using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;

namespace ReactiveUiCrashRepro.Controls;

/// <summary>
/// A pure-MAUI navigation bar that mimics the iOS 26 Liquid Glass appearance.
/// <para>
/// Used on <b>Android</b> (always) and on <b>iOS&lt;26</b> (where the native Liquid
/// Glass material is not available).  The control renders entirely with MAUI
/// components – no platform-specific code is required.
/// </para>
/// <para>
/// Visual design:
/// <list type="bullet">
///   <item>Semi-transparent frosted-glass background with a subtle gradient.</item>
///   <item>Rounded container with a thin glass-edge stroke and a soft shadow.</item>
///   <item>Back button (chevron) on the left.</item>
///   <item>Page title centred.</item>
///   <item>Two action icon buttons on the right.</item>
/// </list>
/// </para>
/// </summary>
public partial class MauiGlassNavigationBar : ContentView
{
    // Material Design chevron-left path (24×24 viewbox).
    private const string ChevronLeftPath =
        "M15.41 7.41 L14 6 L8 12 L14 18 L15.41 16.59 L10.83 12 Z";

    // ── Bindable properties ─────────────────────────────────────────────────

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(MauiGlassNavigationBar),
            defaultValue: string.Empty,
            propertyChanged: (b, _, _) => ((MauiGlassNavigationBar)b).UpdateTitle());

    public static readonly BindableProperty ShowBackButtonProperty =
        BindableProperty.Create(
            nameof(ShowBackButton),
            typeof(bool),
            typeof(MauiGlassNavigationBar),
            defaultValue: true,
            propertyChanged: (b, _, _) => ((MauiGlassNavigationBar)b).RebuildBackButton());

    public static readonly BindableProperty BackCommandProperty =
        BindableProperty.Create(
            nameof(BackCommand),
            typeof(ICommand),
            typeof(MauiGlassNavigationBar),
            defaultValue: null);

    public static readonly BindableProperty Action1IconProperty =
        BindableProperty.Create(
            nameof(Action1Icon),
            typeof(string),
            typeof(MauiGlassNavigationBar),
            defaultValue: null,
            propertyChanged: (b, _, _) => ((MauiGlassNavigationBar)b).RebuildActionButtons());

    public static readonly BindableProperty Action1CommandProperty =
        BindableProperty.Create(
            nameof(Action1Command),
            typeof(ICommand),
            typeof(MauiGlassNavigationBar),
            defaultValue: null);

    public static readonly BindableProperty Action2IconProperty =
        BindableProperty.Create(
            nameof(Action2Icon),
            typeof(string),
            typeof(MauiGlassNavigationBar),
            defaultValue: null,
            propertyChanged: (b, _, _) => ((MauiGlassNavigationBar)b).RebuildActionButtons());

    public static readonly BindableProperty Action2CommandProperty =
        BindableProperty.Create(
            nameof(Action2Command),
            typeof(ICommand),
            typeof(MauiGlassNavigationBar),
            defaultValue: null);

    public static readonly BindableProperty AccentColorProperty =
        BindableProperty.Create(
            nameof(AccentColor),
            typeof(Color),
            typeof(MauiGlassNavigationBar),
            defaultValue: Color.FromArgb("#007AFF"),
            propertyChanged: (b, _, _) => ((MauiGlassNavigationBar)b).UpdateVisuals());

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(MauiGlassNavigationBar),
            defaultValue: null,
            propertyChanged: (b, _, _) => ((MauiGlassNavigationBar)b).UpdateVisuals());

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

    /// <summary>Icon filename for action button 1.</summary>
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

    /// <summary>Icon filename for action button 2.</summary>
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

    /// <summary>Accent colour for icons and interactive elements.</summary>
    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    /// <summary>
    /// Colour for the title text.  When <c>null</c> (the default) a
    /// theme-appropriate colour is used.
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

    // ── Private state ────────────────────────────────────────────────────────

    private Microsoft.Maui.Controls.Shapes.Path? _backChevron;
    private Image? _action1Image;
    private Image? _action2Image;
    private Brush _glassBrush = Brush.Transparent;
    private Color _glassStroke = Colors.Transparent;
    private Color _defaultTitleColor = Colors.Black;

    // ── Constructor ──────────────────────────────────────────────────────────

    public MauiGlassNavigationBar()
    {
        ResolveThemeColors();
        InitializeComponent();
        ApplyThemeToGlassBorder();
        RebuildBackButton();
        RebuildActionButtons();
        UpdateTitle();
        UpdateVisuals();

        // Re-resolve colours when the theme changes at runtime.
        if (Application.Current != null)
            Application.Current.RequestedThemeChanged += (_, _) =>
            {
                ResolveThemeColors();
                ApplyThemeToGlassBorder();
                UpdateVisuals();
            };
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

        _defaultTitleColor = isDark
            ? Colors.White
            : Colors.Black;

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

    // ── Back button ─────────────────────────────────────────────────────────

    private void RebuildBackButton()
    {
        BackButtonContainer.Children.Clear();
        _backChevron = null;

        if (!ShowBackButton) return;

        Color accentColor = AccentColor;

        _backChevron = new Microsoft.Maui.Controls.Shapes.Path
        {
            Data = (Geometry)new PathGeometryConverter().ConvertFromString(ChevronLeftPath)!,
            Fill = new SolidColorBrush(accentColor),
            WidthRequest = 24,
            HeightRequest = 24,
            Aspect = Stretch.Uniform,
            VerticalOptions = LayoutOptions.Center,
        };

        BackButtonContainer.Children.Add(_backChevron);

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnBackTapped;
        BackButtonContainer.GestureRecognizers.Clear();
        BackButtonContainer.GestureRecognizers.Add(tap);
    }

    // ── Action buttons ──────────────────────────────────────────────────────

    private void RebuildActionButtons()
    {
        ActionButtonsContainer.Children.Clear();
        _action1Image = null;
        _action2Image = null;

        if (!string.IsNullOrEmpty(Action1Icon))
        {
            _action1Image = CreateActionImage(Action1Icon);
            var tap1 = new TapGestureRecognizer();
            tap1.Tapped += OnAction1Tapped;
            _action1Image.GestureRecognizers.Add(tap1);
            ActionButtonsContainer.Children.Add(_action1Image);
        }

        if (!string.IsNullOrEmpty(Action2Icon))
        {
            _action2Image = CreateActionImage(Action2Icon);
            var tap2 = new TapGestureRecognizer();
            tap2.Tapped += OnAction2Tapped;
            _action2Image.GestureRecognizers.Add(tap2);
            ActionButtonsContainer.Children.Add(_action2Image);
        }
    }

    private static Image CreateActionImage(string iconSource)
    {
        return new Image
        {
            Source = iconSource,
            WidthRequest = 24,
            HeightRequest = 24,
            VerticalOptions = LayoutOptions.Center,
        };
    }

    // ── Title ───────────────────────────────────────────────────────────────

    private void UpdateTitle()
    {
        TitleLabel.Text = Title;
    }

    // ── Visuals ─────────────────────────────────────────────────────────────

    private void UpdateVisuals()
    {
        Color titleColor = TextColor ?? _defaultTitleColor;
        TitleLabel.TextColor = titleColor;

        Color accentColor = AccentColor;
        if (_backChevron != null)
            _backChevron.Fill = new SolidColorBrush(accentColor);
    }

    // ── Tap handlers ────────────────────────────────────────────────────────

    private void OnBackTapped(object? sender, TappedEventArgs e)
    {
        BackButtonClicked?.Invoke(this, EventArgs.Empty);
        if (BackCommand?.CanExecute(null) == true)
            BackCommand.Execute(null);
    }

    private void OnAction1Tapped(object? sender, TappedEventArgs e)
    {
        Action1Clicked?.Invoke(this, EventArgs.Empty);
        if (Action1Command?.CanExecute(null) == true)
            Action1Command.Execute(null);
    }

    private void OnAction2Tapped(object? sender, TappedEventArgs e)
    {
        Action2Clicked?.Invoke(this, EventArgs.Empty);
        if (Action2Command?.CanExecute(null) == true)
            Action2Command.Execute(null);
    }
}
