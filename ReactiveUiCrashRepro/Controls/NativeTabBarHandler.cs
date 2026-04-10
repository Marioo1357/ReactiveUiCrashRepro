// Platform-specific native-view type alias used by the base class.
#if IOS
using PlatformView = UIKit.UITabBar;
#elif ANDROID
using PlatformView = Google.Android.Material.BottomNavigation.BottomNavigationView;
#endif

#if IOS || ANDROID
using Microsoft.Maui.Handlers;

namespace ReactiveUiCrashRepro.Controls;

/// <summary>
/// MAUI handler that bridges <see cref="NativeTabBar"/> to the platform-native view.
/// Platform-specific creation and update logic lives in Platforms/iOS and Platforms/Android
/// partial class files.
/// </summary>
public partial class NativeTabBarHandler : ViewHandler<NativeTabBar, PlatformView>
{
    public static IPropertyMapper<NativeTabBar, NativeTabBarHandler> Mapper =
        new PropertyMapper<NativeTabBar, NativeTabBarHandler>(ViewMapper)
        {
            [nameof(NativeTabBar.Items)] = MapItems,
            [nameof(NativeTabBar.SelectedIndex)] = MapSelectedIndex,
        };

    public NativeTabBarHandler() : base(Mapper) { }

    public NativeTabBarHandler(IPropertyMapper mapper) : base(mapper) { }

    // ── Static mapper stubs (delegates to the partial implementations) ───────

    private static void MapItems(NativeTabBarHandler handler, NativeTabBar view) =>
        handler.UpdateTabItems(view);

    private static void MapSelectedIndex(NativeTabBarHandler handler, NativeTabBar view) =>
        handler.UpdateSelectedIndex(view);
}
#endif
