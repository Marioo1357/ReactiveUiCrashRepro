namespace ReactiveUiCrashRepro.Controls;

/// <summary>
/// A standalone, platform-native tab bar.
/// <para>
/// iOS  – renders a <c>UITabBar</c> using <c>UITabBarAppearance.ConfigureWithDefaultBackground()</c>
///         which applies the system blur / frosted-glass treatment.  On iOS 26+ the system
///         automatically upgrades the appearance to Liquid Glass.
/// </para>
/// <para>
/// Android – renders a Material Design <c>BottomNavigationView</c> styled with a semi-transparent
///            frosted-glass surface colour and elevation shadow.
/// </para>
/// <para>
/// This control is intentionally <em>standalone</em>: it does not drive navigation itself.
/// Wire up <see cref="TabItemSelected"/> or bind <see cref="SelectedIndex"/> to react to tab
/// changes in your own view-model or code-behind.
/// </para>
/// </summary>
public class NativeTabBar : View
{
    // ── Bindable properties ─────────────────────────────────────────────────

    public static readonly BindableProperty ItemsProperty =
        BindableProperty.Create(
            nameof(Items),
            typeof(IList<TabItem>),
            typeof(NativeTabBar),
            defaultValue: null,
            propertyChanged: (b, _, _) =>
                ((NativeTabBar)b).Handler?.UpdateValue(nameof(Items)));

    public static readonly BindableProperty SelectedIndexProperty =
        BindableProperty.Create(
            nameof(SelectedIndex),
            typeof(int),
            typeof(NativeTabBar),
            defaultValue: 0,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (b, _, _) =>
                ((NativeTabBar)b).Handler?.UpdateValue(nameof(SelectedIndex)));

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

    /// <summary>Raised when the user taps a tab.</summary>
    public event EventHandler<TabItemSelectedEventArgs>? TabItemSelected;

    // ── Internal ─────────────────────────────────────────────────────────────

    /// <summary>Called by platform handlers when the user taps a tab.</summary>
    internal void NotifyTabSelected(int index)
    {
        SelectedIndex = index;
        TabItemSelected?.Invoke(this, new TabItemSelectedEventArgs(index));
    }
}

/// <summary>Event arguments for <see cref="NativeTabBar.TabItemSelected"/>.</summary>
public sealed class TabItemSelectedEventArgs : EventArgs
{
    /// <summary>Zero-based index of the newly selected tab.</summary>
    public int Index { get; }

    public TabItemSelectedEventArgs(int index) => Index = index;
}
