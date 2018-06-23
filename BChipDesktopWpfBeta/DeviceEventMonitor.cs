using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace BChipDesktop
{
    internal static class DeviceEventMonitor
    {
        public const int DbtDeviceArrival = 0x00008000;       
        public const int DbtDeviceRemoveComplete = 0x00008004; 
        public const int DbtDevNodesChanged = 0x00000007; 
        public const int DbyDevTypDeviceInterface = 0x00000005;
        public const int WmDevicechange = 0x0219; //WM_EVENT      
        private const int DbtDevtypDeviceinterface = 0x00000005;
        private const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x00000004;
        private static IntPtr notificationHandle;

        public static void RegisterDeviceNotification(IntPtr windowHandle)
        {
            var dbi = new DEV_BROADCAST_DEVICEINTERFACE
            {
                dbcc_devicetype = DbtDevtypDeviceinterface,
                dbcc_reserved = 0,
                dbcc_classguid = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED")
            };

            dbi.dbcc_size = Marshal.SizeOf(dbi);
            IntPtr buffer = Marshal.AllocHGlobal(dbi.dbcc_size);
            Marshal.StructureToPtr(dbi, buffer, true);

            notificationHandle = RegisterDeviceNotification(windowHandle, buffer, DEVICE_NOTIFY_ALL_INTERFACE_CLASSES);
        }

        public static void UnregisterDeviceNotification()
        {
            UnregisterDeviceNotification(notificationHandle);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

        [DllImport("user32.dll")]
        private static extern bool UnregisterDeviceNotification(IntPtr handle);

        [StructLayout(LayoutKind.Sequential)]
        internal class DEV_BROADCAST_HDR
        {
            public int dbch_size;
            public int dbch_devicetype;
            public int dbch_reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DEV_BROADCAST_DEVICEINTERFACE
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
            public Guid dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public char[] dbcc_name;
        }
    }
}
