using System;
using System.Diagnostics;

namespace VAK.Types
{
    internal class ProcessInfo(Process process)
    {
        internal string ProcessName { get; set; } = process.ProcessName;
        internal IntPtr MainWindowHandle { get; set; } = process.MainWindowHandle;
        internal string MainWindowTitle { get; set; } = process.MainWindowTitle;

        internal string DisplayName => string.Format("{0} - {1}", ProcessName, MainWindowTitle);
    }
}
