using System.IO.Pipes;
using System.Runtime.InteropServices;

namespace Intel.IntelConnect.IPC.Extensions
{
    internal static class NamedPipesExtensions
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetNamedPipeClientProcessId(IntPtr pipe, out int clientProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetNamedPipeServerProcessId(IntPtr pipe, out int clientProcessId);

        /// <summary>
        ///
        /// </summary>
        /// <param name="pipeServer"></param>
        /// <returns></returns>
        internal static int GetNamedPipeClientProcessId(this NamedPipeServerStream pipeServer)
        {
            var hPipe = pipeServer.SafePipeHandle.DangerousGetHandle();

            if (GetNamedPipeClientProcessId(hPipe, out var clientProcessId)) return clientProcessId;

            var error = Marshal.GetLastWin32Error();
            return error;
        }

        internal static int GetNamedPipeServerProcessId(this NamedPipeClientStream pipeClient)
        {
            var hPipe = pipeClient.SafePipeHandle.DangerousGetHandle();

            if (GetNamedPipeServerProcessId(hPipe, out var serverProcessId)) return serverProcessId;

            var error = Marshal.GetLastWin32Error();
            return error;
        }
    }
}
