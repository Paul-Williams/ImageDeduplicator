# nullable enable

using System;
using System.Linq;
using System.Windows.Forms;

namespace ImageDeduper.Controls
{
  internal class RadioGroupBox : GroupBox
  {
    public event EventHandler SelectedChanged = delegate { };

    private int Selected_BackingField;
    public int Selected
    {
      get => Selected_BackingField;
      set
      {
        var val = 0;
        var radioButton = Controls.OfType<RadioButton>()
            .FirstOrDefault(radio =>
                radio.Tag != null
               && int.TryParse(radio.Tag.ToString(), out val) && val == value);

        if (radioButton != null)
        {
          radioButton.Checked = true;
          Selected_BackingField = val;
        }
      }
    }

    protected override void OnControlAdded(ControlEventArgs e)
    {
      base.OnControlAdded(e);

      if (e.Control is RadioButton radioButton)
        radioButton.CheckedChanged += RadioButton_CheckedChanged;
    }

    private void RadioButton_CheckedChanged(object? sender, EventArgs e)
    {
      var radio = sender as RadioButton ?? throw new ArgumentNullException(nameof(sender));
      if (radio.Checked && radio.Tag != null
           && int.TryParse(radio.Tag.ToString(), out var val))
      {
        Selected_BackingField = val;
        SelectedChanged(this, new EventArgs());
      }
    }
  }
}
