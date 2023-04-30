using DatabaseMigration;
using DatabaseMigration.Model;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.EspService;
using ServiceLayer.HubService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.PythonService
{
    public class PythonService : IPythonService
    {
        private readonly string _scriptPath = @"C:\Users\emman\OneDrive\Desktop\MDP\MDP07\API\ServiceLayer\PythonService\DrowsinessDetection\DetectionService.py";
        private readonly INotificationService _notificationService;
        private readonly IEspService _espService;
        public PythonService(INotificationService notificationService, IEspService espService)
        {
            _notificationService = notificationService;
            _espService = espService;
        }

        public async Task StartExecutionAsync(string IpCamAddress, string EarThreshold, string WaitTime, string userId, string ipEspAddress)
        {
            await Task.Run(() => ExecuteAsync(_scriptPath, IpCamAddress, EarThreshold, WaitTime, userId, ipEspAddress));
        }

        public async Task StopDetection(string ipEspAddress)
        {
            await _espService.Vibrate(ipEspAddress, false);
        }

        private async Task ExecuteAsync(string scriptPath, string IpCamAddress, string EarThreshold, string WaitTime, string userId, string ipEspAddress)
        {
            string output = "";
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "python";
                startInfo.Arguments = $"{scriptPath} {IpCamAddress} {EarThreshold} {WaitTime}";
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
                    await _notificationService.SendDetectionResult(userId, output);
                    await _espService.Vibrate(ipEspAddress, true);
                };

                process.Start();
                process.BeginOutputReadLine();
            }
            catch (Exception)
            {
                // Handle exception
            }
        }
    }
}
