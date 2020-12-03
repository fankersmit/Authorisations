using System;
using System.Diagnostics;
using Tests.Helpers;

namespace Tests.Helpers
{
    public class RequestsAppFixture : IDisposable
    {
        private readonly Process _process = null;
        
        public RequestsAppFixture()
        {
            var processName = "RequestsApp";
            var applicationPath = @"C:\Projects\Authorisations\RequestsApp";

            if (!ProcessChecker.IsRunning(processName))
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
                _process.Start();
            }
        }

        private void ReleaseUnmanagedResources()
        {
            if (_process != null && !_process.HasExited)
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