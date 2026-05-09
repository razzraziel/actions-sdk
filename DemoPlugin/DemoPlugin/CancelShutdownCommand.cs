using System;
using System.Diagnostics;

namespace Loupedeck.DemoPlugin
{
    public class CancelShutdownCommand : PluginDynamicCommand
    {
        public CancelShutdownCommand()
            : base("Cancel Timer", "Cancels the active shutdown timer", "Power Management")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Process.Start("shutdown", "-a");
        }
    }
}
