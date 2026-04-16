// Platform-specific native-view type alias used by the base class.
#if IOS
using PlatformView = UIKit.UINavigationBar;
#elif ANDROID
using PlatformView = Android.Widget.LinearLayout;
#endif

#if IOS || ANDROID
using Microsoft.Maui.Handlers;

namespace ReactiveUiCrashRepro.Controls;

/// <summary>
/// MAUI handler that bridges <see cref="NativeNavigationBar"/> to the platform-native view.
/// Platform-specific creation and update logic lives in Platforms/iOS and Platforms/Android
/// partial class files.
/// </summary>
public partial class NativeNavigationBarHandler : ViewHandler<NativeNavigationBar, PlatformView>
{
    public static IPropertyMapper<NativeNavigationBar, NativeNavigationBarHandler> Mapper =
        new PropertyMapper<NativeNavigationBar, NativeNavigationBarHandler>(ViewMapper)
        {
            [nameof(NativeNavigationBar.Title)] = MapTitle,
            [nameof(NativeNavigationBar.ShowBackButton)] = MapShowBackButton,
            [nameof(NativeNavigationBar.Action1Icon)] = MapActionIcons,
            [nameof(NativeNavigationBar.Action2Icon)] = MapActionIcons,
        };

    public NativeNavigationBarHandler() : base(Mapper) { }

    public NativeNavigationBarHandler(IPropertyMapper mapper) : base(mapper) { }

    // ── Static mapper stubs (delegates to the partial implementations) ───────

    private static void MapTitle(NativeNavigationBarHandler handler, NativeNavigationBar view) =>
        handler.UpdateTitle(view);

    private static void MapShowBackButton(NativeNavigationBarHandler handler, NativeNavigationBar view) =>
        handler.UpdateShowBackButton(view);

    private static void MapActionIcons(NativeNavigationBarHandler handler, NativeNavigationBar view) =>
        handler.UpdateActionIcons(view);
}
#endif
