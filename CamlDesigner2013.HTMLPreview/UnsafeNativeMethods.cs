using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace CamlDesigner2013.PreviewControl
{
    internal static class UnsafeNativeMethods
    {
        // ReSharper disable InconsistentNaming
        public const Int32 GWL_STYLE = -16;
        public const Int32 WS_CHILD = 0x40000000;
        public const Int32 WS_VISIBLE = 0x10000000;
        // ReSharper restore InconsistentNaming

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr SetParent(HandleRef hWnd, HandleRef hWndParent);

        [DllImport("user32.dll")]
        public static extern UInt32 SetWindowLong(IntPtr hWnd, Int32 nIndex, UInt32 dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DestroyWindow(IntPtr hWnd);
    }
}
