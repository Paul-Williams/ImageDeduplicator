using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PW.ImageDeduplicator.Common
{
  // See: https://www.cyotek.com/blog/setting-tab-stops-in-a-windows-forms-textbox-control
  // NB: His declarations sucked and blew up at runtime.
  // See: https://docs.microsoft.com/en-gb/windows/win32/controls/em-settabstops?redirectedfrom=MSDN
  // See: https://stackoverflow.com/questions/1298406/how-to-set-the-tab-width-in-a-windows-forms-textbox-control

  public static class TextBoxExtensions
  {
    public static void SetTapStops(this TextBox tb, int tabSize)
    {
      _ = NativeMethods.SendMessage(tb.Handle, NativeMethods.EM_SETTABSTOPS, 1, new[] { tabSize * 4 });
    }

    public static void SetTapStops(this TextBox tb, params int[] tabSizes)
    {
      if (tabSizes.Length == 0) throw new ArgumentException("Array is empty.", nameof(tabSizes));

      _ = NativeMethods.SendMessage(tb.Handle, NativeMethods.EM_SETTABSTOPS, tabSizes.Length, tabSizes);
    }

    public static void ResetTabStops(this TextBox tb)
    {
      _ = NativeMethods.SendMessage(tb.Handle, NativeMethods.EM_SETTABSTOPS, 0, new[] { 0 });
    }

    private static class NativeMethods
    {
      public const int EM_SETTABSTOPS = 0x00CB;

      //[DllImport("User32", CharSet = CharSet.Auto)]
      //public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

      [DllImport("User32", CharSet = CharSet.Auto)]
      public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int[] lParam);

      //[DllImport("user32", CharSet = CharSet.Auto)]
      //public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref int lParam);

    }
  }
}
