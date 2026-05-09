using System;
using System.Diagnostics;

namespace Loupedeck.DemoPlugin
{
    public class ShutdownTimerCommand : ActionEditorCommand
    {
        private const String MinutesControlName = "MinutesInput";

        public ShutdownTimerCommand()
        {
            this.Name = "StartTimer";
            this.DisplayName = "Start Timer";
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
                if (Int32.TryParse(controlValue, out Int32 minutes))
                {
                    String displayName = minutes >= 60
                        ? (minutes % 60 == 0 ? $"{minutes / 60} Hour(s)" : $"{minutes / 60}h {minutes % 60}m")
                        : $"{minutes} Min(s)";

                    e.ActionEditorState.SetDisplayName(displayName);
                }
                else
                {
                    e.ActionEditorState.SetDisplayName("Start Timer");
                }
            }
        }

        protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
        {
            if (actionParameters.TryGetString(MinutesControlName, out var minutesStr) &&
                Int32.TryParse(minutesStr, out Int32 minutes))
            {
                Int32 totalSeconds = minutes * 60;
                Process.Start("shutdown", $"-s -t {totalSeconds}");

                return true;
            }

            return false;
        }
    }
}
