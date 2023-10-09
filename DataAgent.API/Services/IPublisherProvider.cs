using DataAgent.API.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAgent.API.Services;

public interface IPublisherProvider
{
    IMessagePublisher? GetPublisher(string name);

    void GetAllPublishers(out IMessagePublisher[] publisher);
}
