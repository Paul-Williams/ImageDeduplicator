using Data.Models;
using Prism.Events;
using PW.IO;
using PW.IO.FileSystemObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Unity;
using static PW.WinForms.MsgBox;

namespace ImageDeduper;

/// <summary>
/// Displays groups of duplicate images
/// </summary>
public partial class DuplicatesViewerForm : Form
{
  /// <summary>
  /// 
  /// </summary>
  [Dependency]
  public IEventAggregator EA { get; set; } = null!; // Ignore null warning


  private readonly BindingSource BindingSource = new() { DataSource = typeof(ImageGroupCollection) };

  /// <summary>
  /// 
  /// </summary>
  public DuplicatesViewerForm()
  {
    InitializeComponent();

    BindingSource.CurrentItemChanged += BindingSource_CurrentItemChanged;
    DataGrid.SelectionChanged += DataGrid_SelectionChanged;
    DataGrid.DataError += DataGrid_DataError;
    BindingSource.ListChanged += BindingSource_ListChanged;
  }

  private void DataGrid_DataError(object? sender, DataGridViewDataErrorEventArgs e) => ShowError(e.Exception, "DataGrid Binding Error");

  private void BindingSource_ListChanged(object? sender, ListChangedEventArgs e)
  {
    var removeList = new List<ImageGroupCollection>();

    foreach (ImageGroupCollection group in BindingSource.List) if (group.Images.Count < 2) removeList.Add(group);

    if (removeList.Count != 0)
    {
      removeList.ForEach(x => BindingSource.Remove(x));
      BindingSource.ResetBindings(false);
    }

    GroupCountLabel.Text = BindingSource.List.Count.ToString() + " Groups";

  }

  private void BindingSource_CurrentItemChanged(object? sender, EventArgs e)
  {
    if (BindingSource.Current is ImageGroupCollection group)
    {
      DataGrid.DataSource = group.Images;
    }
    else
    {
      DataGrid.DataSource = null;
    }
  }

  private void Form_Load(object sender, EventArgs e)
  {
    Icon = Properties.Resources.App;
    GroupsListBox.DataSource = BindingSource;

    // Value as set in properties window is getting lost, for some reason...
    HorizontalSplitContainer.SplitterWidth = 10;
  }

  /// <summary>
  /// Data to be displayed
  /// </summary>
  /// <param name="groups"></param>
  public void ProvideData(List<ImageGroupCollection> groups)
  {
    BindingSource.Clear();
    BindingSource.DataSource = new BindingList<ImageGroupCollection>(groups);
  }

  /// <summary>
  /// Clean up any resources being used.
  /// </summary>
  /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
  protected override void Dispose(bool disposing)
  {
    if (disposing && (components != null))
    {
      components.Dispose();

    }
    BindingSource.Dispose();
    base.Dispose(disposing);
  }


  #region DataGrid event handlers

  private void DataGrid_MouseClick(object sender, MouseEventArgs e)
  {
    if (e.Button != MouseButtons.Right) return;
    if (BindingSource.Current is not ImageGroupCollection group) return;

    if (DataGrid.ContextMenuStrip is null) DataGrid.ContextMenuStrip = new DuplicateImagesContextMenu(EA);
    ((DuplicateImagesContextMenu)DataGrid.ContextMenuStrip)
      .ProvideData(group, DataGrid.SelectedItems().ToArray());

    DataGrid.ContextMenuStrip.Show(DataGrid, e.Location);


  }
  private void DataGrid_KeyDown(object sender, KeyEventArgs e)
  {
    // This code is copy-paste-modify duplicate of that in DuplicateImagesContextMenu !!
    try
    {
      if (e.KeyCode == Keys.Delete)
      {
        if (!(BindingSource.Current is ImageGroupCollection Group)) return;
        var Selected = DataGrid.SelectedItems();

        if (Selected.Count == 0) return;

        var paths = Selected.Select(x => (FilePath)x.Path).ToArray();

        // Delete all first, then remove from group.
        // Prevents bindings loading images and causing glitches with deletions.
        foreach (var path in paths) if (path.Exists) FileSystem.SendToRecycleBin(path);
        foreach (var item in Selected) Group.Images.Remove(item);

        EA.GetEvent<PubSubEvents.FilesDeletedEvent>().Publish(paths);
      }
    }
    catch (Exception ex)
    {
      EA.GetEvent<PubSubEvents.ExceptionEvent>().Publish(ex);
    }

  }

  private void DataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
  {
    try
    {
      if (DataGrid.SelectedItem() is ImageEntity item) new FileInfo(item.Path).Launch();
    }
    catch (Exception ex)
    {
      ShowError(ex);
    }
  }

  private void DataGrid_SelectionChanged(object? sender, EventArgs e)
  {
    if (DataGrid.SelectedItem() is ImageEntity item) PictureBox.ImageLocation = item.Path;
  }



  #endregion


}

