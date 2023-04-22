using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.PythonService
{
    public class PythonService : IPythonService
    {
        private readonly string _scriptPath = @"C:\Users\antho\Desktop\MDP07\API\ServiceLayer\PythonService\DrowsinessDetection\DetectionService.py";
        private readonly IHubContext<DetectionHub> _hubContext;
        public PythonService(IHubContext<DetectionHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task StartExecutionAsync()
        {
            await Task.Run(() => ExecuteAsync(_scriptPath));
        }

        private async Task ExecuteAsync(string scriptPath)
        {
            string output = "";
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "python";
                startInfo.Arguments = scriptPath;
                startInfo.RedirectStandardOutput = true;

                Process process = new Process();
                process.StartInfo = startInfo;
                process.EnableRaisingEvents = true;
                process.OutputDataReceived += async (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        output += args.Data + "\n";
                    }
                };

                process.Exited += async (sender, args) =>
                {
                    await _hubContext.Clients.All.SendAsync("DetectionResult", output);
                };

                process.Start();
                process.BeginOutputReadLine();
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }

        public async Task SendMessage(string message)
        {
            await _hubContext.Clients.All.SendAsync("DetectionResult", message);
        }
    }
}
