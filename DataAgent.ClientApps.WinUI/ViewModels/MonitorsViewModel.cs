using CommunityToolkit.Mvvm.ComponentModel;
using DataAgent.API.Monitoring;

namespace DataAgent.ClientApps.WinUI.ViewModels;

public partial class MonitorsViewModel : ObservableRecipient
{
    public MonitorsViewModel()
    {
        DataAgent.API.Services.Providers.PublisherProvider.GetAllPublishers(out var publishers);
        Monitors = publishers.ToList();
    }

    public IReadOnlyList<IMessagePublisher> Monitors
    {
        get; private set;
    }
}
