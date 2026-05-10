using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Loupedeck.ShutdownTimer
{
    public class PreDefinedTimeCommand : ActionEditorCommand
    {
        private const String MinutesControlName = "MinutesInput";

        public PreDefinedTimeCommand()
        {
            this.Name = "PreDefinedTimeCommand";
            this.DisplayName = "Pre Defined Time";
            this.GroupName = "Power Management";

            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox(MinutesControlName, "Minutes:")
                .SetRequired());

            this.ActionEditor.ControlValueChanged += this.OnControlValueChanged;
        }

        private void OnControlValueChanged(Object sender, ActionEditorControlValueChangedEventArgs e)
        {
            if (e.ControlName.Equals(MinutesControlName, StringComparison.OrdinalIgnoreCase))
            {
                var controlValue = e.ActionEditorState.GetControlValue(MinutesControlName);
                if (Double.TryParse(controlValue.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out Double minutes))
                {
                    if (minutes < 0.5) minutes = 0.5;
                    if (minutes > 7200) minutes = 7200;

                    String displayName;
                    if (minutes >= 60)
                    {
                        Int32 h = (Int32)(minutes / 60);
                        Int32 m = (Int32)(minutes % 60);
                        String hStr = h == 1 ? "hour" : "hours";
                        String mStr = m == 1 ? "min" : "mins";
                        displayName = m == 0 ? $"{h} {hStr}" : $"{h} {hStr} {m} {mStr}";
                    }
                    else if (minutes < 1)
                    {
                        Int32 s = (Int32)(minutes * 60);
                        String sStr = s == 1 ? "sec" : "secs";
                        displayName = $"{s} {sStr}";
                    }
                    else
                    {
                        String mStr = minutes == 1 ? "min" : "mins";
                        displayName = $"{minutes} {mStr}";
                    }

                    e.ActionEditorState.SetDisplayName(displayName);
                }
                else
                {
                    e.ActionEditorState.SetDisplayName("Invalid Time");
                }
            }
        }

        protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
        {
            bool isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

            if (actionParameters.TryGetString(MinutesControlName, out var minutesStr) &&
                Double.TryParse(minutesStr.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out Double minutes))
            {
                if (minutes < 0.5) minutes = 0.5;
                if (minutes > 7200) minutes = 7200;

                Int32 totalSeconds = (Int32)(minutes * 60);

                if (TimerState.ShutdownTime.HasValue)
                {
                    if (!isMac)
                    {
                        Process.Start("shutdown", "-a");
                    }
                }

                if (!isMac)
                {
                    Process.Start("shutdown", $"-s -t {totalSeconds}");
                }

                TimerState.Start(totalSeconds);

                return true;
            }

            return false;
        }
    }
}
