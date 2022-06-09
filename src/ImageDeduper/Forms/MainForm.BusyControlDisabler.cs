# nullable enable

using PW.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ImageDeduper
{
  /// <summary>
  /// Main app form
  /// </summary>
  public partial class MainForm
  {


    /// <summary>
    /// Facilitates disabling and re-enabling of controls with a 'using' block.
    /// A generic version of this would enable use across different Forms.
    /// That would however require delegation of control selection logic.
    /// </summary>
    private class BusyControlDisabler : IDisposable
    {
      private Control[] Controls { get; }

      private BusyControlDisabler(MainForm parent)
      {
        var controls = new List<Control>();
        controls.AddRange(parent.Controls.OfType<Button>().Where(x => x.Enabled == true && x.Visible == true));
        Controls = controls.ToArray();

        Controls.ForEach(x => x.Enabled = false);

      }

      public static BusyControlDisabler Disable(MainForm parent) => new(parent);

      public void Dispose() => Controls.ForEach(x => x.Enabled = true);

    }
  }
}
