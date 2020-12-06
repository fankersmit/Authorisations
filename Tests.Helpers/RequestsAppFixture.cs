using System;
using System.Diagnostics;
using Tests.Helpers;

namespace Tests.Helpers
{
    public class RequestsAppFixture : IDisposable
    {
        private readonly Process _process = null;
        private bool Started = false;
        
        public RequestsAppFixture()
        {
            var processName = "RequestsApp";
            var applicationPath = @"C:\Projects\Authorisations\RequestsApp";

            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                _process = processes[0];
            }
            else
            {
                _process = new Process
                {
                    StartInfo =
                    {
                        FileName = "dotnet",
                        Arguments = "run",
                        UseShellExecute = false,
                        WorkingDirectory = applicationPath
                    }
                };
            }
        }
        
        public void StartRequestsApp()
        {
            Started = _process.Start();
        }

        private void ReleaseUnmanagedResources()
        {
            if (Started && !_process.HasExited)
            {
                _process.Kill(true);
            }
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~RequestsAppFixture()
        {
            ReleaseUnmanagedResources();
        }
    }
}