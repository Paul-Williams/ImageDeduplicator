// Working, but complicated and buggy.

//# nullable enable

//using Data.Models;
//using Prism.Events;
//using PubSubEvents;
//using PubSubEvents.DatabaseEvents;
//using PW;
//using PW.Extensions;
//using PW.FailFast;
//using System;
//using System.Data.Entity;
//using System.IO;
//using System.Linq;
//using System.Threading;
//using XnaFan.ImageComparison;

///* TODO:
// * Implement events for delete
// * Watch for file change events
// */

//namespace ImageDeduper
//{
//  public sealed class WatcherHandler : IDisposable
//  {
//    /// <summary>
//    /// Constructor
//    /// </summary>
//    public WatcherHandler(IEventAggregator ea, DirectoryInfo watchThis)
//    {
//      Guard.NotNull(ea, nameof(ea));
//      Guard.NotNull(watchThis, nameof(watchThis));
//      Guard.True(watchThis.Exists, nameof(watchThis), "Watch directory must exist.");

//      ExceptionEvent = ea.GetEvent<ExceptionEvent>();
//      StatusInfoEvent = ea.GetEvent<StatusInfoEvent>();
//      ImageDeletedEvent = ea.GetEvent<ImageDeletedEvent>();
//      ImageRenamedEvent = ea.GetEvent<ImageRenamedEvent>();
//      ImageAddedEvent = ea.GetEvent<ImageAddedEvent>();
//      ImageUpdatedEvent = ea.GetEvent<ImageUpdatedEvent>();
//      DuplicateImageAddedEvent = ea.GetEvent<DuplicateImageAddedEvent>();

//      new Thread(ThreadProc).Start(watchThis.FullName);
//    }

//    private CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

//    private Timer? LazyResetTimer { get; set; }

//    private ResettableLazy<DataContexts> ResettableLazyOfDataContexts { get; } = new ResettableLazy<DataContexts>(() => new DataContexts());

//    private DataContexts DataContexts => ResettableLazyOfDataContexts.Value;

//    // Must be nullable, as is not instantiated in the constructor.
//    private FileEventDelayingFilter? DelayedEvents { get; set; }

//    #region PubSub Event References

//    private ExceptionEvent ExceptionEvent { get; }

//    private StatusInfoEvent StatusInfoEvent { get; }

//    private ImageDeletedEvent ImageDeletedEvent { get; }

//    private ImageRenamedEvent ImageRenamedEvent { get; }

//    private ImageAddedEvent ImageAddedEvent { get; }

//    private ImageUpdatedEvent ImageUpdatedEvent { get; }

//    private DuplicateImageAddedEvent DuplicateImageAddedEvent { get; }

//    #endregion PubSub Event References

//    //CHANGE::[26/07/19]:: Was 5 - changed to 60 in attempt to fix System.ObjectDisposedException
//    private static TimeSpan timerDueTime = TimeSpan.FromSeconds(60);

//    private static Timer CreateTimer(TimerCallback callback) => new Timer(callback, null, (int)timerDueTime.TotalMilliseconds, Timeout.Infinite);

//    private static void ResetTimer(Timer timer) => timer.Change((int)timerDueTime.TotalMilliseconds, Timeout.Infinite);

//    private void LazyResetTimer_Callback(object data)
//    {
//      try
//      {
//        ResettableLazyOfDataContexts.Reset();
//      }
//      catch (Exception ex)
//      {
//        ExceptionEvent.Publish(ex);
//      }
//    }

//    private FileSystemWatcher CreateFileSystemWatcher(string path)
//    {
//      var watcher = new FileSystemWatcher(path) { IncludeSubdirectories = true };
//      watcher.Created += Watcher_FileEvent;
//      watcher.Deleted += Watcher_FileEvent;
//      watcher.Changed += Watcher_FileEvent;
//      watcher.Renamed += Watcher_FileEvent; //NB: HACK::This should really be a RenamedEventHandler
//      watcher.Error += Watcher_Error;
//      return watcher;
//    }

//    private void ThreadProc(object watchDirectoryString)
//    {
//      var originalSyncContext = SynchronizationContext.Current;
//      //TraceHelper.WriteLineThreadId("WatcherHandler.ctor.NewThread");
//      try
//      {
//        // Context provides a way for the various watcher-related classes to synchronize events and call-backs to this thread.
//        // This prevents multi-threading issues with the encapsulated EF DbContext.
//        var synchronizationContext = new PW.Threading.BlockingSynchronizationContext();

