using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ReportEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OpenRptInVisualStudio("Booking  ECC clinics Report.rpt");
        }

        private void OpenRptInVisualStudio(string reportFileName)
        {
            string reportsPath = Path.Combine("C:\\Users\\mohamed.kandil\\Downloads");
            string fullPath = Path.Combine(reportsPath, reportFileName);

            if (!File.Exists(fullPath))
            {
                MessageBox.Show("Report file not found: " + fullPath);
                return;
            }

            // Method 1: Use devenv.exe directly (most reliable)
            if (TryOpenWithDevEnv(fullPath))
                return;

            // Method 2: Use file association (your current approach)
            if (TryOpenWithFileAssociation(fullPath))
                return;

            // Method 3: Try to find and launch Visual Studio
            TryFindAndLaunchVisualStudio(fullPath);
        }

        private bool TryOpenWithDevEnv(string fullPath)
        {
            try
            {
                // Common Visual Studio installation paths
                string[] vsPaths = {
                    @"C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\devenv.exe",
                    @"C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\devenv.exe",
                    @"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\devenv.exe",
                    @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\devenv.exe",
                    @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.exe",
                    @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\devenv.exe"
                };

                foreach (string vsPath in vsPaths)
                {
                    if (File.Exists(vsPath))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = vsPath,
                            Arguments = $"\"{fullPath}\"",
                            UseShellExecute = false
                        });
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Continue to next method
                Console.WriteLine($"DevEnv method failed: {ex.Message}");
            }
            return false;
        }

        private bool TryOpenWithFileAssociation(string fullPath)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = fullPath,
                    UseShellExecute = true
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File association method failed: {ex.Message}");
                return false;
            }
        }

        private void TryFindAndLaunchVisualStudio(string fullPath)
        {
            try
            {
                // Use vswhere.exe to find Visual Studio installations
                string vsWherePath = @"C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe";

                if (File.Exists(vsWherePath))
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = vsWherePath,
                        Arguments = "-latest -products * -requires Microsoft.VisualStudio.Component.ManagedDesktop -property installationPath",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    };

                    using (Process process = Process.Start(psi))
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();

                        if (!string.IsNullOrEmpty(output))
                        {
                            string vsPath = Path.Combine(output.Trim(), "Common7", "IDE", "devenv.exe");
                            if (File.Exists(vsPath))
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = vsPath,
                                    Arguments = $"\"{fullPath}\"",
                                    UseShellExecute = false
                                });
                                return;
                            }
                        }
                    }
                }

                // Fallback: Show error message
                MessageBox.Show($"Could not find Visual Studio installation.\nPlease open the file manually: {fullPath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open report: {ex.Message}\nFile location: {fullPath}");
            }
        }

        // Alternative: Create a button to browse and open different reports
        private void btnBrowseAndOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Crystal Reports (*.rpt)|*.rpt|All Files (*.*)|*.*";
                ofd.InitialDirectory = @"C:\Users\mohamed.kandil\Downloads";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    OpenRptInVisualStudio(Path.GetFileName(ofd.FileName));
                }
            }
        }
    }
}