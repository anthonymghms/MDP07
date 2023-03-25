using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.PythonService
{
    public class PythonService
    {
        public async Task<string> ExecuteAsync(string scriptPath)
        {
            string output = "";
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = "python.exe";
                start.Arguments = scriptPath;
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                using (Process process = Process.Start(start))
                {
                    using (System.IO.StreamReader reader = process.StandardOutput)
                    {
                        output = await reader.ReadToEndAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
            }

            return output;
        }
    }
}
