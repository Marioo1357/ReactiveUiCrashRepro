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
    /// SVG path-data string for rendering the icon as a MAUI <c>Shapes.Path</c>.
    /// Used by <see cref="MauiGlassTabBar"/> which gives full fill-colour control.
    /// <para>
    /// Example (Material Design "home" icon, 24×24 view-box):
    /// <c>"M10 20v-6h4v6h5v-8h3L12 3 2 12h3v8z"</c>
    /// </para>
    /// </summary>
    public string? IconGeometry { get; set; }

    /// <summary>
    /// Optional MAUI <see cref="ImageSource"/> for the icon.
    /// Used by <see cref="MauiGlassTabBar"/> as a fallback when
    /// <see cref="IconGeometry"/> is not set.
    /// <para>
    /// You can use any <see cref="ImageSource"/> type:
    /// <list type="bullet">
    ///   <item><c>FileImageSource</c> – e.g. <c>"tab_home.png"</c>
    ///         (place the SVG as <c>Resources/Images/tab_home.svg</c>;
    ///          MAUI converts it to PNG at build time).</item>
    ///   <item><c>FontImageSource</c> – render a glyph from any registered font.</item>
    ///   <item><c>UriImageSource</c>  – load from a URL.</item>
    /// </list>
    /// </para>
    /// </summary>
    public ImageSource? MauiIconSource { get; set; }

    /// <summary>
    /// Badge count shown on the tab.  Zero or negative hides the badge.
    /// </summary>
    public int BadgeCount { get; set; }
}
