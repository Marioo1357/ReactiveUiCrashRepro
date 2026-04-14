using System.Runtime.Versioning;
using Foundation;
using Microsoft.Maui.Platform;
using UIKit;

namespace ReactiveUiCrashRepro.Controls;

[SupportedOSPlatform("ios26.0")]
public partial class NativeActionButtonHandler
{
    protected override UIButton CreatePlatformView()
    {
        var button = new UIButton(UIButtonType.System);
        ConfigureAppearance(button);

        button.TouchUpInside += OnButtonTapped;
        return button;
    }

    protected override void DisconnectHandler(UIButton platformView)
    {
        platformView.TouchUpInside -= OnButtonTapped;
        base.DisconnectHandler(platformView);
    }
    
    private static void ConfigureAppearance(UIButton button)
    {
        var config = UIButtonConfiguration.PlainButtonConfiguration;
        config.Background.BackgroundColor = UIColor.Clear;
        config.CornerStyle = UIButtonConfigurationCornerStyle.Capsule;
        config.ImagePlacement = NSDirectionalRectEdge.Top;
        config.ImagePadding = 4;
        config.TitleAlignment = UIButtonConfigurationTitleAlignment.Center;
        button.Configuration = config;
        
        var blur = UIGlassEffect.Create(UIGlassEffectStyle.Regular);
        var blurView = new UIVisualEffectView(blur);
        
        blurView.Frame = button.Bounds;
        blurView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
        
        button.InsertSubview(blurView, 0);
        
        button.Layer.CornerRadius = 24;
        button.ClipsToBounds = true;
    }

    private void OnButtonTapped(object? sender, EventArgs e)
    {
        VirtualView.NotifyClicked();
    }

    private void UpdateText(NativeActionButton virtualView)
    {
        var config = PlatformView.Configuration;
        if (config != null)
        {
            config.AttributedTitle = new NSAttributedString(virtualView.Text, new UIStringAttributes
            {
                ForegroundColor = UIColor.Blue,
                Font = UIFont.SystemFontOfSize(8, UIFontWeight.Medium)
            });
            config.Title = virtualView.Text;
            PlatformView.Configuration = config;
        }
    }
    
    private void UpdateIcon(NativeActionButton virtualView)
    {
        var config = PlatformView.Configuration;
        if (config == null) return;

        if (!string.IsNullOrEmpty(virtualView.Icon))
        {
            var image = UIImage.FromFile(virtualView.Icon)?
                .ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);

            config.Image = image;
            config.ImagePadding = 8;
            config.BaseForegroundColor = VirtualView.ForegroundColor.ToPlatform();
        }
        else
        {
            config.Image = null;
        }

        PlatformView.Configuration = config;
    }
}
