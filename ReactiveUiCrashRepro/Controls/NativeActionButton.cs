using System.Windows.Input;

namespace ReactiveUiCrashRepro.Controls;

/// <summary>
/// A standalone, platform-native action button.
/// <para>
/// iOS  – renders a <c>UIButton</c> using <c>UIButtonConfiguration</c>.
///         On iOS 26+ the system automatically upgrades the appearance to Liquid Glass.
/// </para>
/// <para>
/// Android – renders a <c>Button</c> styled with a semi-transparent
///            frosted-glass surface colour and elevation shadow.
/// </para>
/// <para>
/// This control is intentionally <em>standalone</em>: it does not drive navigation itself.
/// Wire up <see cref="Clicked"/> or bind <see cref="Command"/> to react to taps.
/// </para>
/// </summary>
public class NativeActionButton : View
{
    // ── Bindable properties ─────────────────────────────────────────────────

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(NativeActionButton),
            defaultValue: string.Empty,
            propertyChanged: (b, _, _) =>
                ((NativeActionButton)b).Handler?.UpdateValue(nameof(Text)));

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(
            nameof(Icon),
            typeof(string),
            typeof(NativeActionButton),
            defaultValue: null,
            propertyChanged: (b, _, _) =>
                ((NativeActionButton)b).Handler?.UpdateValue(nameof(Icon)));

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(
            nameof(Command),
            typeof(ICommand),
            typeof(NativeActionButton),
            defaultValue: null);

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(
            nameof(CommandParameter),
            typeof(object),
            typeof(NativeActionButton),
            defaultValue: null);

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>The button label text.</summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Platform-native icon identifier.
    /// On iOS this is an SF Symbol name (e.g. "plus", "square.and.arrow.up").
    /// On Android this is a drawable resource name (e.g. "ic_add") located in the
    /// platform Resources/drawable folder.  Leave null/empty for a text-only button.
    /// </summary>
    public string? Icon
    {
        get => (string?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>Command executed when the button is tapped.</summary>
    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>Parameter passed to <see cref="Command"/>.</summary>
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>Raised when the user taps the button.</summary>
    public event EventHandler? Clicked;

    // ── Internal ─────────────────────────────────────────────────────────────

    /// <summary>Called by platform handlers when the user taps the button.</summary>
    internal void NotifyClicked()
    {
        Clicked?.Invoke(this, EventArgs.Empty);
        if (Command?.CanExecute(CommandParameter) == true)
            Command.Execute(CommandParameter);
    }
}
