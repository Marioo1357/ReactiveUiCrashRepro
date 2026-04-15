using System.Windows.Input;

namespace ReactiveUiCrashRepro.Controls;

/// <summary>
/// A standalone, platform-native navigation bar.
/// <para>
/// iOS  – renders a <c>UINavigationBar</c> using <c>UINavigationBarAppearance.ConfigureWithDefaultBackground()</c>
///         which applies the system blur / frosted-glass treatment.  On iOS 26+ the system
///         automatically upgrades the appearance to Liquid Glass.
/// </para>
/// <para>
/// Android – renders a styled <c>LinearLayout</c> toolbar with a semi-transparent
///            frosted-glass surface colour and elevation shadow.
/// </para>
/// <para>
/// This control is intentionally <em>standalone</em>: it does not drive navigation itself.
/// Wire up the button events or bind commands to react to user interactions.
/// </para>
/// </summary>
public class NativeNavigationBar : View
{
    // ── Bindable properties ─────────────────────────────────────────────────

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(NativeNavigationBar),
            defaultValue: string.Empty,
            propertyChanged: (b, _, _) =>
                ((NativeNavigationBar)b).Handler?.UpdateValue(nameof(Title)));

    public static readonly BindableProperty ShowBackButtonProperty =
        BindableProperty.Create(
            nameof(ShowBackButton),
            typeof(bool),
            typeof(NativeNavigationBar),
            defaultValue: true,
            propertyChanged: (b, _, _) =>
                ((NativeNavigationBar)b).Handler?.UpdateValue(nameof(ShowBackButton)));

    public static readonly BindableProperty BackCommandProperty =
        BindableProperty.Create(
            nameof(BackCommand),
            typeof(ICommand),
            typeof(NativeNavigationBar),
            defaultValue: null);

    public static readonly BindableProperty Action1IconProperty =
        BindableProperty.Create(
            nameof(Action1Icon),
            typeof(string),
            typeof(NativeNavigationBar),
            defaultValue: null,
            propertyChanged: (b, _, _) =>
                ((NativeNavigationBar)b).Handler?.UpdateValue(nameof(Action1Icon)));

    public static readonly BindableProperty Action1CommandProperty =
        BindableProperty.Create(
            nameof(Action1Command),
            typeof(ICommand),
            typeof(NativeNavigationBar),
            defaultValue: null);

    public static readonly BindableProperty Action2IconProperty =
        BindableProperty.Create(
            nameof(Action2Icon),
            typeof(string),
            typeof(NativeNavigationBar),
            defaultValue: null,
            propertyChanged: (b, _, _) =>
                ((NativeNavigationBar)b).Handler?.UpdateValue(nameof(Action2Icon)));

    public static readonly BindableProperty Action2CommandProperty =
        BindableProperty.Create(
            nameof(Action2Command),
            typeof(ICommand),
            typeof(NativeNavigationBar),
            defaultValue: null);

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>Page title displayed in the centre of the bar.</summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>Whether the back button is visible on the left side.</summary>
    public bool ShowBackButton
    {
        get => (bool)GetValue(ShowBackButtonProperty);
        set => SetValue(ShowBackButtonProperty, value);
    }

    /// <summary>Command executed when the back button is tapped.</summary>
    public ICommand? BackCommand
    {
        get => (ICommand?)GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }

    /// <summary>
    /// Icon for the first (left-most) action button on the right side.
    /// On iOS this is an SF Symbol name; on Android a drawable resource name;
    /// for MAUI glass an image filename.
    /// </summary>
    public string? Action1Icon
    {
        get => (string?)GetValue(Action1IconProperty);
        set => SetValue(Action1IconProperty, value);
    }

    /// <summary>Command executed when action button 1 is tapped.</summary>
    public ICommand? Action1Command
    {
        get => (ICommand?)GetValue(Action1CommandProperty);
        set => SetValue(Action1CommandProperty, value);
    }

    /// <summary>
    /// Icon for the second (right-most) action button on the right side.
    /// On iOS this is an SF Symbol name; on Android a drawable resource name;
    /// for MAUI glass an image filename.
    /// </summary>
    public string? Action2Icon
    {
        get => (string?)GetValue(Action2IconProperty);
        set => SetValue(Action2IconProperty, value);
    }

    /// <summary>Command executed when action button 2 is tapped.</summary>
    public ICommand? Action2Command
    {
        get => (ICommand?)GetValue(Action2CommandProperty);
        set => SetValue(Action2CommandProperty, value);
    }

    // ── Events ──────────────────────────────────────────────────────────────

    /// <summary>Raised when the back button is tapped.</summary>
    public event EventHandler? BackButtonClicked;

    /// <summary>Raised when action button 1 is tapped.</summary>
    public event EventHandler? Action1Clicked;

    /// <summary>Raised when action button 2 is tapped.</summary>
    public event EventHandler? Action2Clicked;

    // ── Internal ─────────────────────────────────────────────────────────────

    /// <summary>Called by platform handlers when the back button is tapped.</summary>
    internal void NotifyBackButtonClicked()
    {
        BackButtonClicked?.Invoke(this, EventArgs.Empty);
        if (BackCommand?.CanExecute(null) == true)
            BackCommand.Execute(null);
    }

    /// <summary>Called by platform handlers when action button 1 is tapped.</summary>
    internal void NotifyAction1Clicked()
    {
        Action1Clicked?.Invoke(this, EventArgs.Empty);
        if (Action1Command?.CanExecute(null) == true)
            Action1Command.Execute(null);
    }

    /// <summary>Called by platform handlers when action button 2 is tapped.</summary>
    internal void NotifyAction2Clicked()
    {
        Action2Clicked?.Invoke(this, EventArgs.Empty);
        if (Action2Command?.CanExecute(null) == true)
            Action2Command.Execute(null);
    }
}
