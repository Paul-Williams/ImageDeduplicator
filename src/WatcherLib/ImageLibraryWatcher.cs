﻿#nullable enable

using Data.Models;
using Prism.Events;
using PubSubEvents;
using PubSubEvents.DatabaseEvents;
using PW.Extensions;
using PW.IO.FileSystemObjects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ImageDeduper
{
  // This class is a WIP replacement for WatcherHandler, in an attempt to simplify things.
  // The blocking collection for deleted directories should later be replaced with a class to also service renamed and created directories.

  public class ImageLibraryWatcher
  {

    /// <summary>
    /// Constructor
    /// </summary>
    public ImageLibraryWatcher(DirectoryPath libraryDirectory!!, IEventAggregator ea!!)
    {
      //EA = ea ?? throw new ArgumentNullException(nameof(ea));
      LibraryDirectory = libraryDirectory;

      //ImageAddedEvent = ea.GetEvent<PubSubEvents.DatabaseEvents.ImageAddedEvent>();
      ////ImageDeletedEvent = EA.GetEvent<PubSubEvents.DatabaseEvents.ImageDeletedEvent>();
      //ImageRenamedEvent = ea.GetEvent<PubSubEvents.DatabaseEvents.ImageRenamedEvent>();
      //FilesDeletedEvent = ea.GetEvent<FilesDeletedEvent>();
      //ImageUpdatedEvent = ea.GetEvent<PubSubEvents.DatabaseEvents.ImageUpdatedEvent>();
      //StatusInfoEvent = ea.GetEvent<StatusInfoEvent>();
      ExceptionEvent = ea.GetEvent<ExceptionEvent>();

      FileEventBuffer = new FileSystemEventBuffer<FilePath>();
      FileWatcher = CreateFileWatcher();

      DirectoryEventBuffer = new FileSystemEventBuffer<DirectoryPath>();
      DirectoryWatcher = CreateDirectoryWatcher();

      //EventListProcessorThread = new Thread(new ParameterizedThreadStart(EventListProcessorThreadProc)) { IsBackground = true, };

      Thread = new EventListProcessorThread(DirectoryEventBuffer, FileEventBuffer, ea);

    }

    private DirectoryPath LibraryDirectory { get; }

    //private IEventAggregator EA { get; }

    #region PubSub Events
    //EA.GetEvent<PubSubEvents.DatabaseEvents.ImageAddedEvent>().Publish(entity);
    //private ImageAddedEvent ImageAddedEvent { get; }
    ////private ImageDeletedEvent ImageDeletedEvent { get; }
    //private ImageRenamedEvent ImageRenamedEvent { get; }
    //private FilesDeletedEvent FilesDeletedEvent { get; }
    //private ImageUpdatedEvent ImageUpdatedEvent { get; }
    //private StatusInfoEvent StatusInfoEvent { get; }

    private ExceptionEvent ExceptionEvent { get; }

    #endregion


    private EventListProcessorThread Thread { get; }


    /// <summary>
    /// Buffer for file events generated by a FileSystemWatcher.
    /// </summary>
    private FileSystemEventBuffer<FilePath> FileEventBuffer { get; }

    /// <summary>
    /// Buffer for directory events generated by a FileSystemWatcher.
    /// </summary>
    private FileSystemEventBuffer<DirectoryPath> DirectoryEventBuffer { get; }

    /// <summary>
    /// File system watched dedicated to file events
    /// </summary>
    private FileSystemWatcher FileWatcher { get; }

    /// <summary>
    /// File system watched dedicated to directory events
    /// </summary>
    private FileSystemWatcher DirectoryWatcher { get; }

    ///// <summary>
    ///// Thread to process the file and directory event buffers.
    ///// </summary>
    //private Thread EventListProcessorThread { get; set; }

    ///// <summary>
    ///// Token for cancelling the thread which processes the file and directory event buffers.
    ///// </summary>
    //private CancellationTokenSource? ThreadCancellationToken { get; set; }

    /// <summary>
    /// Determines if a file type is for a supported image.
    /// </summary>
    private static bool IsSupportedFileType(string path)
    {
      var ext = Path.GetExtension(path);
      return ext.Length != 0 && PW.Drawing.Imaging.GdiImageDecoderFormats.IsSupported(ext);
    }

    /// <summary>
    /// Creates a file system watcher dedicated to files.
    /// </summary>
    private FileSystemWatcher CreateFileWatcher()
    {
      var watcher = new FileSystemWatcher((string)LibraryDirectory)
      {
        IncludeSubdirectories = true,       // See: http://benhall.io/notifyfilters-enumeration-explained-filesystemwatcher/
        NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime
      };

      watcher.Created += (o, e) => { if (IsSupportedFileType(e.FullPath)) FileEventBuffer.AddCreated((FilePath)e.FullPath); };
      watcher.Deleted += (o, e) => { if (IsSupportedFileType(e.FullPath)) FileEventBuffer.AddDeleted((FilePath)e.FullPath); };
      watcher.Changed += (o, e) => { if (IsSupportedFileType(e.FullPath)) FileEventBuffer.AddChanged((FilePath)e.FullPath); };

      // Windows Photos (and likely other programs) create temporary files and rename the original during editing.
      // In an attempt to ignore these extra events we will filter out those events where EITHER of the paths is not a supported
      // file type.
      watcher.Renamed += (o, e) =>
      {
        if (IsSupportedFileType(e.FullPath) && IsSupportedFileType(e.OldFullPath))
          FileEventBuffer.AddRenamed((FilePath)e.FullPath, (FilePath)e.OldFullPath);
      };

      watcher.Error += Watcher_Error;

      return watcher;
    }

    /// <summary>
    /// Creates a file system watcher dedicated to directories.
    /// </summary>
    private FileSystemWatcher CreateDirectoryWatcher()
    {
      var watcher = new FileSystemWatcher((string)LibraryDirectory)
      {
        IncludeSubdirectories = true,       // See: http://benhall.io/notifyfilters-enumeration-explained-filesystemwatcher/
        NotifyFilter = NotifyFilters.DirectoryName
      };

      watcher.Created += (o, e) => DirectoryEventBuffer.AddCreated((DirectoryPath)e.FullPath); // { if (IsSupported(e.FullPath)) FileEventBuffer.AddCreated((FilePath)e.FullPath); };
      watcher.Deleted += (o, e) => DirectoryEventBuffer.AddDeleted((DirectoryPath)e.FullPath);  // DeletedDirectories.Add((DirectoryPath)e.FullPath);
      //watcher.Changed += (o, e) => { if (IsSupported(e.FullPath)) FileEventBuffer.AddChanged((FilePath)e.FullPath); };

      watcher.Renamed += (o, e) => DirectoryEventBuffer.AddRenamed((DirectoryPath)e.FullPath, (DirectoryPath)e.OldFullPath);
      //{
      //  if (IsSupported(e.FullPath) && IsSupported(e.OldFullPath))
      //    FileEventBuffer.AddRenamed((FilePath)e.FullPath, (FilePath)e.OldFullPath);
      //};

      watcher.Error += Watcher_Error;

      return watcher;
    }

    /// <summary>
    /// Receives error events from both FileSystemWatchers.
    /// </summary>
    private void Watcher_Error(object sender, ErrorEventArgs e)
    {
      ExceptionEvent.Publish(e.GetException());
    }

    ///// <summary>
    ///// Thread-Proc for processing of items in the event list.
    ///// </summary>
    //private void EventListProcessorThreadProc(object obj)
    //{

    //  var token = (CancellationToken)obj;
    //  if (token.IsCancellationRequested) return;
    //  Thread.Sleep(10000);

    //  while (!token.IsCancellationRequested)
    //  {
    //    try
    //    {

    //      #region Process directory event buffer

    //      // DEBUG: Test code to remove all images for deleted directories.
    //      // TODO: Look at adding logic to combine directory events, as with file events
    //      var directoryEvents = new FileSystemEventCollection<DirectoryPath>(DirectoryEventBuffer.TakeAll());

    //      if (directoryEvents.Count != 0)
    //      {
    //        using var db = new DatabaseUpdater(EA);

    //        var toDelete = directoryEvents.Deleted.Select(x => x.Path);

    //        foreach (var directory in toDelete)
    //        {
    //          if (token.IsCancellationRequested) return;
    //          var count = db.DeleteAllImages(directory);
    //          StatusInfoEvent.Publish($"Directory deleted: {directory.Value} - {count} images deleted.");
    //        }


    //        var toCreate = directoryEvents.Created.Select(x => x.Path).ToArray();

    //        foreach (var directory in toCreate)
    //        {
    //          if (token.IsCancellationRequested) return;
    //          /*  
    //           *  Notes:
    //           *  Only get images from top-level, as directory-created event is fired individually for each sub-directory.
    //           *  Images added here are duplicated by events triggered by the file-event-watcher, however I do not trust
    //           *  that it always triggers for new directories. Duplicate image-created events will be combined later anyway.
    //           *  
    //           */
    //          FileSystem.EnumerateGdiSupportedImages(directory, SearchOption.TopDirectoryOnly)
    //            .ForEach(FileEventBuffer.AddCreated);
    //        }

    //      }
    //      #endregion


    //      #region Process file event buffer


    //      var events = FileSystemEventCollection<FilePath>.Combine(FileEventBuffer.TakeAll());

    //      if (events.Count != 0)
    //      {
    //        if (token.IsCancellationRequested) return;

    //        using var db = new DatabaseUpdater(EA);

    //        if (token.IsCancellationRequested) return;

    //        // DEBUG: Test code for delete images
    //        var toDelete = events.Deleted.Select(x => x.Path).ToArray();

    //        if (toDelete.Length != 0)
    //        {
    //          toDelete.ForEach(db.DeleteImage);
    //          // HACK: FilePath not yet supported everywhere, convert to FileInfo.
    //          FilesDeletedEvent.Publish(toDelete.Select(x => new FileInfo(x.ToString())).ToArray());
    //        }




    //        if (token.IsCancellationRequested) return;

    //        // DEBUG: Test code for rename images
    //        var toRename = events.Renamed.Select(x => (NewPath: x.Path, OldPath: x.OldPath));

    //        foreach (var paths in toRename)
    //        {
    //          db.ChangePath(paths);
    //          // Hack -- Why does this event need the entity?
    //          ImageRenamedEvent.Publish(new ImageEntity() { Path = paths.OldPath.ToString() });
    //        }



    //        if (token.IsCancellationRequested) return;

    //        var upsertFailures = new List<FileSystemEvent<FilePath>>();

    //        // DEBUG: Test code for Upsert
    //        var toUpsert = events.CreatedOrChanged;

    //        foreach (var x in toUpsert)
    //        {
    //          if (token.IsCancellationRequested) return;
    //          var (entity, isNew) = db.UpsertImage(x.Path, x.OldPath);

    //          if (entity is ImageEntity)
    //          {
    //            if (isNew) ImageAddedEvent.Publish(entity);
    //            else ImageUpdatedEvent.Publish(entity);
    //          }
    //          else upsertFailures.Add(x);
    //        }

    //        if (token.IsCancellationRequested) return;

    //        foreach (var fileEvent in upsertFailures)
    //        {
    //          FileEventBuffer.RequeueFailedEvent(fileEvent);
    //          StatusInfoEvent.Publish("Re-queueing file: " + fileEvent.Path.ToString());
    //        }

    //        if (token.IsCancellationRequested) return;
    //      }

    //      #endregion



    //      if (!token.IsCancellationRequested) Thread.Sleep(5000);
    //    }
    //    catch (Exception ex)
    //    {
    //      ExceptionEvent.Publish(ex);
    //      DebugHelper.WriteLineThreadId(ex.ToString());
    //    }
    //  }

    //}

    private bool _enabled;
    private readonly object _enabledLock = new();

    /// <summary>
    /// Whether the watch is enabled.
    /// </summary>
    public bool Enabled
    {
      get => _enabled;
      set
      {
        lock (_enabledLock)
        {
          // Only continue if the value has changed.
          if (!PW.BackingField.AssignIfNotEqual(value, ref _enabled)) return;

          Thread.Enabled = value;

          FileWatcher.EnableRaisingEvents = value;
          DirectoryWatcher.EnableRaisingEvents = value;



          //  if (value == true) // Enabling thread
          //  {
          //    if (!EventListProcessorThread.IsAlive)
          //    {
          //      ThreadCancellationToken?.Dispose();
          //      ThreadCancellationToken = new CancellationTokenSource();
          //      EventListProcessorThread.Start(ThreadCancellationToken.Token);
          //    }
          //  }
          //  else // Disabling thread
          //  {
          //    ThreadCancellationToken?.Cancel();
          //    ThreadCancellationToken?.Dispose();
          //    ThreadCancellationToken = null;
          //  }
        } // lock released


      }
    }

  }

}