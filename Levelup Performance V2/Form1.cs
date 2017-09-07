using System;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using System.Diagnostics;
namespace Levelup_Performance_V2 {
    public partial class Form1 : Form {
        private Computer _computer;
        private PerformanceCounter theCPUCounter;
        public Form1() {
            InitializeComponent();
            _computer = new Computer();
            theCPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        private void button1_Click(object sender, EventArgs e) {
            
            MessageBox.Show(string.Format("Killed {0} of processes.", Brain.Kill(true,false)));
        }

        private void button2_Click(object sender, EventArgs e) {
            
            MessageBox.Show(string.Format("Freed {0} MB. Memory may still free up dramatically over the next 5-10 seconds.", Brain.Free()));
        }

        private void DoMemory() {
            // memory
            memoryBar.Maximum = ((int)Math.Round((_computer.Info.TotalPhysicalMemory / 1024.0 / 1024.0)));
            float displacement = (_computer.Info.TotalPhysicalMemory - _computer.Info.AvailablePhysicalMemory) / 1024.0f / 1024.0f;
            this.memoryBar.Value = ((int)displacement);
            this.memoryLabel.Text = string.Format("{0}/{1} MB", memoryBar.Value, memoryBar.Maximum);
        }

        private void DoCPU() {
            // cpu
            this.cpuBar.Maximum = 100;
            this.cpuBar.Value = (int)theCPUCounter.NextValue();
            this.cpuLabel.Text = string.Format("{0}%", cpuBar.Value);
        }

        private void tempTimer_Tick(object sender, EventArgs e) {
            DoMemory();

            DoCPU();
        }

        private void Form1_Load(object sender, EventArgs e) {
            var json = new LevelUpJSONData();
            json.Load();
            Brain.json = json;

            _computer = new Computer();
            theCPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");


            tempTimer.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            Brain.json.Save();
        }

        private void button3_Click(object sender, EventArgs e) {
            Settings s = new Settings();
            s.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Brain.Kill(true, true);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Brain.Kill(false, true);
        }
    }
}
