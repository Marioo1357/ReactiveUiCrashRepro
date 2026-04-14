// Platform-specific native-view type alias used by the base class.
#if IOS
using PlatformView = UIKit.UIButton;
#elif ANDROID
using PlatformView = Android.Widget.Button;
#endif

#if IOS || ANDROID
using Microsoft.Maui.Handlers;

namespace ReactiveUiCrashRepro.Controls;

/// <summary>
/// MAUI handler that bridges <see cref="NativeActionButton"/> to the platform-native view.
/// Platform-specific creation and update logic lives in Platforms/iOS and Platforms/Android
/// partial class files.
/// </summary>
public partial class NativeActionButtonHandler : ViewHandler<NativeActionButton, PlatformView>
{
    public static IPropertyMapper<NativeActionButton, NativeActionButtonHandler> Mapper =
        new PropertyMapper<NativeActionButton, NativeActionButtonHandler>(ViewMapper)
        {
            [nameof(NativeActionButton.Text)] = MapText,
            [nameof(NativeActionButton.Icon)] = MapIcon,
        };

    public NativeActionButtonHandler() : base(Mapper) { }

    public NativeActionButtonHandler(IPropertyMapper mapper) : base(mapper) { }

    // ── Static mapper stubs (delegates to the partial implementations) ───────

    private static void MapText(NativeActionButtonHandler handler, NativeActionButton view) =>
        handler.UpdateText(view);

    private static void MapIcon(NativeActionButtonHandler handler, NativeActionButton view) =>
        handler.UpdateIcon(view);
}
#endif
