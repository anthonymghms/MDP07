using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.PythonService
{
    public interface IPythonService
    {
        Task StartExecutionAsync(string IpCamAddress, string EarThreshold, string WaitTime, string userId, string ipEspAddress);
        Task StopExecutionAsync(string ipEspAddress);
        Task RestartExecutionAsync(string IpCamAddress, string EarThreshold, string WaitTime, string userId, string ipEspAddress);
        Task StopVibration(string ipEspAddress);
    }
}
