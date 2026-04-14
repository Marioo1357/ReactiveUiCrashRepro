using CommunityToolkit.Maui.Behaviors;
using Microsoft.Maui.Controls.Shapes;

namespace ReactiveUiCrashRepro.Controls;

/// <summary>
/// A pure-MAUI tab bar that mimics the iOS 26 Liquid Glass appearance.
/// <para>
/// Used on <b>Android</b> (always) and on <b>iOS&lt;26</b> (where the native Liquid
/// Glass material is not available).  The control renders entirely with MAUI
/// components – no platform-specific code is required.
/// </para>
/// <para>
/// Visual design:
/// <list type="bullet">
///   <item>Semi-transparent frosted-glass background with a subtle gradient.</item>
///   <item>Rounded pill container with a thin glass-edge stroke and a soft shadow.</item>
///   <item>Selected tab highlighted with an accent-colour pill and colour-tinted icon.</item>
///   <item>Badge support (red circle with count).</item>
/// </list>
/// </para>
/// <para>
/// <b>Icons</b> – each <see cref="TabItem"/> can provide icons in several ways
/// (checked in this order):
/// <list type="number">
///   <item><see cref="TabItem.IconGeometry"/> – SVG path data rendered as a
///         <c>Shapes.Path</c> with full fill-colour control.</item>
///   <item><see cref="TabItem.MauiIconSource"/> – any MAUI <c>ImageSource</c>
///         (file, font-glyph, URI).</item>
///   <item><see cref="TabItem.Icon"/> – treated as a <c>FileImageSource</c> filename.</item>
/// </list>
/// </para>
/// </summary>
public partial class MauiGlassTabBar : ContentView
{
    // ── Bindable properties ─────────────────────────────────────────────────

    public static readonly BindableProperty ItemsProperty =
        BindableProperty.Create(
            nameof(Items),
            typeof(IList<TabItem>),
            typeof(MauiGlassTabBar),
            defaultValue: null,
            propertyChanged: (b, _, _) => ((MauiGlassTabBar)b).RebuildTabs());

    public static readonly BindableProperty SelectedIndexProperty =
        BindableProperty.Create(
            nameof(SelectedIndex),
            typeof(int),
            typeof(MauiGlassTabBar),
            defaultValue: 0,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (b, o, n) =>
                ((MauiGlassTabBar)b).OnSelectedIndexChanged((int)o, (int)n));

    public static readonly BindableProperty AccentColorProperty =
        BindableProperty.Create(
            nameof(AccentColor),
            typeof(Color),
            typeof(MauiGlassTabBar),
            defaultValue: Color.FromArgb("#007AFF"),
            propertyChanged: (b, _, _) => ((MauiGlassTabBar)b).UpdateSelectionVisuals());

    public static readonly BindableProperty IconColorProperty =
        BindableProperty.Create(
            nameof(IconColor),
            typeof(Color),
            typeof(MauiGlassTabBar),
            defaultValue: null,
            propertyChanged: (b, _, _) => ((MauiGlassTabBar)b).UpdateSelectionVisuals());

