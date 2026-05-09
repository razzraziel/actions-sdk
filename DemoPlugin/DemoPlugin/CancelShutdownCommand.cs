using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Loupedeck.DemoPlugin
{
    public class CancelShutdownCommand : PluginDynamicCommand
    {
        public CancelShutdownCommand()
            : base("Cancel Timer", "Cancels the active shutdown timer", "Power Management")
        {
            TimerState.OnTick += () => this.ActionImageChanged();
        }

        protected override void RunCommand(String actionParameter)
        {
            if (TimerState.ShutdownTime.HasValue)
            {
                bool isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
                if (!isMac)
                {
                    Process.Start("shutdown", "-a");
                }
                TimerState.Cancel();
            }
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            return TimerState.ShutdownTime.HasValue ? "Cancel Timer" : "No Active Timer";
        }
    }
}
