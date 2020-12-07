using System;
using System.Diagnostics;
using Tests.Helpers;

namespace Tests.Helpers
{
    public class RequestsAppFixture : IDisposable
    {
        private static Process _process = null;
        private bool Started = false;
        string _processName = "RequestsApp";
        string _applicationPath = @"C:\Projects\Authorisations\RequestsApp";
        
        public RequestsAppFixture()
        {
            Started = ProcessChecker.IsRunning(_processName);
        }
        
        public void StartRequestsApp()
        {
            if (Started) return;
            
            _process = new Process
            {
                StartInfo =
                {
                    FileName = "dotnet",
                    Arguments = "run",
                    UseShellExecute = false,
                    WorkingDirectory = _applicationPath
                }
            };
            Started = _process.Start();
        }

        private void ReleaseUnmanagedResources()
        {
            if (_process == null) return;
            
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