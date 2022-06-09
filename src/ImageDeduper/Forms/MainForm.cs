#nullable enable

using Data;
using Data.Models;
using Prism.Events;
using PubSubEvents;
using PubSubEvents.DatabaseEvents;
using PW.Extensions;
using PW.ImageDeduplicator.Common;
using PW.IO.FileSystemObjects;
using PW.WinForms;
using PW.WinForms.DataBinding;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Unity;
using XnaFan.ImageComparison;
using static PW.WinForms.MsgBox;
using static PW.Extensions.ExceptionExtensions; 

namespace ImageDeduper
{
  public partial class MainForm : Form
  {
    #region Private Fields

    private const string LibraryPath = @"P:\Porn\";

    #endregion Private Fields


    #region Public Constructors

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public MainForm()
    {
      InitializeComponent();
    }

    #endregion Public Constructors

    #region Private Properties

    // This is only model level to support the radio buttons. This could be changed, as is done in other apps.
    private MainFormController? Controller { get; set; }

    /// <summary>
    /// FileSystemWatcher wrapper which runs on its own thread, handles database IO and raises PubSubEvents
    /// </summary>
    //private WatcherHandler? WatcherHandler { get; set; }

    private BackgroundDuplicateImageDetector? DupeDetector { get; set; }

    #endregion Private Properties

    #region Form Event Handlers

    private void Form_Load(object sender, EventArgs e)
    {
      // The form's controller cannot be create in the form's constructor. This is because the UI thread 
      // is not ready at that point, so the event aggregator gets created on a non-UI thread.
      // This is an issue because some events need to be marshalled back to the UI thread for ease of
      // updating form controls.
      // Normally the controller would only be required in the BindControls method, however currently
      // not all controls are bound. The MatchMethodRadioButton_CheckedChanged method is setting a property 
      // on the controller. This could be fixed and then the whole issue would go away!

      Controller = new MainFormController(EA);

      DupeDetector = new BackgroundDuplicateImageDetector(EA);

      Icon = Properties.Resources.App;
      Draggable.Attach(this);

      BindControls();

      ImagesFolderTextBox.Text = new DirectoryInfo(LibraryPath).FullName;


      CreatePubSubEventSubscriptions();

      //var watchDirectory = new DirectoryInfo(LibraryPath);

      LogTextBox.SetTapStops(12);

      // OLD WATCHER HANDLER CODE
      // DEBUG: Comment out while testing new version
      // if (watchDirectory.Exists) WatcherHandler = new WatcherHandler(EventAggregator, watchDirectory);
      // else EventAggregator.GetEvent<StatusInfoEvent>().Publish("Unable to initialize Watcher. Target directory not found: " + watchDirectory.FullName);
      //>

      // DEBUG: Temporary code to test the new version for exceptions before continuing implementation.
      //LibraryWatcher = new ImageLibraryWatcher(watchDirectory, EventAggregator)
      //{
      //  Enabled = true
      //};
      //>
    }

    //private void Form_Closed(object sender, FormClosedEventArgs e) => WatcherHandler?.Dispose();


    #endregion

    #region Private Methods

    private void BindControls()
    {
      if (Controller is null) throw new InvalidOperationException(nameof(Controller) + " should not be null here.");

      //Controller.OnStateChanged += Controller_StateChanged;
      var b = new PW.WinForms.DataBinding.Binder<MainFormController>(Controller);

      b.BindText(ImagesFolderTextBox, nameof(MainFormController.LibraryPathString));
      b.BindForecolor(ImagesFolderTextBox, nameof(MainFormController.LibraryPathIsValid), ConvertEventHandlers.BoolToColor(Color.Black, Color.Red));
      //b.BindChecked(EnableWatcherCheckBox, nameof(MainFormController.LibraryWatcherEnabled));

      Controls.OfType<Button>().ForEach(button => b.BindEnabled(button, nameof(MainFormController.CanStart)));
      //b.BindProperty(x => Compare = x.CompareMethod ,nameof(MainFormController.CompareMethod)) ;
    }

    #endregion Private Methods


    #region Dependency Injection

    /// <summary>
    /// DataContext factory function. For Dependency Injection
    /// </summary>
    [Dependency]
    public Func<DataContext> DataContextFactory { get; set; } = default!;

