using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace NetNull.ServerManager
{
    internal class Data
    {
        public Process ServerProcess { get; set; }
        public Process LWProcess { get; set; }

        public void Start() {

            if (ServerProcess == null)
            {
                try
                {
                    ProcessStartInfo start = new ProcessStartInfo();
                    //start.Arguments = arguments;
                    start.FileName = "Server.exe";
                    start.WindowStyle = ProcessWindowStyle.Hidden;
                    start.CreateNoWindow = true;
                    ServerProcess = Process.Start(start);
                    //ServerProcess.WaitForExit();
                }
                catch { }
            }

        }

        public void Stop() {

            try
            {
                if (ServerProcess != null)
                {
                    ServerProcess.Dispose();
                    ServerProcess.Close();
                    ServerProcess.Kill();
                }
            }catch { }

        }

        public void Restart() {

            Stop();
            Start();

        }

        public void OpenLogViwe()
        {

            if (ServerProcess == null)
            {
                try
                {
                    ProcessStartInfo start = new ProcessStartInfo();
                    //start.Arguments = arguments;
                    start.FileName = "tools/logview.exe";
                    start.WindowStyle = ProcessWindowStyle.Hidden;
                    start.CreateNoWindow = true;
                    Process.Start(start);
                }
                catch { }
            }

        }

    }
}
