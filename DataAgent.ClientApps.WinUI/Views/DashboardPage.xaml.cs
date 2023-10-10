using DataAgent.ClientApps.WinUI.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace DataAgent.ClientApps.WinUI.Views;

public sealed partial class DashboardPage : Page
{
    public DashboardViewModel ViewModel
    {
        get;
    }

    public DashboardPage()
    {
        ViewModel = App.GetService<DashboardViewModel>();
        InitializeComponent();
    }
}