    /// <summary>
    /// DuplicateDetectedForm factory function. For Dependency Injection
    /// </summary>
    [Dependency]
    public Func<DuplicateDetectedForm> DuplicateDetectedFormFactory { get; set; } = default!;

    /// <summary>
    /// DuplicatesViewerForm factory function. For Dependency Injection
    /// </summary>
    [Dependency]
    public Func<DuplicatesViewerForm> DuplicatesViewerFormFactory { get; set; } = default!;

    /// <summary>
    /// IEventAggregator to be used by the class. For Dependency Injection
    /// </summary>
    [Dependency]
    public IEventAggregator EA { get; set; } = default!; // Prevent CS8618

    // Prevent CS8618

    // Prevent CS8618

    // Prevent CS8618

    #endregion


    /// <summary>
    /// Subscribes to PubSub events
    /// </summary>
    private void CreatePubSubEventSubscriptions()
    {
      var ui = ThreadOption.UIThread;

      //EA.GetEvent<ExceptionEvent>().Subscribe(x => WriteLine($"\r\n*** Exception ***\r\n{x.ToString()}\r\n"), ui);
      EA.GetEvent<ExceptionEvent>().Subscribe(ex => WriteExceptionMessagesToConsole(ex), ui);


      EA.GetEvent<StatusInfoEvent>().Subscribe(WriteLine, ui);
      EA.GetEvent<DuplicateImageAddedEvent>().Subscribe(OnDetectDuplication, ui);
      EA.GetEvent<ImageAddedEvent>().Subscribe(x => WriteLine($"Added:\t{x.FilePath}"), ui);
      EA.GetEvent<ImageUpdatedEvent>().Subscribe(x => WriteLine($"Updated:\t{x.Value}"), ui);
      EA.GetEvent<ImageDeletedEvent>().Subscribe(x => WriteLine($"Deleted:\t{x.Value}"), ui);
      EA.GetEvent<ImageRenamedEvent>().Subscribe(x => WriteLine($"Renamed:\t{x.OldPath.Value}\r\nTo:\t{x.NewPath.Value}"), ui);

      // Writing out this event leads to duplicate event, as file deleted is (nearly) always followed by image (entity) deleted.
      // EA.GetEvent<FilesDeletedEvent>().Subscribe(f => f.ForEach(x => WriteLine($"Deleted:\t{x.Value}\r\n")), ui);      
      EA.GetEvent<DirectoryRenamedEvent>().Subscribe(x => WriteLine($"Directory Renamed:\r\nFrom:\t{x.OldPath.Value}\r\nTo:\t{x.NewPath.Value}\r\n{x.ImagesUpdated} image paths updated.\r\n"), ui);
    }

    /// <summary>
    /// Enables / Disables all buttons.
    /// </summary>
    private void EnableButtons(bool value) => Controls.OfType<Button>().ForEach(x => x.Enabled = value);


    private List<ImageGroupCollection> FindDupesInSingleDirectory(string directory)
    {

      var groups = new List<ImageGroupCollection>(1000);

      var dupeIds = new List<int>(1000);

      using var context = DataContextFactory();

      var imgs = context.ImagesWithinDirectory(directory);

      var s1 = DateTime.Now.Ticks;

      var counter = new PW.Accumulator(1);

      foreach (var img in imgs)
      {
        Application.DoEvents();
        if (dupeIds.Contains(img.Id)) continue;
        var param1 = new SqlParameter("@bytes", img.Bytes)
        {
          SqlDbType = System.Data.SqlDbType.Binary,
          Size = 256
        };
        var param2 = new SqlParameter("@directory", directory)
        {
          SqlDbType = System.Data.SqlDbType.NVarChar,
          Size = 2000
        };
        var result = context.Database.SqlQuery<ImageEntity>("FuzzyMatchBytesWithinDirectory @bytes,@directory", param1, param2).ToList();
        if (result.Count > 1)
        {
          groups.Add(new ImageGroupCollection(result));
          dupeIds.AddRange(result.Select(x => x.Id));
        }
      }

      return groups;

    }

