﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.PythonService
{
    public interface IPythonService
    {
        Task StartExecutionAsync();
        Task SendMessage(string message);
    }
}