//        // Replace the current (null?) context of this thread with our custom one.
//        SynchronizationContext.SetSynchronizationContext(synchronizationContext);

//        var watcher = CreateFileSystemWatcher((string)watchDirectoryString);

//        DelayedEvents = new FileEventDelayingFilter(synchronizationContext);
//        DelayedEvents.NewEvent += DelayedEvents_Handler;

//        // The magic starts here ;)
//        watcher.EnableRaisingEvents = true;

//        // Execution blocks here, until Dispose() is called on this object, by the client.
//        synchronizationContext.WaitForCompletion(CancellationTokenSource.Token);

//        // Token has now been canceled by calling Dispose(), so Tear-down and clean-up.
//        watcher.EnableRaisingEvents = false;
//        watcher.Dispose();
//        synchronizationContext.Dispose();

//        // TODO: ISSUE 33: Dispose 'ResettableLazy<ImagesBatchedOperations> lazy' here in order to flush everything to the database.
//        if (ResettableLazyOfDataContexts.IsValueCreated) ResettableLazyOfDataContexts.Value.Dispose();
//      }

//      // The original context should be null, but we will restore it, to follow 'best practice'.
//      finally { SynchronizationContext.SetSynchronizationContext(originalSyncContext); };
//    }

//    // Handles error events for the Watcher instance. Sends a status event and shuts down the thread.
//    private void Watcher_Error(object sender, ErrorEventArgs e)
//    {
//      StatusInfoEvent.Publish($"File system watcher encountered an error and cannot continue. {e.GetException()?.Message ?? "<Exception Missing>" }");
//      CancellationTokenSource.Cancel();
//    }

//    // Handles 'file events' (i.e. not the error event) for the Watcher instance.
//    public void Watcher_FileEvent(object sender, FileSystemEventArgs e)
//    {
//      Guard.NotNull(e, nameof(e));

//      var file =  new FileInfo(e.FullPath);

//      // Prevent recycle bin files from being processed.
//      if (e.FullPath.Contains("$RECYCLE.BIN", StringComparison.OrdinalIgnoreCase)) return;

//      // Only support events for image files
//      if (!PW.Drawing.Imaging.GdiImageDecoderFormats.IsSupported(file.Extension)) return;

//      // WIP: Check file is not in the directory-ignore-list
//      if (Data.IgnoredDirectories.IsIgnored(file)) return;

//      // All ok, so add it to the delay-cache-thingy and wait for it to get back to us, or not ;)
//      DelayedEvents?.Add(e);
//    }

//    // Resets the timer if it already exists, otherwise creates one.
//    private void CreateOrResetTimer()
//    {
//      if (LazyResetTimer is null) LazyResetTimer = CreateTimer(LazyResetTimer_Callback);
//      else ResetTimer(LazyResetTimer);
//    }

//    // Handles events from the WatcherEventDelayingFilter
//    private void DelayedEvents_Handler(object sender, FileSystemEventArgs e)
//    {
//      // Getting an exception from somewhere below here which is blowing the app...
//      try
//      {
//        CreateOrResetTimer();

//        switch (e.ChangeType)
//        {
//          case WatcherChangeTypes.Created:
//          case WatcherChangeTypes.Changed:
//            Handle_WatcherChangeTypes_CreatedOrChanged(new FileInfo(e.FullPath));
//            break;

//          case WatcherChangeTypes.Deleted:
//            Handle_WatcherChangeTypes_Deleted(e);
//            break;

//          case WatcherChangeTypes.Renamed:
//            Handle_WatcherChangeTypes_Renamed(e);
//            break;

//          // This may get hit if the value 'WatcherChangeTypes.All' somehow makes it here, or some invalid int is passed in.
//          default: throw new InvalidOperationException($"Unsupported {nameof(WatcherChangeTypes)} value {e.ChangeType}.");
//        }
//      }
//      catch (Exception ex)
//      {
//        ExceptionEvent.Publish(ex);
//      }

//    }

