using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.Diagnostics;
using Microsoft.Win32;
using System.Management.Instrumentation;
using System.Runtime.InteropServices;
using System.IO;

namespace Nero_System_Information
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();


            ManagementObjectSearcher mo = new ManagementObjectSearcher("select * from Win32_Processor");

        }


        private void Main_Load(object sender, EventArgs e)
        {

            timer1.Start();

            GetGPUInfo();

            var cpu =
new ManagementObjectSearcher("select * from Win32_Processor")
.Get()
.Cast<ManagementObject>()
.First();

            var wmi =
    new ManagementObjectSearcher("select * from Win32_OperatingSystem")
    .Get()
    .Cast<ManagementObject>()
    .First();

            textBox1.Text = System.Environment.UserName;
            string windowsName = GetWindowsVersion();
            textBox2.Text = windowsName;
            textBox3.Text = System.Environment.UserDomainName;
            textBox4.Text = System.Environment.Version.ToString();
            textBox5.Text = System.Environment.SystemPageSize.ToString();
            textBox6.Text = System.Environment.SystemDirectory.ToString();
            textBox7.Text = System.Environment.Is64BitOperatingSystem.ToString();
            textBox8.Text = System.Environment.Is64BitProcess.ToString();
            textBox9.Text = System.Environment.ProcessorCount.ToString();
            textBox10.Text = System.Environment.OSVersion.Platform.ToString();
            textBox14.Text = HardwareInfo.GetProcessorId();
            textBox16.Text = (string)cpu["ProcessorId"]; ;
            textBox17.Text = (string)cpu["SocketDesignation"];
            textBox18.Text = (string)cpu["Caption"];
            textBox21.Text = (string)wmi["OSArchitecture"];
            textBox42.Text = GetTotalRAM();
            textBox39.Text = GetRAMSpeed();
            textBox37.Text = GetRAMManufacturer();
            textBox33.Text = GetUsedRAM();
            textBox29.Text = GetFreeRAM();
            DisplayScreenInfo();
            UpdateBatteryInfo();

            int q = System.Environment.TickCount;
            int w = (q / 1000) / 60;

            textBox11.Text = w.ToString() + "  Minute(s)";

            RegistryKey Rkey = Registry.LocalMachine;
            Rkey = Rkey.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0");

            textBox12.Text = (string)Rkey.GetValue("ProcessorNameString");

            textBox15.Text = (string)Rkey.GetValue("ProcessorNameString");

            textBox13.Text = System.Environment.WorkingSet.ToString();

            ManagementClass management2 = new ManagementClass("Win32_BIOS");
            ManagementObjectCollection managementobject2 = management2.GetInstances();
            foreach (ManagementObject mngObject2 in managementobject2)
            {
                textBox22.Text = mngObject2.Properties["SerialNumber"].Value.ToString();
                break;


            }

        }

        public double GetHDDSize(string drive)
        {

            if (string.IsNullOrEmpty(drive) || drive == null)
            {
                drive = "C";
            }

            ManagementObject disk = new ManagementObject("Win32_LogicalDisk.DeviceID=\"" + drive + ":\"");

            disk.Get();

            return Convert.ToDouble(disk["Size"]);


        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void GroupBox3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cmbCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private string GetWindowsVersion()
        {
            string productName = string.Empty;
            try
            {

                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                if (key != null)
                {

                    object productValue = key.GetValue("ProductName");
                    if (productValue != null)
                    {
                        productName = productValue.ToString();
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error retrieving Windows version: " + ex.Message);
            }

            return productName;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            float fcpu = pCPU.NextValue();
            float fram = pRAM.NextValue();

            progressBar1.Value = (int)fcpu;

            lblCPU.Text = string.Format("{0:0.00}%", fcpu);

            chart1.Series["CPU"].Points.AddY(fcpu);
        }

        public static class ConverterExtension
        {
            private const long Kb = 1024;
            private const long Mb = Kb * 1024;
            private const long Gb = Mb * 1024;
            private const long Tb = Gb * 1024;

        }

        private string GetTotalRAM()
        {
            string totalRAM = string.Empty;
            try
            {

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
                ulong totalCapacity = 0;
                foreach (ManagementObject obj in searcher.Get())
                {
                    totalCapacity += (ulong)obj["Capacity"];
                }

                totalRAM = (totalCapacity / (1024 * 1024 * 1024)).ToString() + " GB";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving RAM info: " + ex.Message);
            }

            return totalRAM;

        }

        private string GetRAMSpeed()
        {
            string ramSpeed = string.Empty;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Speed FROM Win32_PhysicalMemory");
                foreach (ManagementObject obj in searcher.Get())
                {
                    ramSpeed = obj["Speed"].ToString() + " MHz";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving RAM speed: " + ex.Message);
            }

            return ramSpeed;

        }

        private string GetRAMManufacturer()
        {
            string manufacturer = string.Empty;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Manufacturer FROM Win32_PhysicalMemory");
                foreach (ManagementObject obj in searcher.Get())
                {
                    manufacturer = obj["Manufacturer"].ToString();
                    break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving RAM manufacturer: " + ex.Message);
            }

            return manufacturer;
        }

        private string GetRAMType()
        {
            string ramType = string.Empty;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT MemoryType FROM Win32_PhysicalMemory");
                foreach (ManagementObject obj in searcher.Get())
                {
                    ramType = ConvertMemoryType((ushort)obj["MemoryType"]);
                    break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving RAM type: " + ex.Message);
            }

            return ramType;
        }

        private string ConvertMemoryType(ushort type)
        {
            switch (type)
            {
                case 20:
                    return "DDR";
                case 21:
                    return "DDR2";
                case 24:
                    return "DDR3";
                case 26:
                    return "DDR4";
                default:
                    return "Unknown";

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = GetNumberOfRAMModules().ToString();
        }

        private int GetNumberOfRAMModules()
        {
            int numberOfModules = 0;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
                foreach (ManagementObject obj in searcher.Get())
                {
                    numberOfModules++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving number of RAM modules: " + ex.Message);
            }

            return numberOfModules;

        }

        private string GetUsedRAM()
        {
            string usedRAM = string.Empty;
            try
            {

                var currentProcess = Process.GetCurrentProcess();
                long usedMemory = currentProcess.PrivateMemorySize64;
                usedRAM = (usedMemory / (1024 * 1024)).ToString() + " MB";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving used RAM: " + ex.Message);
            }

            return usedRAM;

        }

        private string GetFreeRAM()
        {
            string freeRAM = string.Empty;
            try
            {

                using (var counter = new PerformanceCounter("Memory", "Available MBytes"))
                {
                    float availableMemory = counter.NextValue();
                    freeRAM = availableMemory.ToString() + " MB";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving free RAM: " + ex.Message);
            }

            return freeRAM;

        }

        private void UpdateBatteryInfo()
        {
            try
            {

                PowerStatus powerStatus = SystemInformation.PowerStatus;
                int batteryPercentage = (int)(powerStatus.BatteryLifePercent * 100);


                progressBar2.Value = batteryPercentage;


                labelBatteryPercentage.Text = batteryPercentage.ToString() + "%";


                if (powerStatus.PowerLineStatus == PowerLineStatus.Online)
                {
                    labelChargingStatus.Text = "Charging";
                }
                else
                {
                    labelChargingStatus.Text = "Not Charging";
                }


                labelPowerScheme.Text = GetPowerScheme();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private string GetPowerScheme()
        {
            PowerStatus powerStatus = SystemInformation.PowerStatus;
            if (powerStatus.PowerLineStatus == PowerLineStatus.Online)
            {
                return "Plugged In (Balanced/High Performance)";
            }
            else
            {
                return "On Battery (Power Saver)";
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("control.exe", "/name Microsoft.PowerOptions");
        }

        private void DisplayScreenInfo()
        {
            try
            {

                Screen primaryScreen = Screen.PrimaryScreen;


                labelScreenWidth.Text = "Width: " + primaryScreen.Bounds.Width.ToString() + " pixels";
                labelScreenHeight.Text = "Height: " + primaryScreen.Bounds.Height.ToString() + " pixels";
                labelScreenBitsPerPixel.Text = "Bits per pixel: " + primaryScreen.BitsPerPixel.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("control.exe", "/name Microsoft.Display");
        }

        private void GetGPUInfo()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_VideoController");

                foreach (ManagementObject obj in searcher.Get())
                {
                    GPUName.Text = "Name: " + obj["Name"];
                    VideoProcessor.Text = "Video Processor: " + obj["VideoProcessor"];
                    CurrentResolution.Text = "Current Resolution: " + obj["CurrentHorizontalResolution"] + " x " + obj["CurrentVerticalResolution"];
                    StatusGPU.Text = "" + obj["Status"];
                    VideoMode.Text = "" + obj["VideoModeDescription"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Beta_MSG betaForm = new Beta_MSG();
            betaForm.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Beta_MSG betaForm = new Beta_MSG();
            betaForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Beta_MSG betaForm = new Beta_MSG();
            betaForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Beta_MSG betaForm = new Beta_MSG();
            betaForm.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Beta_MSG betaForm = new Beta_MSG();
            betaForm.Show();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Beta_MSG betaForm = new Beta_MSG();
            betaForm.Show();
        }
    }
}