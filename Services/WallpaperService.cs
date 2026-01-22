using System;
using System.Runtime.InteropServices;
using PInvoke;

namespace InteractiveWallpaper.Services
{
    public class WallpaperService
    {
        // Win32 API imports will go here
        private const int WM_SPAWN_WORKER = 0x052C;

        // Delegate for EnumWindows callback
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // Find a window by class name and window name
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Find a child window
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        // Send a message to a window with timeout
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessageTimeout(
            IntPtr hWnd,
            uint Msg,
            IntPtr wParam,
            IntPtr lParam,
            uint fuFlags,
            uint uTimeout,
            out IntPtr lpdwResult);

        // Enumerate all top-level windows
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        // Set a window's parent
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        // Method to find the WorkerW window
        private IntPtr FindWorkerW()
        {
            IntPtr desktopWindowParent = FindWindow("Progman", null);

            if (desktopWindowParent == IntPtr.Zero){
                Console.WriteLine("Desktop parent window not found!");
            }

            //Send message to spawn WorkerW
            IntPtr result = IntPtr.Zero;
            SendMessageTimeout(
                desktopWindowParent,   // Send to Progman
                WM_SPAWN_WORKER,       // The 0x052C message
                IntPtr.Zero,           // No wParam
                IntPtr.Zero,           // No lParam
                0x0000,                // SMTO_NORMAL flags
                1000,                  // 1 second timeout
                out result);           // Result (we don't use it)

            IntPtr workerW = IntPtr.Zero;
            EnumWindows((hWnd, lParam) => 
            {
                // Check if THIS window has SHELLDLL_DefView as a child
                IntPtr child = FindWindowEx(hWnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                
                if (child != IntPtr.Zero)
                {
                    workerW = hWnd;
                    return false; // Stop searching when workerW is found
                }

                return true; // Keep searching
            }, IntPtr.Zero);



            return workerW;
        }
        
        // Main method to set window as wallpaper
        public void SetAsWallpaper(IntPtr windowHandle)
        {
            IntPtr workerW = FindWorkerW();
            
            if (workerW == IntPtr.Zero)
            {
                throw new Exception("Could not find WorkerW window");
            }
            
            SetParent(windowHandle, workerW);
        }
    }
}