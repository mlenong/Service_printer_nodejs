using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Threading;

namespace ServicePrintTray
{
    public class ServicePrintTray : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private Process backendProcess;
        private const string BackendExecutable = "service-backend.exe";
        private static Mutex mutex = null;
        private const string AppName = "ServicePrintAppMutex";

        public ServicePrintTray()
        {
            // Initialize Tray Icon
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Info", OnInfo);
            trayMenu.MenuItems.Add("Exit", OnExit);

            trayIcon = new NotifyIcon();
            trayIcon.Text = "Service Print - Active";
            
            // Robust Icon Loading
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "printer.ico");
            if (File.Exists(iconPath))
            {
                trayIcon.Icon = new Icon(iconPath);
            }
            else
            {
                // Fallback to extraction if printer.ico is missing
                trayIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }

            // Set the Main Form icon to match
            this.Icon = trayIcon.Icon;

            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
            trayIcon.DoubleClick += new EventHandler(OnInfo);

            // Hide the form
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Load += (s, e) => this.Hide();

            // Start Backend
            StartBackend();

            // Set Startup Registry
            SetStartup();
        }

        private void StartBackend()
        {
            try
            {
                Process[] pname = Process.GetProcessesByName("service-backend");
                if (pname.Length == 0)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = BackendExecutable;
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden; // Run hidden
                    startInfo.CreateNoWindow = true;
                    startInfo.UseShellExecute = false;
                    startInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    
                    backendProcess = Process.Start(startInfo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting backend service: " + ex.Message, "Service Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetStartup()
        {
            try
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                rk.SetValue("ServicePrint", Application.ExecutablePath);
            }
            catch (Exception)
            {
                // Verify permissions or specific errors if needed
            }
        }

        private void OnInfo(object sender, EventArgs e)
        {
            // Professional Info Window
            Form infoForm = new Form();
            infoForm.Text = "Service Print Info";
            infoForm.Size = new Size(400, 250);
            infoForm.StartPosition = FormStartPosition.CenterScreen;
            infoForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            infoForm.MaximizeBox = false;
            infoForm.MinimizeBox = false;
            // Set Info Window Icon
            if (trayIcon.Icon != null) infoForm.Icon = trayIcon.Icon;
            
            // Icon
            PictureBox iconBox = new PictureBox();
            iconBox.Size = new Size(64, 64);
            iconBox.Location = new Point(20, 20);
            iconBox.SizeMode = PictureBoxSizeMode.StretchImage;
            if (trayIcon.Icon != null) iconBox.Image = trayIcon.Icon.ToBitmap();
            infoForm.Controls.Add(iconBox);

            // Title Label
            Label titleLabel = new Label();
            titleLabel.Text = "Service Print";
            titleLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            titleLabel.Location = new Point(100, 20);
            titleLabel.AutoSize = true;
            infoForm.Controls.Add(titleLabel);

            // Subtitle/Vendor Label
            Label vendorLabel = new Label();
            vendorLabel.Text = "by Mlenong";
            vendorLabel.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            vendorLabel.ForeColor = Color.Gray;
            vendorLabel.Location = new Point(105, 50);
            vendorLabel.AutoSize = true;
            infoForm.Controls.Add(vendorLabel);

            // Status Label
            Label statusLabel = new Label();
            statusLabel.Text = "Status: Service Running";
            statusLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            statusLabel.ForeColor = Color.Green;
            statusLabel.Location = new Point(105, 80);
            statusLabel.AutoSize = true;
            infoForm.Controls.Add(statusLabel);

            // Port Info
            Label portLabel = new Label();
            portLabel.Text = "Listening on: http://localhost:3000";
            portLabel.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            portLabel.Location = new Point(105, 105);
            portLabel.AutoSize = true;
            infoForm.Controls.Add(portLabel);

            // OK Button
            Button okButton = new Button();
            okButton.Text = "OK";
            okButton.Size = new Size(100, 35);
            okButton.Location = new Point(150, 160);
            okButton.DialogResult = DialogResult.OK;
            infoForm.Controls.Add(okButton);
            infoForm.AcceptButton = okButton;

            infoForm.ShowDialog();
        }

        private void OnExit(object sender, EventArgs e)
        {
            // Kill backend process if we started it
            if (backendProcess != null && !backendProcess.HasExited)
            {
                try
                {
                    backendProcess.Kill();
                }
                catch { }
            }
            
            Process[] backendProcesses = Process.GetProcessesByName("service-backend");
            foreach(Process p in backendProcesses)
            {
                   try { p.Kill(); } catch {}
            }

            trayIcon.Visible = false;
            Application.Exit();
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window
            ShowInTaskbar = false; // Remove from taskbar
            base.OnLoad(e);
        }

        [STAThread]
        static void Main()
        {
            bool createdNew;
            mutex = new Mutex(true, AppName, out createdNew);

            if (!createdNew)
            {
                // App is already running!
                MessageBox.Show("Servis sudah aktif.\nCek icon printer di System Tray (pojok kanan bawah).", 
                    "Service Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Application.Run(new ServicePrintTray());
            
            // Keep the mutex reference alive until the normal termination of the program
            GC.KeepAlive(mutex);
        }
    }
}
