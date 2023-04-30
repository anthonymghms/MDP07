﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.PythonService
{
    public interface IPythonService
    {
        Task StartExecutionAsync(string IpCamAddress, string EarThreshold, string WaitTime, string userId, string ipEspAddress);
        Task StopDetection(string ipEspAddress);
    }
}
