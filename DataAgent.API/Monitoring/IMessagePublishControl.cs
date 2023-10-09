using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAgent.API.Monitoring;
public interface IMessagePublishControl
{
    bool IsEnabled
    {
        get; set;
    }
}