//    private void Handle_WatcherChangeTypes_Renamed(FileSystemEventArgs e)
//    {
//      // In order to have all events handled by single proc RenamedEventArgs have ended up as FileSystemEventArgs.
//      // We need to cast this back in order to get at the OldFile
//      var oldPath = ((RenamedEventArgs)e).OldFullPath;

//      // Attempt to retrieve the entity which corresponds to the renamed file's path -- Entity may not exist.
//      if (DataContexts.Context.ImageByPath(oldPath) is ImageEntity entity)
//      {
//        entity.Path = e.FullPath;
//        DataContexts.Batch.Update(entity);
//        ImageRenamedEvent.Publish(entity);
//      }

//      // If the entity does not exist, then take this opportunity to add it to the database.
//      else
//      {
//        if (PW.Functional.Try.ValueOrDefault(new FileInfo(e.FullPath).ToImageEntity) is ImageEntity newEntity)
//        {
//          DataContexts.Batch.Insert(newEntity);
//          ImageAddedEvent.Publish(newEntity);
//        }
//        else // Failed to read image file.
//        {
//          PublishEventCantOpenImage(e.FullPath);
//        }
//      }
//    }

//    private void Handle_WatcherChangeTypes_Deleted(FileSystemEventArgs e)
//    {
//      // Attempt to find the id of the entity which corresponds to the deleted file's path -- Entity may not exist.
//      var id = DataContexts.SavedImages.Where(x => x.Path == e.FullPath).Select(x => x.Id).FirstOrDefault();
//      // If an id was found, then create an entity 'stub' and remove it from the database.
//      if (id != 0)
//      {
//        DataContexts.Batch.Delete(new ImageEntity(id));
//        ImageDeletedEvent.Publish(new ImageEntity() { Path = e.FullPath });
//      }
//    }

//    private void Handle_WatcherChangeTypes_CreatedOrChanged(FileInfo file)
//    {
//      // There is an unique index on path, so attempting to add another entity with the same
//      // path would cause an exception. An entity with the same path may exist if there is stale data
//      // in the database. We will consider a match on path as the same entity and update the existing one.
//      // If this file with a unique path, but a duplicate thumb-print, then it still needs to be added
//      // to the database as a new entity.

//      var imageEntity = PW.Functional.Try.ValueOrDefault(file.ToImageEntity);

//      if (imageEntity is null)
//      {
//        PublishEventCantOpenImage(file.FullName);
//        return;
//      }

//      // Attempt to retrieve an existing entity with either the same path or byte data (thumb-print).
//      if (FindEntityWithSamePathOrThumbPrint(imageEntity) is ImageEntity match)
//      {
//        // If there is a match, and the path is not the same, then the thumb-print must be the same.
//        // We will have an assertion for this, for now, to ensure that the logic is sound.
//        Assert.True((UpdateEntityIfPathsMatch(imageEntity, match)) || HandleEntityWithSameThumbPrint(imageEntity, DataContexts.SavedImages, match),
//          "Logic Error: matched entity must have either same path or thumb-print.");
//      }
//      else
//      {
//        // HACK:
//        // Can't, as yet, work out why exporting from GIMP causes a duplicate path index exception.
//        // So, just catch the exception and update the existing image...

//        // UPDATE: This is not working. The exception is not thrown here, rather when attempting to save changes.

//        try
//        {
//          // If there was not an entity which matched on either the path or thumb-print,
//          // then this file is new, so add it to the database.
//          DataContexts.Batch.Insert(imageEntity);
//          ImageAddedEvent.Publish(imageEntity); // NULL EXCEPTION HERE
//        }
//        catch (System.Data.SqlClient.SqlException ex)
//        {
//          // Can't remember the specific exception for the duplicate.
//          // For now, just display the error:
//          StatusInfoEvent.Publish(ex.ToString()); // TODO: Update this when exact error is determined.
//        }
//      }
//    }

//    // Publishes a status info event for an image file that cannot be opened.
//    private void PublishEventCantOpenImage(string imagePath) =>
//      StatusInfoEvent.Publish($"{nameof(WatcherHandler)} was unable to open image: '{imagePath}'");