    public static readonly BindableProperty SelectedIconColorProperty =
        BindableProperty.Create(
            nameof(SelectedIconColor),
            typeof(Color),
            typeof(MauiGlassTabBar),
            defaultValue: null,
            propertyChanged: (b, _, _) => ((MauiGlassTabBar)b).UpdateSelectionVisuals());

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(MauiGlassTabBar),
            defaultValue: null,
            propertyChanged: (b, _, _) => ((MauiGlassTabBar)b).UpdateSelectionVisuals());

    public static readonly BindableProperty SelectedTextColorProperty =
        BindableProperty.Create(
            nameof(SelectedTextColor),
            typeof(Color),
            typeof(MauiGlassTabBar),
            defaultValue: null,
            propertyChanged: (b, _, _) => ((MauiGlassTabBar)b).UpdateSelectionVisuals());

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>The tab items to display.</summary>
    public IList<TabItem>? Items
    {
        get => (IList<TabItem>?)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    /// <summary>Zero-based index of the currently selected tab.</summary>
    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    /// <summary>
    /// General accent colour used for the selected tab pill highlight and as the
    /// default for <see cref="SelectedIconColor"/> and <see cref="SelectedTextColor"/>
    /// when they are not explicitly set.  Defaults to iOS system blue (<c>#007AFF</c>).
    /// </summary>
    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    /// <summary>
    /// Colour for unselected tab icons.  When <c>null</c> (the default) a
    /// theme-appropriate grey is used.
    /// </summary>
    public Color? IconColor
    {
        get => (Color?)GetValue(IconColorProperty);
        set => SetValue(IconColorProperty, value);
    }

    /// <summary>
    /// Colour for the selected tab icon.  When <c>null</c> (the default)
    /// <see cref="AccentColor"/> is used.
    /// </summary>
    public Color? SelectedIconColor
    {
        get => (Color?)GetValue(SelectedIconColorProperty);
        set => SetValue(SelectedIconColorProperty, value);
    }

    /// <summary>
    /// Colour for unselected tab labels.  When <c>null</c> (the default) a
    /// theme-appropriate grey is used.
    /// </summary>
    public Color? TextColor
    {
        get => (Color?)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    /// <summary>
    /// Colour for the selected tab label.  When <c>null</c> (the default)
    /// <see cref="AccentColor"/> is used.
    /// </summary>
    public Color? SelectedTextColor
    {
        get => (Color?)GetValue(SelectedTextColorProperty);
        set => SetValue(SelectedTextColorProperty, value);
    }

    /// <summary>Raised when the user taps a tab.</summary>
    public event EventHandler<TabItemSelectedEventArgs>? TabItemSelected;

    // ── Private state ────────────────────────────────────────────────────────

    private readonly List<TabViewState> _tabStates = new();

    // Default theme-resolved unselected colour (used when IconColor/TextColor are null).
    private Color _defaultUnselectedColor = Color.FromArgb("#8E8E93");
    private Brush _glassBrush = Brush.Transparent;
    private Color _glassStroke = Colors.Transparent;

    // ── Constructor ──────────────────────────────────────────────────────────

    public MauiGlassTabBar()
    {
        ResolveThemeColors();
        InitializeComponent();
        ApplyThemeToGlassBorder();

        // Re-resolve colours when the theme changes at runtime.
        if (Application.Current != null)
            Application.Current.RequestedThemeChanged += (_, _) =>
            {
                ResolveThemeColors();
                ApplyThemeToGlassBorder();
                UpdateSelectionVisuals();
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

        _defaultUnselectedColor = isDark
            ? Color.FromArgb("#98989E")
            : Color.FromArgb("#8E8E93");

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

    // ── Tab building ─────────────────────────────────────────────────────────

    private void RebuildTabs()
    {
        TabGrid.Children.Clear();
        TabGrid.ColumnDefinitions.Clear();
        _tabStates.Clear();

        var items = Items;
        if (items == null || items.Count == 0) return;

        for (int i = 0; i < items.Count; i++)
        {
            TabGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

            var state = CreateTabView(items[i], i);
            Grid.SetColumn(state.Pill, i);
            TabGrid.Children.Add(state.Pill);
            _tabStates.Add(state);
        }

        UpdateSelectionVisuals();
    }

    private TabViewState CreateTabView(TabItem item, int index)
    {
        bool isSelected = index == SelectedIndex;
        Color iconColor = GetEffectiveIconColor(isSelected);
        Color textColor = GetEffectiveTextColor(isSelected);

        // ── Icon ─────────────────────────────────────────────────────────────
        View icon = CreateIcon(item, iconColor, out var tintBehavior);

        // ── Badge ────────────────────────────────────────────────────────────
        View iconWithBadge;
        Border? badge = null;
        if (item.BadgeCount > 0)
        {
            badge = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 8 },
                Background = new SolidColorBrush(Colors.Red),
                StrokeThickness = 0,
                Padding = new Thickness(4, 1),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(0, -4, -6, 0),
                Content = new Label
                {
                    Text = item.BadgeCount.ToString(),
                    TextColor = Colors.White,
                    FontSize = 9,
                    HorizontalTextAlignment = TextAlignment.Center,
                },
            };

            var badgeGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.Center,
                Children = { icon, badge },
            };
            iconWithBadge = badgeGrid;
        }
        else
        {
            iconWithBadge = icon;
        }

        // ── Label ────────────────────────────────────────────────────────────
        var label = new Label
        {
            Text = item.Title,
            FontSize = 10,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            TextColor = textColor,
        };

        // ── Stack (icon + label) ─────────────────────────────────────────────
        var stack = new VerticalStackLayout
        {
            Spacing = 2,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children = { iconWithBadge, label },
        };

        // ── Selection pill ───────────────────────────────────────────────────
        var pill = new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = 18 },
            Stroke = Brush.Transparent,
            StrokeThickness = 0,
            Background = isSelected
                ? new SolidColorBrush(AccentColor.WithAlpha(0.12f))
                : Brush.Transparent,
            Padding = new Thickness(6, 8),
            Content = stack,
        };

        // ── Tap handler ──────────────────────────────────────────────────────
        var tap = new TapGestureRecognizer();
        int capturedIndex = index;
        tap.Tapped += (_, _) => SelectTab(capturedIndex);
        pill.GestureRecognizers.Add(tap);

        return new TabViewState
        {
            Pill = pill,
            Icon = icon,
            Label = label,
            Badge = badge,
            TintBehavior = tintBehavior,
        };
    }

    private static View CreateIcon(TabItem item, Color color, out IconTintColorBehavior? tintBehavior)
    {
        tintBehavior = null;

        // 1) Prefer IconGeometry (SVG path data → Shapes.Path with fill-colour control).
        if (!string.IsNullOrEmpty(item.IconGeometry))
        {
            var converter = new PathGeometryConverter();
            var geometry = (Geometry?)converter.ConvertFromInvariantString(item.IconGeometry);

            return new Microsoft.Maui.Controls.Shapes.Path
            {
                Data = geometry,
                Fill = new SolidColorBrush(color),
                WidthRequest = 24,
                HeightRequest = 24,
                Aspect = Stretch.Uniform,
                HorizontalOptions = LayoutOptions.Center,
            };
        }

        // 2) MauiIconSource (any ImageSource – file, font-glyph, URI).
        //    Tinted via IconTintColorBehavior from CommunityToolkit.Maui.
        if (item.MauiIconSource != null)
        {
            tintBehavior = new IconTintColorBehavior { TintColor = color };
            var image = new Image
            {
                Source = item.MauiIconSource,
                WidthRequest = 24,
                HeightRequest = 24,
                HorizontalOptions = LayoutOptions.Center,
            };
            image.Behaviors.Add(tintBehavior);
            return image;
        }

        // 3) Fallback: treat Icon string as a filename.
        //    Also tinted via IconTintColorBehavior.
        if (!string.IsNullOrEmpty(item.Icon))
        {
            tintBehavior = new IconTintColorBehavior { TintColor = color };
            var image = new Image
            {
                Source = item.Icon,
                WidthRequest = 24,
                HeightRequest = 24,
                HorizontalOptions = LayoutOptions.Center,
            };
            image.Behaviors.Add(tintBehavior);
            return image;
        }

        // 4) Nothing – invisible placeholder.
        return new BoxView { WidthRequest = 24, HeightRequest = 24, Color = Colors.Transparent };
    }

    // ── Selection ────────────────────────────────────────────────────────────

    private void SelectTab(int index)
    {
        SelectedIndex = index;
        TabItemSelected?.Invoke(this, new TabItemSelectedEventArgs(index));
    }

    private void OnSelectedIndexChanged(int oldIndex, int newIndex)
    {
        UpdateSelectionVisuals(oldIndex, newIndex);
    }

    /// <summary>Updates visuals for all tabs (used when accent colour or theme changes).</summary>
    private void UpdateSelectionVisuals()
    {
        for (int i = 0; i < _tabStates.Count; i++)
        {
            bool isSelected = i == SelectedIndex;
            ApplyTabVisualState(_tabStates[i], isSelected);
        }
    }

    /// <summary>Updates visuals only for the old and new selected tabs.</summary>
    private void UpdateSelectionVisuals(int oldIndex, int newIndex)
    {
        if (oldIndex >= 0 && oldIndex < _tabStates.Count)
            ApplyTabVisualState(_tabStates[oldIndex], isSelected: false);

        if (newIndex >= 0 && newIndex < _tabStates.Count)
            ApplyTabVisualState(_tabStates[newIndex], isSelected: true);
    }

    private void ApplyTabVisualState(TabViewState state, bool isSelected)
    {
        Color iconColor = GetEffectiveIconColor(isSelected);
        Color textColor = GetEffectiveTextColor(isSelected);

        // Pill background
        state.Pill.Background = isSelected
            ? new SolidColorBrush(AccentColor.WithAlpha(0.12f))
            : Brush.Transparent;

        // Icon colour
        if (state.Icon is Microsoft.Maui.Controls.Shapes.Path path)
            path.Fill = new SolidColorBrush(iconColor);
        else if (state.TintBehavior != null)
            state.TintBehavior.TintColor = iconColor;
        else if (state.Icon is Image img)
            img.Opacity = isSelected ? 1.0 : 0.55;

        // Label colour
        state.Label.TextColor = textColor;
    }

    // ── Colour helpers ──────────────────────────────────────────────────────

    /// <summary>Returns the effective icon colour, respecting explicit overrides.</summary>
    private Color GetEffectiveIconColor(bool isSelected) =>
        isSelected
            ? (SelectedIconColor ?? AccentColor)
            : (IconColor ?? _defaultUnselectedColor);

    /// <summary>Returns the effective text colour, respecting explicit overrides.</summary>
    private Color GetEffectiveTextColor(bool isSelected) =>
        isSelected
            ? (SelectedTextColor ?? AccentColor)
            : (TextColor ?? _defaultUnselectedColor);

    // ── Helper types ─────────────────────────────────────────────────────────

    private sealed class TabViewState
    {
        public required Border Pill { get; init; }
        public required View Icon { get; init; }
        public required Label Label { get; init; }
        public Border? Badge { get; init; }
        public IconTintColorBehavior? TintBehavior { get; init; }
    }
}
