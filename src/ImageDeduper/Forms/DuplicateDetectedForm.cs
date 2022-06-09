# nullable enable

using System;
using System.IO;
using System.Windows.Forms;
using CSharpFunctionalExtensions;
using Data.Models;
using ImageDeduper.Controls;
using Prism.Events;
using PW.FailFast;
using PW.IO;
using PW.IO.FileSystemObjects;
using PW.WinForms;
using Unity;
using static PW.WinForms.MsgBox;

namespace ImageDeduper
{
  /// <summary>
  /// Displays pair of detected duplicate images
  /// </summary>
  public partial class DuplicateDetectedForm : Form
  {
    private PW.Flags.SetOnlyFlag DataProvided { get; }

    /// <summary>
    /// 
    /// </summary>
    [Dependency]
    public IEventAggregator EventAggregator { get; set; } = null!; // Injected
    //private const string MustInit = nameof(ProvideData) + " must be called first.";


    private FilePath DuplicateImageFilePath { get; set; }

    private FilePath ExistingImageFilePath { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DuplicateDetectedForm()
    {
      InitializeComponent();
      AttachEventHandlers();
      DuplicateImageFilePath = null!;
      ExistingImageFilePath = null!;
      DataProvided = new();
    }

    private void DuplicateDetectedForm_Load(object sender, EventArgs e)
    {
      Icon = Properties.Resources.App;
    }

    private void AttachEventHandlers()
    {
      NewDuplicateImageControl.DeleteButton_Click += DuplicateImageControl_DeleteButton_Click;
      NewDuplicateImageControl.ReplaceOther_Click += DuplicateImageControl_ReplaceOtherButton_Click;

      ExistingDuplicateImageControl.DeleteButton_Click += DuplicateImageControl_DeleteButton_Click;
      ExistingDuplicateImageControl.ReplaceOther_Click += DuplicateImageControl_ReplaceOtherButton_Click;
    }


    Timer CheckFilePathsTimer { get; } = new Timer() { Interval = 5000, Enabled = false };

    /// <summary>
    /// Timer handler: Closes the form if either image no longer exists on disk.
    /// </summary>
    void CheckFilePathsTimer_Tick(object? sender, EventArgs e)
    {
      if (!DataProvided.IsSet) return;
      if (!DuplicateImageFilePath.Exists || !ExistingImageFilePath.Exists)
      {
        CheckFilePathsTimer.Enabled = false;
        EventAggregator.GetEvent<PubSubEvents.StatusInfoEvent>().Publish($"{nameof(DuplicateDetectedForm)}: Auto-closed : At least one of the images no longer exists.");
        Close(); 
      }
    }


    /// <summary>
    /// Pass the two images
    /// </summary>
    public void ProvideData(ImageEntity existing, ImageEntity duplicate)
    {
      Guard.NotNull(existing, nameof(existing));
      Guard.NotNull(duplicate, nameof(duplicate));

      DuplicateImageFilePath = (FilePath)duplicate.Path;
      ExistingImageFilePath = (FilePath)existing.Path;

      NewDuplicateImageControl.ProvideData("New Image", duplicate);
      ExistingDuplicateImageControl.ProvideData("Existing Image", existing);

      DataProvided.Set();

      EventAggregator.GetEvent<PubSubEvents.FilesDeletedEvent>().Subscribe(OnFilesDeleted, ThreadOption.UIThread);

      // Using timer for the moment as the event (above) does not seem to be working.
      CheckFilePathsTimer.Tick += CheckFilePathsTimer_Tick;
      CheckFilePathsTimer.Enabled = true;

    }

    /// <summary>
    /// Close this form if any of the deleted files are our duplicate/existing files.
    /// </summary>
    private void OnFilesDeleted(FilePath[] files)
    {
      foreach (var file in files)
      {
        if (ExistingImageFilePath == file || DuplicateImageFilePath == file)
        {
          Close();
          return;
        }
      }
    }


    private static Result TryRecycleFile(FileInfo file)
    {
      try
      {
        file.SendToRecycleBin();
        return Result.Success();
      }
      catch (Exception ex)
      {
        return Result.Failure(ex.Message);
      };
    }


    private static void TrySelectInExplorer(ImageEntity? ie)
    {
      if (ie is null) return;
      try
      {
        var f = new FileInfo(ie.Path);
        if (f.Exists) f.SelectInExplorer();
      }
      catch (Exception ex)
      {
        MsgBox.ShowError(ex);
      }
    }

    // Handles click events for both delete buttons.
    private void DuplicateImageControl_DeleteButton_Click(object? sender, EventArgs e)
    {
      if (sender != NewDuplicateImageControl && sender != ExistingDuplicateImageControl)
        throw new InvalidOperationException("Unexpected sender in " + nameof(DuplicateImageControl_DeleteButton_Click));

      var path = (sender as DuplicateImageControl)?.ImageEntity?.Path;

      if (path is null) throw new InvalidOperationException("ImageEntity.Path not available.");

      var file = new FileInfo(path);

      TryRecycleFile(file)
        .OnFailure(msg => ShowError(msg))
        .Tap(() =>
        {
          EventAggregator.GetEvent<PubSubEvents.FilesDeletedEvent>().Publish(new[] { (FilePath)file.FullName });
          Close();
        });
    }


    private void ThrowIfNoData()
    {
      if (!DataProvided) throw new InvalidOperationException("Data not set.");
    }

    private void DuplicateImageControl_ReplaceOtherButton_Click(object? sender, EventArgs e)
    {

      try
      {
        ThrowIfNoData();

        var ctl = sender as DuplicateImageControl;

        string pSrc, pTgt;

        if (ctl == NewDuplicateImageControl)
        {
          pSrc = NewDuplicateImageControl.ImageEntity!.Path;
          pTgt = ExistingDuplicateImageControl.ImageEntity!.Path;
        }
        else
        {
          pSrc = ExistingDuplicateImageControl.ImageEntity!.Path;
          pTgt = NewDuplicateImageControl.ImageEntity!.Path;
        }

        if (!File.Exists(pSrc)) return;
        if (File.Exists(pTgt)) FileSystem.SendFileToRecycleBin(pTgt);
        File.Move(pSrc, pTgt);
        Close();
      }
      catch (Exception ex)
      {
        EventAggregator.GetEvent<PubSubEvents.ExceptionEvent>().Publish(ex);
      }
    }

    private void DeleteBothButton_Click(object sender, EventArgs e)
    {
      try
      {
        ThrowIfNoData();

        var f1 = NewDuplicateImageControl.ImageEntity!.Path;
        var f2 = ExistingDuplicateImageControl.ImageEntity!.Path;

        FileSystem.SendFileToRecycleBin(f1);
        FileSystem.SendFileToRecycleBin(f2);
        Close();
      }
      catch (Exception ex)
      {
        EventAggregator.GetEvent<PubSubEvents.ExceptionEvent>().Publish(ex);
      }
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        CheckFilePathsTimer?.Stop();
        CheckFilePathsTimer?.Dispose();

        components.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}
