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
        private Process _process;
        private bool _processExitedNormally = true;
        public PythonService(INotificationService notificationService, IEspService espService)
        {
            _notificationService = notificationService;
            _espService = espService;
        }

        public async Task StopVibration(string ipEspAddress)
        {
            await _espService.StopVibrate(ipEspAddress);
        }

        public async Task StartExecutionAsync(string IpCamAddress, string EarThreshold, string WaitTime, string userId, string ipEspAddress)
        {
            await Task.Run(() => ExecuteAsync(_scriptPath, IpCamAddress, EarThreshold, WaitTime, userId, ipEspAddress));
        }

        public async Task StopExecutionAsync(string ipEspAddress)
        {
            _processExitedNormally = false;
            var processes = Process.GetProcessesByName("python");
            foreach (var process in processes)
            {
                if(!process.HasExited) process.Kill();
            }
            await _espService.StopVibrate(ipEspAddress);
        }

        public async Task RestartExecutionAsync(string IpCamAddress, string EarThreshold, string WaitTime, string userId, string ipEspAddress)
        {
            var processes = Process.GetProcessesByName("python");
            foreach (var process in processes)
            {
                process.Kill();
            }
            await _espService.StopVibrate(ipEspAddress);
            await Task.Run(() => ExecuteAsync(_scriptPath, IpCamAddress, EarThreshold, WaitTime, userId, ipEspAddress));
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

                _process = new Process();
                _process.StartInfo = startInfo;
                _process.EnableRaisingEvents = true;
                _process.OutputDataReceived += async (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        output += args.Data + "\n";
                    }
                };

                _process.Exited += async (sender, args) =>
                {
                    await _notificationService.SendDetectionResult(userId, output);
                    await _espService.Vibrate(ipEspAddress);
                };

                _process.Start();
                _process.BeginOutputReadLine();
            }
            catch (Exception)
            {
                // Handle exception
            }
        }
    }
}
