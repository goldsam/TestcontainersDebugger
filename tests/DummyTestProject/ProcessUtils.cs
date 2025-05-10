using System.Diagnostics;
using System.Management;

public static class ProcessUtils
{
    public static Process GetParentProcess(Process process)
    {
        using (var query = new ManagementObjectSearcher($"SELECT * FROM Win32_Process WHERE ProcessId={process.Id}"))
        {
            foreach (ManagementObject obj in query.Get())
            {
                var parentId = Convert.ToInt32(obj["ParentProcessId"]);
                try
                {
                    return Process.GetProcessById(parentId);
                }
                catch (ArgumentException)
                {
                    // Process might have exited.
                    return null;
                }
            }
        }
        return null;
    }
}
