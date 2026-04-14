using System.Windows.Input;

namespace ReactiveUiCrashRepro.Controls;

public class NativeActionButton : View
{
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
    
    public static readonly BindableProperty ForegroundColorProperty =
        BindableProperty.Create(
            nameof(ForegroundColor),
            typeof(Color),
            typeof(NativeActionButton),
            defaultValue: Color.FromArgb("#00FF00"),
            propertyChanged: (b, _, _) =>
                ((NativeActionButton)b).Handler?.UpdateValue(nameof(ForegroundColor)));

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
    
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    
    public string? Icon
    {
        get => (string?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public Color ForegroundColor
    {
        get => (Color)GetValue(ForegroundColorProperty);
        set => SetValue(ForegroundColorProperty, value);
    }
    
    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
    
    public event EventHandler? Clicked;
    
    internal void NotifyClicked()
    {
        Clicked?.Invoke(this, EventArgs.Empty);
        if (Command?.CanExecute(CommandParameter) == true)
            Command.Execute(CommandParameter);
    }
}
