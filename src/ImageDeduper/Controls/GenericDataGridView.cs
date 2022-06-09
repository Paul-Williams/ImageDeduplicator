//// Moved code to: PW.WinForms.Controls

//using System.Drawing;
//using System.Linq;
//using System.Windows.Forms;

//namespace ImageDeduper
//{

//  /// <summary>
//  /// Generic (typed) version of the standard <see cref="DataGridView"/> control.
//  /// </summary>
//  /// <typeparam name="T"></typeparam>
//  public class GenericDataGridView<T> : DataGridView
//  {

//    /// <summary>
//    /// Creates a new instance of the control
//    /// </summary>
//    public GenericDataGridView()
//    {
//      SelectionMode = DataGridViewSelectionMode.FullRowSelect;
//      RowHeadersVisible = false;
//      BackgroundColor = Color.White;
//      Dock = DockStyle.Fill;
//      AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
//      AllowUserToResizeRows = false;
//    }

//    /// <summary>
//    /// Returns the first select data bound item.
//    /// </summary>
//    public T SelectedItem()
//    {
//      var rows = SelectedItems();
//      return rows.Length == 0 ? default(T) : rows[0];
//    }

//    /// <summary>
//    /// Returns all selected data bound items.
//    /// </summary>
//    public T[] SelectedItems()
//    {
//      var rows = SelectedRows;
//      return rows.Count == 0 ? new T[] { } : rows.OfType<DataGridViewRow>().Select(x => (T)x.DataBoundItem).ToArray();
//    }

//  }
//}
