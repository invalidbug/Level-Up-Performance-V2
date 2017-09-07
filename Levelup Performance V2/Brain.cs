using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using Newtonsoft.Json;
using System.Windows.Forms;
namespace Levelup_Performance_V2 {
    public class LevelUpJSONData {
        public List<string> BannedProcList;

        public void Load() {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                          @"\LevelUp Performance\data.json";
            if (!File.Exists(path)) {
                BannedProcList = new List<string>();
                return;
            }
            var a = JsonConvert.DeserializeObject<LevelUpJSONData>(File.ReadAllText(path));
            this.BannedProcList = a.BannedProcList;
        }

        public void Save() {
            var a = JsonConvert.SerializeObject(this);
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                      @"\LevelUp Performance\");
            File.WriteAllText(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\LevelUp Performance\data.json",
                a);
        }
    }

    [StandardModule]
    internal sealed class Brain {
        public static LevelUpJSONData json;

        [DllImport("kernel32.dll", EntryPoint = "QueryDosDevice")]
        private static extern uint DosDevice(string name, StringBuilder path, uint length);

        [DllImport("psapi.dll", EntryPoint = "GetProcessImageFileName")]
        private static extern uint ProcessFileName(IntPtr handle, StringBuilder name, uint size);


        public static void CheckProcess(List<Process> p) {
            List<Process> proctoremove = new List<Process>();
            foreach (var proc in p) {
                if (json.BannedProcList.Contains(proc.ProcessName)) {
                    proctoremove.Add(proc);
                }
            }
            if (proctoremove.Count > 0) {
                foreach (var pr in proctoremove) {
                    p.Remove(pr);
                }
            }
            p.Remove(System.Diagnostics.Process.GetCurrentProcess());
        }

        private static string ProcessLocation(IntPtr handle) {
            StringBuilder stringBuilder = new StringBuilder(512);
            if ((long)ProcessFileName(handle, stringBuilder, 512U) > 0L) {
                string str1 = stringBuilder.ToString();
                string[] logicalDrives = Environment.GetLogicalDrives();
                int index = 0;
                while (index < logicalDrives.Length) {
                    string str2 = logicalDrives[index];
                    if ((long)DosDevice(str2.Substring(0, 2), stringBuilder, 512U) > 0L && str1.StartsWith(stringBuilder.ToString()))
                        return Path.GetFullPath(str2 + str1.Remove(0, stringBuilder.Length)).ToLower();
                    checked { ++index; }
                }
            }
            return string.Empty;
        }

        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        private static extern bool SetWorkingSet(IntPtr handle, int minimum, int maximum);

        public static int Kill(bool doWhitelist = true,bool displaydebug = true) {
            string[] array = new string[2]
            {
        Environment.SystemDirectory.ToLower(),
        Path.GetDirectoryName(Environment.SystemDirectory).ToLower()
            };
            string Right = ProcessLocation(Process.GetCurrentProcess().Handle);
            Process[] processes = Process.GetProcesses();
            int index = 0;
            int num = 0;
            while (index < processes.Length) {
                Process process = processes[index];
                try {
                    string str = ProcessLocation(process.Handle);
                    if (!string.IsNullOrEmpty(str) & Operators.CompareString(str, Right, false) != 0) {
                        if (Array.IndexOf<string>(array, Path.GetDirectoryName(str)) == -1) {
                            if (json.BannedProcList.Contains(process.ProcessName)) {
                                if (doWhitelist)
                                {
                                    ++index;
                                    continue;
                                }
                            }
                            if(displaydebug)
                                MessageBox.Show(process.ProcessName + Environment.NewLine + str);
                            process.Kill();
                            checked { ++num; }
                        }
                    }
                } catch (Exception ex) {
                    ProjectData.SetProjectError(ex);
                    ProjectData.ClearProjectError();
                }
                checked { ++index; }
            }
            return num;
        }

        public static int Free() {
            Process[] processes = Process.GetProcesses();
            int index = 0;
            int num = 0;
            while (index < processes.Length) {
                Process process = processes[index];
                try {
                    if (SetWorkingSet(process.Handle, -1, -1))
                        checked { ++num; }
                } catch (Exception ex) {
                    ProjectData.SetProjectError(ex);
                    ProjectData.ClearProjectError();
                }
                checked { ++index; }
            }
            return num;
        }
    }
}
