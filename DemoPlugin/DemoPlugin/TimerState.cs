using System;
using System.Timers;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Loupedeck.DemoPlugin
{
    public static class TimerState
    {
        public static DateTime? ShutdownTime { get; private set; }
        public static event Action OnTick;

        private static System.Timers.Timer _timer;

        static TimerState()
        {
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += (s, e) =>
            {
                if (ShutdownTime.HasValue && DateTime.Now >= ShutdownTime.Value)
                {
                    ShutdownTime = null;
                    _timer.Stop();

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        ProcessStartInfo psi = new ProcessStartInfo
                        {
                            FileName = "osascript",
                            Arguments = "-e \"tell app 'System Events' to shut down\"",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        Process.Start(psi);
                    }
                }
                OnTick?.Invoke();
            };
        }
        public static void Start(Int32 seconds)
        {
            ShutdownTime = DateTime.Now.AddSeconds(seconds);
            _timer.Start();
            OnTick?.Invoke();
        }

        public static void Cancel()
        {
            ShutdownTime = null;
            _timer.Stop();
            OnTick?.Invoke();
        }
    }
}
