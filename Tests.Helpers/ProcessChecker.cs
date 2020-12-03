using System;
using System.Diagnostics;

namespace Tests.Helpers
{
    public static class ProcessChecker
    {
        public static bool IsRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }
    }
}