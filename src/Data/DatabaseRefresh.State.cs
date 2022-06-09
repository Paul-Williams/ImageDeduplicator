# nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PubSubEvents;
using PW.Extensions;
using PW.IO;
using PW.IO.FileSystemObjects;

namespace Data
{
  public partial class DatabaseRefresh
  {
    private class State
    {
      private const string MustInit = nameof(InitializeCache) + " must be called first.";

      private HashSet<DirectoryPath>? _directories;
      private Dictionary<string, (int Id, DateTime LastWriteTime)>? _filePathToEntityMapping;

      public StatusInfoEvent StatusEvent { get; } = Library.GetEvent<StatusInfoEvent>();

      public PubSubEvents.DatabaseEvents.ImageAddedEvent ImageAddedEvent { get; } = Library.GetEvent<PubSubEvents.DatabaseEvents.ImageAddedEvent>();

      // Directories already in the database. 
      // These are not currently stored as directories in the database and
      // have to be obtained by extracting the value from each image file path and then
      // taking the distinct values -- Slow?
      public HashSet<DirectoryPath> Directories
      {
        get => _directories ?? throw new InvalidOperationException(MustInit);
        private set => _directories = value ?? throw new InvalidOperationException(nameof(_directories) + " should not be explicitly set to null.");
      }

      // Entity lookup cache [Key:=Path]
      public Dictionary<string, (int Id, DateTime LastWriteTime)> FilePathToEntityMapping
      {
        get => _filePathToEntityMapping ?? throw new InvalidOperationException(MustInit);
        private set => _filePathToEntityMapping = value ?? throw new InvalidOperationException(nameof(_filePathToEntityMapping) + " should not be explicitly set to null.");
      }

      /// <summary>
      /// Determines whether the file on disc has changed since it was added to the database.
      /// Because of FileTime <-> SQL-Time mismatching, a margin of 1 minute is given when calculating this.
      /// </summary>
      public bool HasChanged(FileInfo file)
      {
        if (FilePathToEntityMapping is null) throw new InvalidOperationException(MustInit);
        return FilePathToEntityMapping[file.FullName].LastWriteTime.Difference(file.LastWriteTime) > TimeSpan.FromMinutes(1); //DataContextHelper.OneMinute;
      }



      public async Task InitializeCache(DirectoryPath root)
      {
        await Task.Run(() =>
        {
          try
          {
            using var dc = new DataContext();

            StatusEvent.Publish("Creating file path to entity mapping");

            // HARDCODE: Path
            // TODO: Add 'LibraryPath' to database.
            // Don't filter images for library root, as it will be all of them anyway!
            var images = Paths.DirectoryPathsMatch(root.Value, @"P:\Porn\")
              ? dc.Images
              : dc.Images.Where(x => x.Path.StartsWith(root.Value));

            FilePathToEntityMapping = images
              .Select(x => new { x.Path, x.Id, x.LastWriteTime })
              .ToDictionary(x => x.Path, x => (x.Id, x.LastWriteTime), StringComparer.OrdinalIgnoreCase);

            StatusEvent.Publish("Creating existing directory list.");

            Directories = FilePathToEntityMapping.Keys
              //This filter is not required here. It is done when creating the FilePathToEntityMapping dictionary.
              //.Where(x => x.StartsWith(start.Value, StringComparison.OrdinalIgnoreCase)) 
              .Select(p => Path.GetDirectoryName(p))
              .Distinct(StringComparer.OrdinalIgnoreCase)
              .Select(p => new DirectoryPath(p!))
              .ToHashSet();
          }
          catch (Exception ex)
          {
            // TODO: Do something more appropriate with the exception.
            System.Diagnostics.Trace.WriteLine(ex.ToString());
          }


        }).ConfigureAwait(false);
      }

      public int Created;
      public int Updated;
      public int Deleted;

    }

  }

}
