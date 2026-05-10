namespace Loupedeck.ShutdownTimer
{
    using System;

    public class ShutdownTimerApplication : ClientApplication
    {
        public ShutdownTimerApplication()
        {
        }

        protected override String GetProcessName() => "DemoApplication";

        protected override String GetBundleName() => "";
    }
}
