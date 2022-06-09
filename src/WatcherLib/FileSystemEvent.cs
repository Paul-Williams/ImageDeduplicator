#nullable enable

using PW.Extensions;
using PW.IO.FileSystemObjects;
using System;
using System.Linq;

// WIP: To be used with replacement for WatcherHandler.

// Why is this generic?

namespace ImageDeduper
{
  /// <summary>
  /// Represents a <see cref="System.IO.FileSystemWatcher"/> event for a file.
  /// </summary>
  internal class FileSystemEvent<T> where T : class, IFileSystemPath
  {
    private FileSystemEvent(FileSystemEventType eventType, T file!!, T? oldFile, long timeStamp, int retryCount)
    {
      EventType = eventType;
      Path = file;
      OldPath = oldFile;
      Timestamp = timeStamp;
      RetryCount = retryCount;
    }

    private FileSystemEvent(FileSystemEventType eventType, T file!!, T oldFile!!)
    {
      EventType = eventType;
      Path = file;
      OldPath = oldFile;
      Timestamp = DateTime.Now.Ticks;
      RetryCount = 0;
    }

    private FileSystemEvent(FileSystemEventType eventType, T file!!)
    {
      if (eventType == FileSystemEventType.Renamed)
        throw new InvalidOperationException("Renamed events requires both a new and old path. Use the constructor which allows two file paths.");

      EventType = eventType;
      Path = file;
      OldPath = null;
      Timestamp = DateTime.Now.Ticks;
      RetryCount = 0;
    }

    #region Factory methods
    /// <summary>
    /// Creates an instance for a Renamed event.
    /// </summary>
    public static FileSystemEvent<T> Renamed(T file, T oldFile) => new(FileSystemEventType.Renamed, file, oldFile);

    /// <summary>
    /// Creates an instance for a Changed event.
    /// </summary>
    public static FileSystemEvent<T> Changed(T file) => new(FileSystemEventType.Changed, file);

    /// <summary>
    /// Creates an instance for a Created event.
    /// </summary>
    public static FileSystemEvent<T> Created(T file) => new(FileSystemEventType.Created, file);

    /// <summary>
    /// Creates an instance for a Deleted event.
    /// </summary>
    public static FileSystemEvent<T> Deleted(T file) => new(FileSystemEventType.Deleted, file);




    #endregion


    /// <summary>
    /// Used to sort events so that they can be processed in the order they were created.
    /// </summary>
    public long Timestamp { get; }

    /// <summary>
    /// The type of the file event.
    /// </summary>
    public FileSystemEventType EventType { get; }

    /// <summary>
    /// The current path to the file for which the event occurred.
    /// </summary>
    public T Path { get; }

    /// <summary>
    /// The original path of the file before a rename event, or before a series of events including a rename event.
    /// This will be null for most instances.
    /// </summary>
    public T? OldPath { get; }

    /// <summary>
    /// The number of times that processing of the file for this event has failed. 
    /// This normally happens because of a file-read-lock when a file is still downloading.
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// Combines a sequence of events for the same directory.
    /// For example, often, when an image is edited and saved, multiple 'changed' events will fire.
    /// </summary>
    public static FileSystemEvent<DirectoryPath> Combine(FileSystemEvent<DirectoryPath>[] events!!)
    {
      if (events.Length == 0) throw new ArgumentException("Array contains no elements.", nameof(events));
      if (events.Length == 1) return events[0];
      if (events.Length == 2) return Combine(events[0], events[1]);

      var r = events[0];
      foreach (var n in events.Skip(1)) r = Combine(r, n);
      return r;

    }


    /// <summary>
    /// Combines a sequence of events for the same file.
    /// For example, often, when an image is edited and saved, multiple 'changed' events will fire.
    /// </summary>
    public static FileSystemEvent<FilePath> Combine(FileSystemEvent<FilePath>[] events!!)
    {
      if (events.Length == 0) throw new ArgumentException("Array contains no elements.", nameof(events));
      if (events.Length == 1) return events[0];
      if (events.Length == 2) return Combine(events[0], events[1]);

      var r = events[0];
      foreach (var n in events.Skip(1)) r = Combine(r, n);
      return r;

    }

    /// <summary>
    /// Returns an event based on combining two sequential directory events.
    /// </summary>
    /// <param name="e1">The first event.</param>
    /// <param name="e2">The subsequent event.</param>
    /// <returns></returns>
    private static FileSystemEvent<DirectoryPath> Combine(FileSystemEvent<DirectoryPath> e1, FileSystemEvent<DirectoryPath> e2)
    {
      // Quick hack -- Re-work later :)

      // Work-around for issue that renaming a directory in Explorer fires a deleted followed by a renamed event. 
      if (e1.EventType == FileSystemEventType.Deleted && e2.EventType == FileSystemEventType.Renamed)
        return e2;

      // Handle renamed twice in quick succession. 
      // Merge the two into a single rename, skipping the intermediate name.
      if (e1.EventType == FileSystemEventType.Renamed && e2.EventType == FileSystemEventType.Renamed)
        return new FileSystemEvent<DirectoryPath>(FileSystemEventType.Renamed, e1.Path, e2.Path, e1.Timestamp, 0);

      // Handle created and then quickly renamed.
      // Merge the two into single create, using the name to which it was renamed.
      if (e1.EventType == FileSystemEventType.Created && e2.EventType == FileSystemEventType.Renamed)
        return new FileSystemEvent<DirectoryPath>(FileSystemEventType.Created, e1.Path, e2.Path, e1.Timestamp, 0);

      throw new Exception($"Unexpected combination of directory events: {e1.EventType} + {e2.EventType}");

    }

