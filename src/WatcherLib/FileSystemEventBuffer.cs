#nullable enable

using PW.IO.FileSystemObjects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ImageDeduper
{
  /// <summary>
  /// Buffers events from FileSystemWatcher for subsequent processing.
  /// </summary>
  internal class FileSystemEventBuffer<T> where T : class, IFileSystemPath
  {

    public FileSystemEventBuffer()
    {
      Events = new BlockingCollection<FileSystemEvent<T>>();
    }

    private BlockingCollection<FileSystemEvent<T>> Events { get; }

    public int MaxRetries { get; set; } = 3;

    public List<FileSystemEvent<T>> TakeAll() => Events.TakeAll();

    /// <summary>
    /// Used to retry events when the file was read-locked etc. If the retry count is exceeded, then the event is discarded.
    /// </summary>
    public void RequeueFailedEvent(FileSystemEvent<T> failedEvent!!)
    {
      failedEvent.RetryCount += 1;

      if (failedEvent.RetryCount <= MaxRetries) Events.Add(failedEvent);

    }

    #region Add event to buffer methods

    public void AddCreated(T path)
    {
      //DebugHelper.WriteLineThreadId((string)path);
      Events.Add(FileSystemEvent<T>.Created(path));
    }

    public void AddChanged(T path)
    {
      //DebugHelper.WriteLineThreadId((string)path);
      Events.Add(FileSystemEvent<T>.Changed(path));
    }

    public void AddDeleted(T path)
    {
      //DebugHelper.WriteLineThreadId((string)path);
      Events.Add(FileSystemEvent<T>.Deleted(path));
    }

    public void AddRenamed(T path, T oldPath)
    {
      //DebugHelper.WriteLineThreadId(path + " : " + oldPath);
      Events.Add(FileSystemEvent<T>.Renamed(path, oldPath));
    }
    #endregion




  }
}
