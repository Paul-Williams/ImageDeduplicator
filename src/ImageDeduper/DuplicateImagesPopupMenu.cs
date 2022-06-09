using Data.Models;
using Prism.Events;
using PW.Extensions;
using PW.IO;
using PW.IO.FileSystemObjects;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static PW.WinForms.MsgBox;

namespace ImageDeduper;

internal class DuplicateImagesContextMenu : ContextMenuStrip
{
  private const string MustInit = nameof(ProvideData) + " must be called first.";

  private ImageGroupCollection? Group { get; set; }

  private ImageEntity[]? Selected { get; set; }

  private readonly IEventAggregator _ea;

  public DuplicateImagesContextMenu(IEventAggregator ea)
  {
    _ea = ea;
    Items.Add(new ToolStripButton("Delete Image File", null, DeleteSelected_Click));
    Items.Add(new ToolStripButton("Show in Windows Explorer", null, ShowInExplorer_Click));
    Items.Add(new ToolStripButton("Open", null, Open_Click));
  }

  private void Open_Click(object? sender, EventArgs e)
  {
    if (Selected is null) return;
    try
    {
      Selected.ForEach(x => new FileInfo(x.Path).Launch());
    }
    catch (Exception ex)
    {
      ShowError(ex);
    }

  }

  private void ShowInExplorer_Click(object? sender, EventArgs e)
  {
    if (Selected is null) return;

    try
    {
      Selected.ForEach(x => new FileInfo(x.Path).SelectInExplorer());
    }
    catch (Exception ex)
    {
      ShowError(ex);
    }

  }

  private void DeleteSelected_Click(object? sender, EventArgs e)
  {
    if (Selected is null || Group is null) throw new InvalidOperationException(MustInit);

    try
    {
      var pairs = Selected.Select(x => (Info: x, File: new FileInfo(x.Path))).ToArray();

      if (pairs.Length == 0) return;

      //string fileStr() => pairs.Length > 1 ? "files" : "file";

      //var question = $"Are you sure you want delete the {fileStr()}?:"
      //  + Environment.NewLine
      //  + string.Join(Environment.NewLine, Selected.Select(x => x.Path));


      //if (Ask(question) == AskResult.Yes)
      //{

      // Delete all first, then remove from group.
      // Prevents bindings loading the image and causing glitches with deletions.
      foreach (var item in pairs.Select(x => x.File)) if (item.Exists) item.SendToRecycleBin();
      foreach (var item in pairs.Select(x => x.Info)) Group.Images.Remove(item);

      _ea.GetEvent<PubSubEvents.FilesDeletedEvent>().Publish(pairs.Select(x => (FilePath)x.File).ToArray());

      //}


    }
    catch (Exception ex)
    {
      ShowError(ex, "Error deleting file.");
    }


  }


  public void ProvideData(ImageGroupCollection group, ImageEntity[] selected)
  {
    Group = group;
    Selected = selected;
    if (group is null || selected is null || selected.Length == 0)
      foreach (ToolStripItem menu in Items) { menu.Enabled = false; }
  }




}
