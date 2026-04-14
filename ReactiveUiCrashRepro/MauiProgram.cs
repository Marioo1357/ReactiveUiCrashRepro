using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ReactiveUI.Builder;
using ReactiveUiCrashRepro.Controls;

namespace ReactiveUiCrashRepro;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseReactiveUI(reactiveUiBuilder =>
                reactiveUiBuilder.WithMauiScheduler())
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureMauiHandlers(handlers =>
            {
#if IOS || ANDROID
                handlers.AddHandler<NativeTabBar, NativeTabBarHandler>();
                handlers.AddHandler<NativeActionButton, NativeActionButtonHandler>();
#endif
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}