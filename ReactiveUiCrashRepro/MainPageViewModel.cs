using System.Windows.Input;
using ReactiveUI;

namespace ReactiveUiCrashRepro;

public class MainPageViewModel : ReactiveObject
{
    public MainPageViewModel()
    {
        ClickCommand1 = ReactiveCommand.CreateFromTask(ClickCommand1Execute);
        ClickCommand2 = ReactiveCommand.CreateFromTask(ClickCommand2Execute);
    }
    
    public ICommand ClickCommand1 { get; }
    public ICommand ClickCommand2 { get; }

    private async Task ClickCommand1Execute()
    {
        Console.WriteLine("Clicked 1");
        await Task.Delay(5);
        Console.WriteLine("Finished 1");
    }
    
    private async Task ClickCommand2Execute()
    {
        Console.WriteLine("Clicked 2");
        await Task.Delay(5);
        Console.WriteLine("Finished 2");
    }
}