    private List<ImageGroupCollection> OriginalShowDatabaseDuplicatesCode()
    {
      using var context = DataContextFactory();
      var imageInfos = context.Images
        .Where(x => x.Path.StartsWith(ImagesFolderTextBox.Text))

        // NOTE: Found that sorting by .FileSize here returned duplicates missed when sorting by .Bytes
        // When ordered by size and after deleting some duplicates, further duplicates where then found.
        // Further duplicates where found when then re-running with '.OrderByDescending'. 
        // These were not found when reverting back to '.OrderBy', either for .FileSize or .Bytes
        // Order by .Id & .Path also returned more. Basically, this is chuffed!
        .OrderBy(x => x.Bytes) // <-- [18/03/19] Found this is required. Assumed was not as this is the clustered index.
                               //.Select(ImageInfoMapper.ToImageInfo)
        .ToList();

      var dupes = ImageTool.CreateGroupsFromAlreadySortedList(imageInfos, new CloseMatchComparer(4, 10));

      if (dupes.Length != 0)
      {
        var groups = new List<ImageGroupCollection>();
        for (var i = 0; i < dupes.Length; i++)
        {
          var group = new ImageGroupCollection();
          group.AddRange(dupes[i]);
          groups.Add(group);
        }

        return groups.OrderBy(g => g.Images[0].Path).ToList();
      }
      else return new List<ImageGroupCollection>();
    }

    private void WriteExceptionMessagesToConsole(Exception ex)
    {
      var sb = new StringBuilder();
      sb.AppendLine("*** Exception ***");
      ex.EnumerateMessages().ForEach(s => sb.AppendLine(s));
      sb.AppendLine(" ");
      sb.AppendLine(ex.ToString());
      sb.AppendLine(" ");
      LogTextBox.Text = sb.ToString();
    }

    /// <summary>
    /// Writes a line to the top of the console text-box
    /// </summary>
    private void WriteLine(string str)
    {

      const int MaxLines = 50;

      LogTextBox.Lines = str.ReadLines().Concat(LogTextBox.Text.ReadLines()).Take(MaxLines).ToArray();

      LogTextBox.SelectionStart = 0;
      LogTextBox.SelectionLength = 0;

    }
    private void WriteLine(Exception ex) => WriteExceptionMessagesToConsole(ex);  //EA.GetEvent<ExceptionEvent>().Publish(ex);
    #region Button Event Handlers

    private void ClearDatabaseButton_Click(object sender, EventArgs e)
    {
      //if (Ask("Are you really sure?") == AskResult.Yes)
      //{
      //  using (var context = DataContextFactory())
      //    context.Database.ExecuteSqlCommand($"DELETE FROM [{Data.Models.ImageInfoEntity.TableName }]");
      //}
    }

    private void ClearDupeDetectorQueueButton_Click(object sender, EventArgs e)
    {
      try
      {
        DupeDetector?.Clear();
      }
      catch (Exception ex)
      {
        WriteLine(ex);
      }
    }

//    private void HardcoreDedupe()
//    {
//      WriteLine("Disabled.");
//      return;
//#pragma warning disable CS0162 // Unreachable code detected
//      try
//#pragma warning restore CS0162 // Unreachable code detected
//      {
//        EnableButtons(false);

//        if (ShowQuestion("ARE YOU REALLY SURE?!!") != QuestionResult.Yes) return;


//        using var context = DataContextFactory();
//        var images = context.Images.Where(x => x.Path.StartsWith(ImagesFolderTextBox.Text))
//          .OrderBy(x => x.Bytes).ToList(); // Select(ImageInfoMapper.ToImageInfo).ToList();

//        { // Ensure each entity's image exists on disc, otherwise clean-up those entities
//          var orphanedEntities = images.Where(x => !File.Exists(x.Path)).ToList();

//          if (orphanedEntities.Count != 0)
//          {
//            context.Images.RemoveRange(orphanedEntities);
//            context.SaveChanges();
//            //orphanedEntities.ForEach(x => images.Remove(x));

//            images = images.Except(orphanedEntities).ToList();

//          }
//        }


//        //images = images.Where(x => File.Exists(x.Path)).ToList();

//        var dupeGroups = ImageTool.CreateGroupsFromAlreadySortedList(images, new CloseMatchComparer(4, 10));

//        if (dupeGroups.Length != 0)
//        {
//          var removeThese = new List<ImageEntity>();
//          foreach (var group in dupeGroups)
//          {
//            PW.FailFast.Assert.IsFalse(group.Length < 2, "Group has less than two items.");

//            var biggest = group.Aggregate((i, j) => i.FileSize > j.FileSize ? i : j);
//            removeThese.AddRange(group.Where(x => x != biggest));


//            //ImageEntity? biggest = null;

//            //foreach (var item in group)
//            //{
//            //  if (biggest is null) biggest = item;
//            //  else if (item.FileSize > biggest.FileSize)
//            //  {
//            //    removeThese.Add(biggest);
//            //    biggest = item;
//            //  }
//            //  else removeThese.Add(item);
//            //}
//          }

//          Parallel.ForEach(removeThese.Select(x => new FileInfo(x.Path)).ToArray(), new ParallelOptions() { MaxDegreeOfParallelism = 8 },
//            PW.IO.FileInfoExtensions.SendToRecycleBin);

//        }

//      }
//      catch (Exception ex) { WriteLine(ex); }
//      finally { EnableButtons(true); }
//    }

