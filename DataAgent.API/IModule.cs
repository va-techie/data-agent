using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAgent.API;

public interface IModule
{
    void Initialize();
    void Shutdown();

}
