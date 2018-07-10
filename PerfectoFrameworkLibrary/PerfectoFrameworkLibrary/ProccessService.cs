// Decompiled with JetBrains decompiler
// Type: PerfectoLab.ProccessService
// Assembly: PerfectoLab, Version=10.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 531781DB-4194-4A8D-B8E1-DB2C0966BF16
// Assembly location: C:\Users\under\Documents\Visual Studio 2015\Projects\PerfectoLabSeleniumTestProject4\packages\PerfectoLab.10.3.0.0\lib\PerfectoLab.dll

using System.Diagnostics;

namespace PerfectoLab
{
    public class ProccessService
    {
        public static int GetVisualStudioId()
        {
            Process currentProcess = Process.GetCurrentProcess();
            return !(currentProcess.ProcessName == "devenv") ? ProccessService.GetParent(currentProcess).Id : currentProcess.Id;
        }

        private static string FindIndexedProcessName(int pid)
        {
            string processName = Process.GetProcessById(pid).ProcessName;
            Process[] processesByName = Process.GetProcessesByName(processName);
            string instanceName = (string)null;
            for (int index = 0; index < processesByName.Length; ++index)
            {
                instanceName = index == 0 ? processName : processName + "#" + (object)index;
                if ((int)new PerformanceCounter("Process", "ID Process", instanceName).NextValue() == pid)
                    return instanceName;
            }
            return instanceName;
        }

        private static Process FindPidFromIndexedProcessName(string indexedProcessName)
        {
            return Process.GetProcessById((int)new PerformanceCounter("Process", "Creating Process ID", indexedProcessName).NextValue());
        }

        private static Process GetParent(Process process)
        {
            return ProccessService.FindPidFromIndexedProcessName(ProccessService.FindIndexedProcessName(process.Id));
        }
    }
}
