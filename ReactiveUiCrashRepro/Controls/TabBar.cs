namespace ReactiveUiCrashRepro.Controls;

/// <summary>
/// A self-contained tab bar control that automatically picks the best platform
/// implementation at runtime:
/// <list type="bullet">
///   <item><b>iOS 26+</b> – <see cref="NativeTabBar"/> (native <c>UITabBar</c> with
///         automatic Liquid Glass).</item>
///   <item><b>iOS &lt; 26</b> – <see cref="MauiGlassTabBar"/> (pure MAUI frosted-glass
///         look-alike).</item>
///   <item><b>Android</b> – <see cref="MauiGlassTabBar"/> (100 % MAUI frosted-glass
///         appearance).</item>
/// </list>
/// <para>
/// Consumers only need to set <see cref="Items"/> and bind <see cref="SelectedIndex"/>.
/// The page does not need to know which implementation is used internally.
/// </para>
/// <para>
/// Style properties (<see cref="AccentColor"/>, <see cref="IconColor"/>,
/// <see cref="SelectedIconColor"/>, <see cref="TextColor"/>,
/// <see cref="SelectedTextColor"/>) are forwarded to the MAUI glass tab bar.
/// On iOS 26+ the native <c>UITabBar</c> handles its own appearance via Liquid Glass.
/// </para>
/// </summary>
public class TabBar : ContentView
{
    // ── Bindable properties ─────────────────────────────────────────────────

    public static readonly BindableProperty ItemsProperty =
        BindableProperty.Create(
            nameof(Items),
            typeof(IList<TabItem>),
            typeof(TabBar),
            defaultValue: null);

    public static readonly BindableProperty SelectedIndexProperty =
        BindableProperty.Create(
            nameof(SelectedIndex),
            typeof(int),
            typeof(TabBar),
            defaultValue: 0,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty AccentColorProperty =
        BindableProperty.Create(
            nameof(AccentColor),
            typeof(Color),
            typeof(TabBar),
            defaultValue: Color.FromArgb("#007AFF"));

    public static readonly BindableProperty IconColorProperty =
        BindableProperty.Create(
            nameof(IconColor),
            typeof(Color),
            typeof(TabBar),
            defaultValue: null);

    public static readonly BindableProperty SelectedIconColorProperty =
        BindableProperty.Create(
            nameof(SelectedIconColor),
            typeof(Color),
            typeof(TabBar),
            defaultValue: null);

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(TabBar),
            defaultValue: null);

    public static readonly BindableProperty SelectedTextColorProperty =
        BindableProperty.Create(
            nameof(SelectedTextColor),
            typeof(Color),
            typeof(TabBar),
            defaultValue: null);

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
    /// General accent colour used for the selected tab highlight and as the
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

    // ── Constructor ──────────────────────────────────────────────────────────

    public TabBar()
    {
        Content = CreateInnerTabBar();
    }

    // ── Implementation selection ─────────────────────────────────────────────

    private View CreateInnerTabBar()
    {
#if IOS
        if (OperatingSystem.IsIOSVersionAtLeast(26))
            return CreateNativeTabBar();
#endif
        return CreateMauiGlassTabBar();
    }

#if IOS
    private NativeTabBar CreateNativeTabBar()
    {
        var native = new NativeTabBar();
        native.SetBinding(NativeTabBar.ItemsProperty,
            new Binding(nameof(Items), source: this));
        native.SetBinding(NativeTabBar.SelectedIndexProperty,
            new Binding(nameof(SelectedIndex), BindingMode.TwoWay, source: this));

        native.TabItemSelected += (_, e) => TabItemSelected?.Invoke(this, e);
        return native;
    }
#endif

    private MauiGlassTabBar CreateMauiGlassTabBar()
    {
        var glass = new MauiGlassTabBar();

        // Data bindings
        glass.SetBinding(MauiGlassTabBar.ItemsProperty,
            new Binding(nameof(Items), source: this));
        glass.SetBinding(MauiGlassTabBar.SelectedIndexProperty,
            new Binding(nameof(SelectedIndex), BindingMode.TwoWay, source: this));

        // Style bindings
        glass.SetBinding(MauiGlassTabBar.AccentColorProperty,
            new Binding(nameof(AccentColor), source: this));
        glass.SetBinding(MauiGlassTabBar.IconColorProperty,
            new Binding(nameof(IconColor), source: this));
        glass.SetBinding(MauiGlassTabBar.SelectedIconColorProperty,
            new Binding(nameof(SelectedIconColor), source: this));
        glass.SetBinding(MauiGlassTabBar.TextColorProperty,
            new Binding(nameof(TextColor), source: this));
        glass.SetBinding(MauiGlassTabBar.SelectedTextColorProperty,
            new Binding(nameof(SelectedTextColor), source: this));

        glass.TabItemSelected += (_, e) => TabItemSelected?.Invoke(this, e);
        return glass;
    }
}
