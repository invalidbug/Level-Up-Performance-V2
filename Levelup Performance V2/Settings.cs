using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Levelup_Performance_V2 {
    public partial class Settings : Form {
        public Settings() {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e) {
            RefreshProcesses();
        }

        private void button1_Click(object sender, EventArgs e) {
            RefreshProcesses();
        }

        private void RefreshProcesses() {
            listBox1.Items.Clear();
            List<Process> processes = Process.GetProcesses().ToList();
            Brain.CheckProcess(processes);
            foreach (var p in processes) {
                if (Brain.json.BannedProcList.Contains(p.ProcessName)) {
                    continue;
                }
                listBox1.Items.Add(p.ProcessName);
            }

            listBox2.Items.Clear();
            foreach (string i in Brain.json.BannedProcList) {
                listBox2.Items.Add(i);
            }
            Brain.json.Save();
        }

        private void button3_Click(object sender, EventArgs e) {
            if (listBox1.SelectedItem != null) {
                Brain.json.BannedProcList.Add((string)listBox1.SelectedItem);
                listBox2.Items.Add((string)listBox1.SelectedItem);
                listBox1.Items.Remove((string)listBox1.SelectedItem);
                Refresh();
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            if (listBox2.SelectedItem != null) {
                Brain.json.BannedProcList.Remove((string)listBox2.SelectedItem);
                listBox1.Items.Add((string)listBox2.SelectedItem);
                listBox2.Items.Remove((string)listBox2.SelectedItem);
                Refresh();
            }
        }
    }
}
