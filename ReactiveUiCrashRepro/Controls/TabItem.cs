namespace ReactiveUiCrashRepro.Controls;

/// <summary>
/// Represents a single tab item displayed in a <see cref="NativeTabBar"/> or
/// <see cref="MauiGlassTabBar"/>.
/// </summary>
public class TabItem
{
    /// <summary>
    /// The label shown below the icon.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Platform-native icon identifier.
    /// On iOS this is an SF Symbol name (e.g. "house.fill", "magnifyingglass").
    /// On Android this is a drawable resource name (e.g. "ic_home") located in the
    /// platform Resources/drawable folder.  Leave null/empty for a title-only tab.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Badge count shown on the tab.  Zero or negative hides the badge.
    /// </summary>
    public int BadgeCount { get; set; }
}