    /// <summary>
    /// Returns an event based on combining two sequential events.
    /// </summary>
    /// <param name="e1">The first event.</param>
    /// <param name="e2">The subsequent event.</param>
    /// <returns></returns>
    private static FileSystemEvent<FilePath> Combine(FileSystemEvent<FilePath> e1!!, FileSystemEvent<FilePath> e2!!)
    {
      var i = e1.EventType;
      var n = e2.EventType;

      // // Original code -- Logic appears invalid -- new version below
      ////  Created   Created   INVALID 
      //if (i == FileSystemEventType.Created && n == FileSystemEventType.Created) throw new InvalidOperationException();

      //  Created   Created   Created 
      if (i == FileSystemEventType.Created && n == FileSystemEventType.Created) return e1;

      //  Created   Deleted   Deleted
      if (i == FileSystemEventType.Created && n == FileSystemEventType.Deleted) return e2;

      //  Created   Changed   Created
      if (i == FileSystemEventType.Created && n == FileSystemEventType.Changed) return e1;

      //  Created   Renamed   Created
      if (i == FileSystemEventType.Created && n == FileSystemEventType.Renamed)
        return new FileSystemEvent<FilePath>(FileSystemEventType.Created, e2.Path, null, e1.Timestamp, 0);


      //  Deleted   Created   Changed
      if (i == FileSystemEventType.Deleted && n == FileSystemEventType.Created)
        return new FileSystemEvent<FilePath>(FileSystemEventType.Changed, e1.Path, null, e1.Timestamp, 0);

      //  Deleted   Deleted   INVALID
      if (i == FileSystemEventType.Deleted && n == FileSystemEventType.Deleted) throw new InvalidOperationException("Deleted   Deleted   INVALID");

      //  Deleted   Changed   INVALID (This has been changed to resolve to 'Created' as MS Edge fires this order when a download finished.)
      if (i == FileSystemEventType.Deleted && n == FileSystemEventType.Changed)
        return new FileSystemEvent<FilePath>(FileSystemEventType.Created, e2.Path, null, e1.Timestamp, 0);
      //throw new InvalidOperationException("Deleted   Changed   INVALID");

      //  Deleted   Renamed   INVALID
      if (i == FileSystemEventType.Deleted && n == FileSystemEventType.Renamed) throw new InvalidOperationException("Deleted   Renamed   INVALID");

      //  Changed   Created   INVALID
      if (i == FileSystemEventType.Changed && n == FileSystemEventType.Created) throw new InvalidOperationException("Changed   Created   INVALID");

      //  Changed   Deleted   Deleted
      if (i == FileSystemEventType.Changed && n == FileSystemEventType.Deleted)
        return new FileSystemEvent<FilePath>(FileSystemEventType.Deleted, e1.OldPath is null ? e1.Path : e1.OldPath, null, e1.Timestamp, 0);

      //  Changed   Changed   Changed
      if (i == FileSystemEventType.Changed && n == FileSystemEventType.Changed) return e1;

      //  Changed   Renamed   Changed
      if (i == FileSystemEventType.Changed && n == FileSystemEventType.Renamed)
        return new FileSystemEvent<FilePath>(FileSystemEventType.Changed, e2.Path, e2.OldPath, e1.Timestamp, 0);

      //  Renamed   Created   INVALID 
      if (i == FileSystemEventType.Renamed && n == FileSystemEventType.Created) throw new InvalidOperationException("Renamed   Created   INVALID");

      //  Renamed   Deleted   Deleted
      if (i == FileSystemEventType.Renamed && n == FileSystemEventType.Deleted)
        return new FileSystemEvent<FilePath>(FileSystemEventType.Deleted, e2.OldPath!, null, e1.Timestamp, 0);

      //  Renamed   Changed   Changed
      if (i == FileSystemEventType.Renamed && n == FileSystemEventType.Deleted)
        return new FileSystemEvent<FilePath>(FileSystemEventType.Changed, e2.Path, e2.OldPath, e1.Timestamp, 0);

      //  Renamed   Renamed   Renamed
      if (i == FileSystemEventType.Renamed && n == FileSystemEventType.Renamed)
        return new FileSystemEvent<FilePath>(FileSystemEventType.Renamed, e2.Path, e1.OldPath, e1.Timestamp, 0);

      // If we got here then there is a combination of events which was not covered above.
      // This should not be the case, so treat it as an error.
      throw new InvalidOperationException($"No rule for: {i} + {n}");

    }



  }

}
