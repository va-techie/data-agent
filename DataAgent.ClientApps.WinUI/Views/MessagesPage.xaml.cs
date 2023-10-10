using System.Windows;
using DataAgent.ClientApps.WinUI.ViewModels;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;

namespace DataAgent.ClientApps.WinUI.Views;

public sealed partial class MessagesPage : Page
{
    public MessagesViewModel ViewModel
    {
        get;
    }

    public MessagesPage()
    {
        ViewModel = App.GetService<MessagesViewModel>();
        ViewModel.DispatcherQueue = DispatcherQueue;
        InitializeComponent();
        this.DataContext = ViewModel;
    }
}
