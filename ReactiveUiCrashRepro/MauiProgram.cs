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
#endif
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}