    private void ImageFolderBrowseButton_Click(object sender, EventArgs e)
    {
      try
      {
        using var dlg = new FolderBrowserDialog
        {
          Description = "Select folder containing images to de-duplicated.",
          ShowNewFolderButton = false,
          SelectedPath = ImagesFolderTextBox.Text ?? LibraryPath
        };
        if (dlg.ShowDialog(this) == DialogResult.OK) ImagesFolderTextBox.Text = dlg.SelectedPath;
      }
      catch (Exception ex)
      {
        WriteLine(ex);
      }


    }

    //private void RemoveCertainDirectoriesFromDatabase()
    //{
    //  try
    //  {
    //    using (BusyControlDisabler.Disable(this))
    //    using (var x = DataContextFactory()) x.Database.ExecuteSqlCommand("sp_RemoveNamedFolderImages");
    //  }
    //  catch (Exception ex)
    //  {
    //    WriteLine(ex);
    //  }
    //}
    
    private void ShowDatabaseDuplicatesInViewerButton_Click(object sender, EventArgs e)
    {
      try
      {
        EnableButtons(false);

        var groups = UseStoreProcCheckBox.Checked == false
          ? OriginalShowDatabaseDuplicatesCode()
          : FindDupesInSingleDirectory(ImagesFolderTextBox.Text);

        if (groups.Count != 0)
        {
          var viewer = DuplicatesViewerFormFactory();
          viewer.ProvideData(groups);
          viewer.Show();
        }
        else
        {
          ShowInfo("No duplicate images found.");
        }

      }
      catch (Exception ex)
      {

        WriteLine(ex);
      }
      finally
      {
        EnableButtons(true);
      }
    }

    private async void UpdateDatabaseButton_ClickAsync(object sender, EventArgs e)
    {

      try
      {
        using (BusyControlDisabler.Disable(this))
        {
          var cud = await DatabaseRefresh.PerformRefreshAsync(new DirectoryPath(ImagesFolderTextBox.Text)).ConfigureAwait(true);
          WriteLine($"Created: {cud.Created} - Updated: {cud.Updated} - Deleted: {cud.Deleted}");
        }
      }
      catch (Exception ex)
      {
        WriteExceptionMessagesToConsole(ex);
      }

    }
    #endregion



    #region Misc Event Handlers

    private void ClearToolStripMenuItem_Click(object sender, EventArgs e) => LogTextBox.Clear();

    /// <summary>
    /// Handles <see cref="DuplicateImageAddedEvent"/> PubSubEvent
    /// </summary>    
    private void OnDetectDuplication(DuplicateImageAddedEventArgs ea)
    {
      try
      {
        WriteLine($"Duplicate Detected\r\nNew:\t{ea.Existing.Path}\r\nExisting:\t{ea.Duplicate.Path}");

        var dlg = DuplicateDetectedFormFactory(); ;
        dlg.ProvideData(ea.Existing, ea.Duplicate);
        dlg.Show();
      }
      catch (Exception ex)
      {
        WriteLine(ex);
      }

    }
    #endregion

    private void LogContextMenu_Clear_Click(object sender, EventArgs e) => LogTextBox.Clear();
  }


}
