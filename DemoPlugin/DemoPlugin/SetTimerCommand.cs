using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Loupedeck.DemoPlugin
{
    public class SetTimerCommand : PluginDynamicCommand
    {
        public SetTimerCommand()
            : base("Set Timer", "Prompts for minutes and starts timer", "Power Management")
        {
            TimerState.OnTick += () => this.ActionImageChanged();
        }

        protected override void RunCommand(String actionParameter)
        {
            bool isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            String result = "";

            if (isMac)
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "osascript",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    process.StandardInput.WriteLine("tell application \"System Events\"");
                    process.StandardInput.WriteLine("activate");
                    process.StandardInput.WriteLine("set response to display dialog \"How many minutes until shutdown?\" default answer \"60\" buttons {\"Cancel\", \"Start\"} default button 2");
                    process.StandardInput.WriteLine("text returned of response");
                    process.StandardInput.WriteLine("end tell");
                    process.StandardInput.Close();

                    result = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();
                }
            }
            else
            {
                String script = "Add-Type -AssemblyName System.Windows.Forms; Add-Type -AssemblyName System.Drawing; $f = New-Object System.Windows.Forms.Form; $f.Text = 'Set Timer'; $f.Size = New-Object System.Drawing.Size(300,165); $f.StartPosition = 'CenterScreen'; $f.FormBorderStyle = 'FixedToolWindow'; $f.BackColor = [System.Drawing.Color]::FromArgb(32,32,32); $f.ForeColor = [System.Drawing.Color]::White; $f.TopMost = $true; $f.Font = New-Object System.Drawing.Font('Segoe UI', 10); $l = New-Object System.Windows.Forms.Label; $l.Text = 'How many minutes until shutdown?'; $l.Location = New-Object System.Drawing.Point(15,15); $l.AutoSize = $true; $t = New-Object System.Windows.Forms.TextBox; $t.Location = New-Object System.Drawing.Point(15,45); $t.Size = New-Object System.Drawing.Size(250,25); $t.BackColor = [System.Drawing.Color]::FromArgb(45,45,45); $t.ForeColor = [System.Drawing.Color]::White; $t.BorderStyle = 'FixedSingle'; $b = New-Object System.Windows.Forms.Button; $b.Text = 'Start'; $b.Location = New-Object System.Drawing.Point(15,80); $b.Size = New-Object System.Drawing.Size(250,30); $b.FlatStyle = 'Flat'; $b.FlatAppearance.BorderSize = 0; $b.BackColor = [System.Drawing.Color]::FromArgb(0,120,215); $b.DialogResult = [System.Windows.Forms.DialogResult]::OK; $f.Controls.Add($l); $f.Controls.Add($t); $f.Controls.Add($b); $f.AcceptButton = $b; if ($f.ShowDialog() -eq [System.Windows.Forms.DialogResult]::OK) { Write-Output $t.Text }";

                String encoded = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(script));

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = $"-NoProfile -EncodedCommand {encoded}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    result = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();
                }
            }

            if (!String.IsNullOrEmpty(result) && Double.TryParse(result.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out Double minutes))
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
            }
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            if (TimerState.ShutdownTime.HasValue)
            {
                TimeSpan t = TimerState.ShutdownTime.Value - DateTime.Now;
                if (t.TotalSeconds > 0)
                {
                    if (t.TotalHours >= 1)
                    {
                        return $"{(Int32)t.TotalHours}:{t.Minutes:D2}:{t.Seconds:D2}";
                    }
                    else if (t.TotalMinutes >= 1)
                    {
                        return $"{t.Minutes}:{t.Seconds:D2}";
                    }
                    else
                    {
                        return $"{t.Seconds}";
                    }
                }
            }
            return "Set Timer";
        }
    }
}
