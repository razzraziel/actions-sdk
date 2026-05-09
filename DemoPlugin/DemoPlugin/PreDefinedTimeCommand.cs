using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Loupedeck.DemoPlugin
{
    public class PreDefinedTimeCommand : ActionEditorCommand
    {
        private const String MinutesControlName = "MinutesInput";

        public PreDefinedTimeCommand()
        {
            this.Name = "PreDefinedTime";
            this.DisplayName = "Pre Defined Time";
            this.GroupName = "Power Management";

            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox(MinutesControlName, "Minutes:")
                .SetRequired());

            this.ActionEditor.ControlValueChanged += this.OnControlValueChanged;
            TimerState.OnTick += () => this.ActionImageChanged();
        }

        private void OnControlValueChanged(Object sender, ActionEditorControlValueChangedEventArgs e)
        {
            if (e.ControlName.Equals(MinutesControlName, StringComparison.OrdinalIgnoreCase))
            {
                var controlValue = e.ActionEditorState.GetControlValue(MinutesControlName);
                if (Double.TryParse(controlValue.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out Double minutes))
                {
                    String displayName;
                    if (minutes >= 60)
                    {
                        Int32 h = (Int32)(minutes / 60);
                        Int32 m = (Int32)(minutes % 60);
                        displayName = m == 0 ? $"{h} Hour(s)" : $"{h}h {m}m";
                    }
                    else
                    {
                        displayName = $"{minutes} Min(s)";
                    }

                    e.ActionEditorState.SetDisplayName(displayName);
                }
                else
                {
                    e.ActionEditorState.SetDisplayName("Pre Defined Time");
                }
            }
        }

        protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
        {
            bool isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

            if (actionParameters.TryGetString(MinutesControlName, out var minutesStr) &&
                Double.TryParse(minutesStr.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out Double minutes))
            {
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