//    // Returns the first entity with either the same path or thumb-print, or null if no match is found.
//    private ImageEntity FindEntityWithSamePathOrThumbPrint(ImageEntity ii)
//    {
//      // BUGFIX: 31 -- Not sure if this will cause knock-on problems...
//      // If there is a match in the local cache (yet to be written to the DB) then return it.
//      if (DataContexts.Batch.FirstOrDefault(x => x.Path == ii.Path || x.Bytes == ii.Bytes) is ImageEntity localMatch) return localMatch;

//      // Otherwise check the database for a match
//      // return DataContexts.SavedImages.Where(x => x.Path == ii.Path || x.Bytes == ii.Bytes).FirstOrDefault();

//      // 01/10/19 -- Use new Stored Procedure -- Slower, but performs fuzzy-match.
//      return DataContexts.Context.FuzzyMatchBytesFirstOrDefault(ii.Bytes);
//    }

//    /// <summary>
//    /// Returns true, when the paths match and the existing image entity is updated. Otherwise returns false.
//    /// </summary>
//    private bool UpdateEntityIfPathsMatch(ImageEntity newImage, ImageEntity existingImage)
//    {
//      if (existingImage.Path == newImage.Path)
//      {
//        // Handle case where a newly inserted image entity has been modified on disk before the initial
//        // insert is committed to the database (i.e. entity is still just in 'local' cache).
//        // In this case, modify the existing entity with the data from the new entity.
//        // ASSUMES: If the Id is zero, then the entity has not yet been committed.
//        if (existingImage.Id == 0)
//        {
//          existingImage.Bytes = newImage.Bytes;
//          existingImage.FileSize = newImage.FileSize;
//          existingImage.Height = newImage.Height;
//          existingImage.HorizontalResolution = newImage.HorizontalResolution;
//          existingImage.LastWriteTime = newImage.LastWriteTime;
//          existingImage.VerticalResolution = newImage.VerticalResolution;
//          existingImage.Width = newImage.Width;
//        }

//        // Existing image id is non-zero, so has already been committed to the database.
//        // Use its id for the new image and update the database to reflect the changes.
//        else
//        {
//          // Set the id of the new image to match that of the existing image
//          // and update the database with the new image.
//          newImage.Id = existingImage.Id;
//          DataContexts.Batch.Update(newImage);
//        }

//        // Display that the entity has been modified, regardless of whether it is local or previously saved.
//        ImageUpdatedEvent.Publish(newImage);
//        return true;
//      }
//      else return false;
//    }

//    private bool HandleEntityWithSameThumbPrint(ImageEntity newImage, DbSet<ImageEntity> dbSet, ImageEntity existingImage)
//    {
//      // This should have already been checked  in <see cref="UpdateEntityIfPathsMatch(ImageInfo, ImageEntity)"/>
//      // However, we will check it again to save any possible exception on entity insert.
//      Assert.False(newImage.Path == existingImage.Path,
//        $"Paths should not match. Matching path case should be handled, by method '{nameof(UpdateEntityIfPathsMatch)}()', before calling this method");

//      // This assertion is no longer valid. Fuzzy-match is used now, so ImageInfo.Bytes may differ between the two instances.
//      //// This method is designed to handle the case where a new file and an existing entity have a duplicate thumb-print.
//      //// DEBUG: For now we will assert this is the case.
//      //Assert.True(existingImage.Bytes.SequenceEquals(newImage.Bytes),
//      //  "Thumb-prints don't match. This method should only be called when it is known that the two objects have the same thumb-print.");

//      // Even though this file has a duplicate thumb-print it is still a new file, so add it to the database anyway.
//      var newEntity = dbSet.Add(newImage);
//      ImageAddedEvent.Publish(newEntity);

//      // If the database is stale, then the entity may refer to a file which no longer exists.
//      // Only raise the event if the entity-path is still valid.
//      if (File.Exists(existingImage.Path)) DuplicateImageAddedEvent.Publish((existingImage, newImage));

//      // If the other entity-path is no longer valid, then remove the obsolete record from the database.
//      // And raise the event for that action.
//      else
//      {
//        dbSet.Remove(existingImage);
//        ImageDeletedEvent.Publish(existingImage);
//      }

//      return true;
//    }

//    /// <summary>
//    /// Cancels the internal thread-wait and allows thread to exit, disposing of all resources.
//    /// </summary>
//    public void Dispose() => CancellationTokenSource.Cancel();
//  }
//}