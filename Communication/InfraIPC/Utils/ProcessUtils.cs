using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Intel.IntelConnect.IPC.Utils
{
    internal static class ProcessUtils
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] uint dwFlags,
            [Out] StringBuilder lpExeName, [In, Out] ref uint lpdwSize);

        public static FileInfo GetProcessFile(int processId)
        {
            var processById = Process.GetProcessById(processId);
            return GetProcessFilePath(processById)!;
        }

        private static FileInfo? GetProcessFilePath(Process process)
        {
            var fileNameBuilder = new StringBuilder(2048);
            var bufferLength = (uint)fileNameBuilder.Capacity + 1;

            return QueryFullProcessImageName(process.Handle, 0, fileNameBuilder, ref bufferLength)
                ? new FileInfo(fileNameBuilder.ToString())
                : null;
        }

    }
}
