using Prism.Events;
using PubSubEvents;
using PubSubEvents.DatabaseEvents;
using PW.Extensions;
using PW.FailFast;
using PW.IO.FileSystemObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using static PW.IO.DirectoryExtensions;

namespace ImageDeduper
{
  internal class EventListProcessorThread
  {

    public EventListProcessorThread(
      FileSystemEventBuffer<DirectoryPath> DirectoryEventBuffer,
      FileSystemEventBuffer<FilePath> FileEventBuffer,
      IEventAggregator ea
      )
    {
      this.DirectoryEventBuffer = DirectoryEventBuffer;
      this.FileEventBuffer = FileEventBuffer;
      EA = ea;

      ImageAddedEvent = ea.GetEvent<PubSubEvents.DatabaseEvents.ImageAddedEvent>();
      ImageRenamedEvent = ea.GetEvent<PubSubEvents.DatabaseEvents.ImageRenamedEvent>();
      //FilesDeletedEvent = ea.GetEvent<FilesDeletedEvent>();
      ImageDeletedEvent = ea.GetEvent<ImageDeletedEvent>();
      ImageUpdatedEvent = ea.GetEvent<PubSubEvents.DatabaseEvents.ImageUpdatedEvent>();
      StatusInfoEvent = ea.GetEvent<StatusInfoEvent>();
      ExceptionEvent = ea.GetEvent<ExceptionEvent>();

      Thread = new Thread(new ParameterizedThreadStart(ThreadProc)) { IsBackground = true, };

    }

    /// <summary>
    /// Token for cancelling the thread which processes the file and directory event buffers.
    /// </summary>
    private CancellationTokenSource? ThreadCancellationToken { get; set; }


    private bool _enabled;
    private readonly object _enabledLock = new();
    public bool Enabled
    {
      get => _enabled;
      set
      {
        lock (_enabledLock)
        {
          if (!PW.BackingField.AssignIfNotEqual(value, ref _enabled)) return;

          if (value == true && Thread.IsAlive) return;
          if (value == false && !Thread.IsAlive) return;

          if (value == true) // Enabling thread
          {
            if (!Thread.IsAlive)
            {
              ThreadCancellationToken?.Dispose();
              ThreadCancellationToken = new CancellationTokenSource();
              Thread.Start(ThreadCancellationToken.Token);
            }
          }
          else // Disabling thread
          {
            ThreadCancellationToken?.Cancel();
            // Not disposing here as it is so close to the Cancel()
            // and I have read there could be issues if the Thread is still running.
            // ThreadCancellationToken.Dispose();       
          }
        }
      }
    }


    private Thread Thread { get; set; }


    private FileSystemEventBuffer<DirectoryPath> DirectoryEventBuffer { get; }
    private FileSystemEventBuffer<FilePath> FileEventBuffer { get; }
    private IEventAggregator EA { get; }

    #region PubSub Events
    //EA.GetEvent<PubSubEvents.DatabaseEvents.ImageAddedEvent>().Publish(entity);
    private ImageAddedEvent ImageAddedEvent { get; }
    //private ImageDeletedEvent ImageDeletedEvent { get; }
    private ImageRenamedEvent ImageRenamedEvent { get; }
    
    //private FilesDeletedEvent FilesDeletedEvent { get; }

    public ImageDeletedEvent ImageDeletedEvent { get; }

    private ImageUpdatedEvent ImageUpdatedEvent { get; }
    private StatusInfoEvent StatusInfoEvent { get; }

    private ExceptionEvent ExceptionEvent { get; }

    #endregion

