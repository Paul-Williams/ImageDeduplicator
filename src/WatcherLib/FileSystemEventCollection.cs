#nullable enable

using PW.Extensions;
using PW.IO.FileSystemObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ImageDeduper
{
  internal class FileSystemEventCollection<T> : IEnumerable<FileSystemEvent<T>> where T : class, IFileSystemPath
  {

    private FileSystemEventCollection()
    {
      Events = new List<FileSystemEvent<T>>();
    }

    public FileSystemEventCollection(IEnumerable<FileSystemEvent<T>> events)
    {
      Events = new List<FileSystemEvent<T>>(events);
    }

    private List<FileSystemEvent<T>> Events { get; }

    public int Count => Events.Count;

    public IEnumerator<FileSystemEvent<T>> GetEnumerator() => Events.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Events.GetEnumerator();

    /// <summary>
    /// Returns just those events where <see cref="FileSystemEvent.EventType"/> equals <see cref="FileSystemEventType.Renamed"/>.
    /// </summary>
    public IEnumerable<FileSystemEvent<T>> Renamed => Events.Where(x => x.EventType == FileSystemEventType.Renamed);

    /// <summary>
    /// Returns just those events where <see cref="FileSystemEvent.EventType"/> equals <see cref="FileSystemEventType.Deleted"/>.
    /// </summary>
    public IEnumerable<FileSystemEvent<T>> Deleted => Events.Where(x => x.EventType == FileSystemEventType.Deleted);

    public IEnumerable<FileSystemEvent<T>> CreatedOrChanged
      => Events.Where(x => x.EventType is FileSystemEventType.Created or FileSystemEventType.Changed);

    public IEnumerable<FileSystemEvent<T>> Created => Events.Where(x => x.EventType == FileSystemEventType.Created);

    public IEnumerable<FileSystemEvent<T>> Changed => Events.Where(x => x.EventType == FileSystemEventType.Changed);


    public static FileSystemEventCollection<DirectoryPath> Combine(ICollection<FileSystemEvent<DirectoryPath>> events!!)
    {
      if (events.Count == 0) return new FileSystemEventCollection<DirectoryPath>();
      if (events.Count == 1) return new FileSystemEventCollection<DirectoryPath>(events);

      //HACK: See notes in method: Combine(IEnumerable<FileSystemEvent<FilePath>> events)

      // Explorer triggers delete followed by rename event, when there should just be a renamed event.
      // Find if any delete events are due to that issue and remove them.
      var removeThese = (
        from left in events
        join right in events on left.Path.Value equals right.OldPath?.Value
        where left.EventType == FileSystemEventType.Deleted
        where right.EventType == FileSystemEventType.Renamed
        select left)
        .ToArray();


      if (removeThese.Length != 0) removeThese.ForEach(x => events.Remove(x));

      // TODO: Add code here to solve issue #1

      if (events.Count == 1) return new FileSystemEventCollection<DirectoryPath>(events);

      var r = new FileSystemEventCollection<DirectoryPath>();


      var eventsForSingleDirectory = events.GroupBy(x => (string)x.Path).ToArray();

      foreach (var group in eventsForSingleDirectory)
      {
        var combinedEvent = FileSystemEvent<DirectoryPath>.Combine(group.OrderBy(x => x.Timestamp).ToArray());
        r.Events.Add(combinedEvent);
      }

      return r;
    }

    /// <summary>
    /// Creates a new collection from the enumeration. Multiple events for the same file are combined to a single event.
    /// </summary>
    public static FileSystemEventCollection<FilePath> Combine(IEnumerable<FileSystemEvent<FilePath>> events)
    {
      var r = new FileSystemEventCollection<FilePath>();

      // BUG: 
      // There is a logic problem with this method of grouping.
      // It is supposed to group all events for a file. However, for Renamed events
      // the grouping should look at OldFile. 
      // If multiple renamed events occurred then this is further complicated.

      // Eg. Take this sequence of events
      // Created(a)
      // Changed(a)
      // Renamed(b,a)
      // Changed(b)
      // Renamed(c,b)

      // Although they are actually all for the same original file (a), only
      // the first two events will be grouped as (a).
      // The next two will appear in a second group (b) and the last will be in a group on its own.
      // Ideally all the above should be combined to a single event 'Created(c)'

      // I think that to properly combine the events it would be necessary to walk through the sequence
      // and use logic based on the type of the event and the values of either the File or OldFile.

      // The likelihood of this is low if the events are processed soon after they are generated.
      // Furthermore, as long as they are persisted to the database in the correct order 
      // then the database will still be in sync with the file system. It will just mean that additional 
      // db updates will be performed. Given this example the db will be updated three times, instead of once.


      // Possible new logic:
      // Get all Created events
      // If there are multiple Created events for the same file, then keep only the latest.

      // If there are any Deleted events for a Created event file that are more recent than the Created event
      //    then drop the created event
      // If there are any created events, then drop any Changed events for the same file
      // If there 

      var eventsForSingleFile = events.GroupBy(x => (string)x.Path).ToArray();

      foreach (var group in eventsForSingleFile)
      {
        var combinedEvent = FileSystemEvent<T>.Combine(group.OrderBy(x => x.Timestamp).ToArray());
        r.Events.Add(combinedEvent);
      }

      return r;
    }



  }
}
