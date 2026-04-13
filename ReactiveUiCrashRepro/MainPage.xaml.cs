namespace ReactiveUiCrashRepro;

public partial class MainPage
{
    public MainPage()
    {
        BindingContext = new MainPageViewModel();
        InitializeComponent();
    }
}