    private async void ThreadProc(object? obj)
    {
      var token = obj is not null ? (CancellationToken)obj : CancellationToken.None;

      if (token.IsCancellationRequested) return;
      Thread.Sleep(1000);

      while (!token.IsCancellationRequested)
      {
        try
        {

          #region Process directory event buffer

          // DEBUG: Test code to remove all images for deleted directories.
          // TODO: Look at adding logic to combine directory events, as with file events

          var t = DirectoryEventBuffer.TakeAll();
          if (t.Count != 0)
          {
            var directoryEvents = FileSystemEventCollection<DirectoryPath>.Combine(t);// <DirectoryPath>(t);
            using var db = new DatabaseUpdater(EA);

            var toDelete = directoryEvents.Deleted.Select(x => x.Path);
            foreach (var directory in toDelete)
            {
              if (token.IsCancellationRequested) return;
              var count = db.DeleteAllImages(directory);
              StatusInfoEvent.Publish($"Directory deleted: {directory.Value} - {count} images deleted.");
            }

            var toRename = directoryEvents.Renamed.ToArray();
            foreach (var x in toRename)
            {
              Assert.IsNotNull(x.OldPath);
              // CS8604: ValidatedNotNull being ignored on Assert and Guard classes. Plings to the rescue...
              // Ignore rename events where it is just the cASING that has been changed :)
              if (x.OldPath! != x.Path) db.RenameDirectory(x.OldPath!, x.Path);
            }

            var toCreate = directoryEvents.Created.Select(x => x.Path).ToArray();
            foreach (var directory in toCreate)
            {
              if (token.IsCancellationRequested) return;
              /*  
               *  Notes:
               *  Only get images from top-level, as directory-created event is fired individually for each sub-directory.
               *  Images added here are duplicated by events triggered by the file-event-watcher, however I do not trust
               *  that it always triggers for new directories. Duplicate image-created events will be combined later anyway.
               *  UPDATE: Found that moving a directory within the watched-folders only triggers an directory-created event
               *  for the top-level directory, not sub-directories. Reverting back to using SearchOption.AllDirectories.
               *  
               */
              directory.EnumerateGdiSupportedImages(SearchOption.AllDirectories)
                .ForEach(FileEventBuffer.AddCreated);
            }

          }
          #endregion


          #region Process file event buffer


          var events = FileSystemEventCollection<FilePath>.Combine(FileEventBuffer.TakeAll());

          if (events.Count != 0)
          {
            if (token.IsCancellationRequested) return;

            using var db = new DatabaseUpdater(EA);

            if (token.IsCancellationRequested) return;

            // DEBUG: Test code for delete images
            var toDelete = events.Deleted.Select(x => x.Path).ToArray();

            if (toDelete.Length != 0)
            {
              toDelete.ForEach(db.DeleteImage);
              //FilesDeletedEvent.Publish(toDelete);
              toDelete.ForEach(ImageDeletedEvent.Publish);
            }

            if (token.IsCancellationRequested) return;

            // DEBUG: Test code for rename images
            // ASSERT: OldPath will never be null for 'Renamed' events.            
            var toRename = events.Renamed.Select(x
              => x.OldPath is not null
              ? new FileRenamePair(x.OldPath, x.Path)
              : throw new InvalidOperationException("OldPath is null. This should never happen for 'Renamed' events."));

            foreach (var paths in toRename)
            {
              db.ChangePath(paths);
              ImageRenamedEvent.Publish(paths);
            }

            if (token.IsCancellationRequested) return;

            var upsertFailures = new List<FileSystemEvent<FilePath>>();

            // DEBUG: Test code for Upsert
            var toUpsert = events.CreatedOrChanged;

            foreach (var x in toUpsert)
            {
              if (token.IsCancellationRequested) return;
              var (entity, isNew) = await db.UpsertImage(x.Path, x.OldPath);

              if (entity is not null)
              {
                if (isNew) ImageAddedEvent.Publish(new ImageAddedEventArgs(entity.Id, (FilePath)entity.Path));
                else ImageUpdatedEvent.Publish((FilePath)entity.Path);
              }
              else upsertFailures.Add(x);
            }

            if (token.IsCancellationRequested) return;

            foreach (var fileEvent in upsertFailures)
            {
              FileEventBuffer.RequeueFailedEvent(fileEvent);
              StatusInfoEvent.Publish("Re-queueing file: " + fileEvent.Path.ToString());
            }

            if (token.IsCancellationRequested) return;
          }

          #endregion


          if (!token.IsCancellationRequested) Thread.Sleep(5000);
        }
        catch (Exception ex)
        {
          ExceptionEvent.Publish(ex);
          DebugHelper.WriteLineThreadId(ex.ToString());
        }
      }
    }

  }
}
