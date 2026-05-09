using System;
using System.Diagnostics;

namespace Loupedeck.DemoPlugin
{
    public class ShutdownOnTheFlyCommand : PluginDynamicCommand
    {
        public ShutdownOnTheFlyCommand()
            : base("Custom Time", "Prompts for minutes and starts timer", "Power Management")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            String script = "Add-Type -AssemblyName System.Windows.Forms; Add-Type -AssemblyName System.Drawing; $f = New-Object System.Windows.Forms.Form; $f.Text = 'Custom Time'; $f.Size = New-Object System.Drawing.Size(300,165); $f.StartPosition = 'CenterScreen'; $f.FormBorderStyle = 'FixedToolWindow'; $f.BackColor = [System.Drawing.Color]::FromArgb(32,32,32); $f.ForeColor = [System.Drawing.Color]::White; $f.TopMost = $true; $f.Font = New-Object System.Drawing.Font('Segoe UI', 10); $l = New-Object System.Windows.Forms.Label; $l.Text = 'Kac dakika sonra kapansin?'; $l.Location = New-Object System.Drawing.Point(15,15); $l.AutoSize = $true; $t = New-Object System.Windows.Forms.TextBox; $t.Location = New-Object System.Drawing.Point(15,45); $t.Size = New-Object System.Drawing.Size(250,25); $t.BackColor = [System.Drawing.Color]::FromArgb(45,45,45); $t.ForeColor = [System.Drawing.Color]::White; $t.BorderStyle = 'FixedSingle'; $b = New-Object System.Windows.Forms.Button; $b.Text = 'Baslat'; $b.Location = New-Object System.Drawing.Point(15,80); $b.Size = New-Object System.Drawing.Size(250,30); $b.FlatStyle = 'Flat'; $b.FlatAppearance.BorderSize = 0; $b.BackColor = [System.Drawing.Color]::FromArgb(0,120,215); $b.DialogResult = [System.Windows.Forms.DialogResult]::OK; $f.Controls.Add($l); $f.Controls.Add($t); $f.Controls.Add($b); $f.AcceptButton = $b; if ($f.ShowDialog() -eq [System.Windows.Forms.DialogResult]::OK) { Write-Output $t.Text }";

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
                String result = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit();

                if (!String.IsNullOrEmpty(result) && Int32.TryParse(result, out Int32 minutes))
                {
                    Int32 totalSeconds = minutes * 60;
                    Process.Start("shutdown", $"-s -t {totalSeconds}");
                }
            }
        }
    }
}
