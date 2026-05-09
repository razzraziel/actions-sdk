using System;
using System.Timers;

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
