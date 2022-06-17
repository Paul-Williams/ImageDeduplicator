# nullable enable

using Data;
using Data.Models;
using Prism.Events;
using PubSubEvents.DatabaseEvents;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

// NOTES:
// There is currently no way to stop the background thread once this class is instantiated. 
// This means that the thread will continue to run until the application exits.
// Internally thread cancellation is implemented, but this is not exposed publicly.
// I think the simplest way to expose this would be to make the class disposable and cancel the thread within the Dispose() method.


namespace ImageDeduper
{
  /// <summary>
  /// Automatically queues newly added ImageEntity instances for duplicate-detection. 
  /// Subscribes to ImageAddedEvent and adds ImageEntity paths to the queue.
  /// Uses a dedicated, blocking, thread to process the queue.
  /// Publishes DuplicateImageAddedEvent when a duplicate is detected.
  /// </summary>
  internal class BackgroundDuplicateImageDetector
  {
    public BackgroundDuplicateImageDetector(IEventAggregator ea)
    {
      DuplicateImageAddedEvent = ea.GetEvent<DuplicateImageAddedEvent>();
      ExceptionEvent = ea.GetEvent<PubSubEvents.ExceptionEvent>();
      StatusInfo = ea.GetEvent<PubSubEvents.StatusInfoEvent>();

      Queue = new BlockingCollection<int>();
      ea.GetEvent<ImageAddedEvent>().Subscribe(Enqueue);

      QueueThread = new Thread(new ParameterizedThreadStart(QueueThreadProc)) { IsBackground = true };

      ThreadCanceller = new CancellationTokenSource();
      QueueThread.Start(ThreadCanceller.Token);
    }

    #region PubSub Event stuff

    /// <summary>
    /// PubSub event raised upon detection of a duplicate image.
    /// </summary>
    private DuplicateImageAddedEvent DuplicateImageAddedEvent { get; }

    /// <summary>
    /// Event raised when an exception occurs.
    /// </summary>
    private PubSubEvents.ExceptionEvent ExceptionEvent { get; }

    private PubSubEvents.StatusInfoEvent StatusInfo { get; }

    #endregion

    private BlockingCollection<int> Queue { get; }

    private Thread QueueThread { get; }

    private CancellationTokenSource ThreadCanceller { get; }

    private void Enqueue(ImageAddedEventArgs ea)
    {
      Queue.Add(ea.EntityId);
    }

    private void QueueThreadProc(object? obj)
    {
      var token = obj is not null ? (CancellationToken)obj : CancellationToken.None; ;

      try
      {
        // NB: GetConsumingEnumerable() blocks until item is available
        foreach (var entityId in Queue.GetConsumingEnumerable(token))
        {
          // Construct the event message - If the queue has further items, then include this information in the message.
          static string GetMsg(int queueLength) => queueLength > 0
            ? $"Checking for duplicate. {queueLength} remaining in queue."
            : "Checking for duplicate.";

          StatusInfo.Publish(GetMsg(Queue.Count));
          var (duplicate, existing) = FindFirstDuplicate(entityId);

          if (duplicate is ImageEntity && existing is ImageEntity)
            DuplicateImageAddedEvent.Publish(new DuplicateImageAddedEventArgs(existing, duplicate));

        }
      }
      catch (Exception ex)
      {
        ExceptionEvent.Publish(ex);
      }
    }

    /// <summary>
    /// Attempts to get the file path's ImageEntity and the first duplicate ImageEntity for the specified file path.
    /// If the file path does not exist, or there is no duplicate ImageEntity, or the first duplicate ImageEntity's 
    /// file no longer exists then (null, null) is returned.
    /// If both the file path and first duplicate entity exist on disk, then (ImageEntity, ImageEntity) is returned.
    /// </summary>
    private static (ImageEntity? Duplicate, ImageEntity? Existing) FindFirstDuplicate(int entityId)
    {

      // Ensure the image still exists on disk -- It may have been deleted since being queued.
      // if (!FileSystem.Exists(filePath)) return (null, null);

      using var dc = new DataContext();

      // Retrieve the entity for the new image.
      var newImageEntity = dc.Images.Find(entityId);
      if (newImageEntity is null) return (null, null);

      // Ensure the new image still exists on disk.
      if (!File.Exists(newImageEntity.Path)) return (null, null);

      // Check database for duplicate
      // If the database is stale, then the returned entity may refer to a file which no longer exists.
      // Only raise the event if the entity-paths are both still valid.
      return dc.FuzzyMatchBytesFirstOrDefault(newImageEntity) is ImageEntity existing && File.Exists(existing.Path) && File.Exists(newImageEntity.Path)
        ? ((ImageEntity? Duplicate, ImageEntity? Existing))(newImageEntity, existing)
        : ((ImageEntity? Duplicate, ImageEntity? Existing))(null, null);

    }

    /// <summary>
    /// Removes all items from the queue.
    /// </summary>
    public void Clear()
    {
      // Take and discard any and all items from the queue.
      while (Queue.TryTake(out _)) { }
    }

  }
}
