using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Loupedeck.ShutdownTimer
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

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (TimerState.ShutdownTime.HasValue)
            {
                using (var bitmapBuilder = new BitmapBuilder(imageSize))
                {
                    var activeRed = new BitmapColor(180, 0, 0);
                    bitmapBuilder.Clear(activeRed);

                    bitmapBuilder.DrawText("Cancel");

                    return bitmapBuilder.ToImage();
                }
            }

            return null;
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            return TimerState.ShutdownTime.HasValue ? "" : "No Active Timer";
        }
    }
}
