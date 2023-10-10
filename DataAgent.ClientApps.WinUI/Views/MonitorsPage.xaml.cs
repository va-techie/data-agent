using DataAgent.ClientApps.WinUI.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace DataAgent.ClientApps.WinUI.Views;

public sealed partial class MonitorsPage : Page
{
    public MonitorsViewModel ViewModel
    {
        get;
    }

    public MonitorsPage()
    {
        ViewModel = App.GetService<MonitorsViewModel>();
        InitializeComponent();
        this.DataContext = ViewModel;
    }
}
