# nullable enable

using Data.Models;
using PubSubEvents;
using PW.Data.EntityFramework;
using PW.Extensions;
using static PW.Extensions.ArrayExtensions;
using PW.FailFast;
using PW.Functional;
using PW.IO;
using PW.IO.FileSystemObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using XnaFan.ImageComparison;
using static PW.IO.DirectoryInfoExtensions;
using static PW.IO.DirectoryExtensions;
using static PW.Functional.FuncExtensions;
using PW.ImageDeduplicator.Common;

namespace Data
{
  public static partial class DatabaseRefresh
  {
    /// <summary>
    /// Delete all entities which no longer exist on disk.
    /// Returns the number of deleted entities.
    /// </summary>
    private static async Task DeleteEntitiesForMissingFiles(State state)
    {

      var statusEvent = Library.GetEvent<StatusInfoEvent>();

      statusEvent.Publish("Getting entity ids for deleted image files.");

      // Get Id's of entities where the file no longer exists on disk.
      var missing = await Task.Run(() =>
      {
        return state.FilePathToEntityMapping
       .Where(pair => !File.Exists(pair.Key))     // Key is the file path
       .ToArray();
      }).ConfigureAwait(false);

      statusEvent.Publish($"Deleting {missing.Length} orphaned entities");
      // If any were found, delete the from data database.
      var ids = missing.Select(pair => pair.Value.Id).ToArray();

      // This throwing 'System.Data.SqlClient.SqlException (0x80131904): The query processor ran out of internal resources'
      // when there were 43493 orphaned files.
      // Lets try saving in batches...
      // NB: The deletes are performed as SQL statements, rather than using 'context.Images.Remove'. Therefore the issue must be with the 
      // length of the generated SQL statement, rather than being directly the number of entities deleted.

      const int batchSize = 1000; // Arbitrary figure, picked from the ether.

      if (ids.Length != 0)
      {

        if (ids.Length < batchSize)
        {
          using var context = new DataContext();
          await context.ExecuteDeleteByIdAsync<ImageEntity, int>(ids).ConfigureAwait(false);

        }
        else
        {
          using var context = new DataContext();
          foreach (var batch in ids.Segment(batchSize))
          {
            await context.ExecuteDeleteByIdAsync<ImageEntity, int>(batch).ConfigureAwait(false);
          }
        }
      }

      state.Deleted += ids.Length;

      //// Original, working code. Threw when too many changes...
      //if (ids.Length != 0)
      //{
      //  using var context = new DataContext();
      //  await context.ExecuteDeleteByIdAsync<ImageEntity, int>(ids).ConfigureAwait(false);
      //  state.Deleted += ids.Length;
      //}

      // Now delete them from the cache, as they are invalid.
      missing.ForEach(pair => state.FilePathToEntityMapping.Remove(pair.Key));

    }


    private static readonly DataflowLinkOptions propagate = new() { PropagateCompletion = true };

    /// <summary>
    /// This is the 'do-it' method ;)
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    public static async Task<(int Created, int Updated, int Deleted)> PerformRefreshAsync(DirectoryPath root)
    {
      Guard.MustExist(root, nameof(root));

      var state = new State();

      await state.InitializeCache(root).ConfigureAwait(false);
      await DeleteEntitiesForMissingFiles(state).ConfigureAwait(false);

      // DEBUG
      // return (state.Created, state.Updated, state.Deleted);
      // DEBUG

      using var bi = new BatchedOperations<DataContext, ImageEntity>(() => new DataContext());

      // Create Blocks
      var directoryToFileOps = CreateBlock_DirectoryFileOps(state);
      var fileOpToEntityOp = CreateBlock_FileOpsToEntityOps(state);
      var saveEntityToDb = new ActionBlock<EntityOp>(x =>
      {
        if (x.Entity is null) throw new InvalidOperationException(nameof(x.Entity) + " should not null here.");

        try
        {
          if (x.Operation == DbOperation.Insert)
          {
            bi.Insert(x.Entity);
            Interlocked.Increment(ref state.Created);

            // This does not work properly here as entity may well still be in the batch and not saved to the database.
            // state.ImageAddedEvent.Publish(x.Entity);
          }
          else
          {
            bi.Update(x.Entity);
            Interlocked.Increment(ref state.Updated);
            //state.StatusEvent.Publish("Updating entity: " + x.Entity.Path);
          }
        }
        catch (Exception)// ex)
        {

          throw;
        }



      });

      // Link Blocks
      directoryToFileOps.LinkTo(fileOpToEntityOp, propagate);

      // Some pictures may be corrupt and cannot be opened. In this case a null entity will be returned.
      // This is of no use, so we will dump it to a null target block. We could do something better, but you know...
      fileOpToEntityOp.LinkTo(DataflowBlock.NullTarget<EntityOp>(), x => x.Entity is null);
      fileOpToEntityOp.LinkTo(saveEntityToDb, propagate);


      // Begin DataFlow
      // WIP: Ignore list implementation --------------------------------------v
      // It would be better for IO if there were an overload of EnumerateDirectories which took an ignore list.
      foreach (var directory in EnumerateDirectories(root).Where(x => !IgnoredDirectories.IsIgnored(x)))
        if (!directoryToFileOps.Post(directory)) state.StatusEvent.Publish($"{nameof(fileOpToEntityOp)} did not accept '{directory}'.");

      directoryToFileOps.Complete();

      await saveEntityToDb.Completion.ConfigureAwait(false);
      Trace.WriteLine("YAY");

      return (state.Created, state.Updated, state.Deleted);

    }

    private static IEnumerable<DirectoryPath> EnumerateDirectories(DirectoryPath start) =>
      // HACK -- Need version of EnumerateAuthorizedDirectories() which takes DirectoryPath
      start.ToDirectoryInfo().EnumerateAuthorizedDirectories(true).Where(x => !x.FullName.Contains("$RECYCLE.BIN")).Select(x => new DirectoryPath(x));

    /// <summary>
    /// Returns a block for transforming a file, which already exists in the database, into it's database entity.
    /// The DbOperation argument is used to either insert a new entity, or update an existing entity
    /// </summary>
    private static TransformBlock<FileOp, EntityOp> CreateBlock_FileOpsToEntityOps(State state)
    {
      return new TransformBlock<FileOp, EntityOp>
        (fileOp =>
        {
          try
          {
            if (ValueOrDefault(fileOp.File, x => x.ToFileInfo().ToImageEntity()) is ImageEntity imageInfo)
            {
              if (fileOp.Operation == DbOperation.Update)
              {
                imageInfo.Id = state.FilePathToEntityMapping[fileOp.File.Value].Id;
                return new EntityOp(imageInfo, fileOp.Operation);
              }
              else return new EntityOp(imageInfo, fileOp.Operation);
            }
            else
            {
              // HACK: WE don't really want to do this. 
              // When a picture cannot be opened we would rather simply abort the operation.
              state.StatusEvent.Publish("Failed to load image: " + fileOp.File.Value);
              return new EntityOp(null, fileOp.Operation);
            }
          }
          catch (Exception)//ex)
          {

            throw;
          }


        }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 8 });
    }


    ///////// <summary>
    ///////// Returns a block for transforming directories, not yet in the database, into multiple FileInfo instances. 
    ///////// Because we know that none of these files will already be in the database, we can just go ahead and insert them.
    ///////// </summary>
    //////private static TransformManyBlock<DirectoryInfo, FileOp> TransformNewDirectoryToFilesBlock() =>
    //////  new TransformManyBlock<DirectoryInfo, FileOp>(directory =>
    //////    directory.EnumerateImages(SearchOption.TopDirectoryOnly)
    //////      .Select(file => new FileOp(file, DbOperation.Insert)));

    /// <summary>
    /// Returns a block for transforming directories, already fully or partially in the database, into multiple FileInfo instances. 
    /// FileInfo objects are only return for those either missing from the database, or that need updating.
    /// Existing, unchanged files are omitted.
    /// </summary>
    private static TransformManyBlock<DirectoryPath, FileOp> CreateBlock_DirectoryFileOps(State cache) =>
      new(directory =>
        {
          try
          {
            // Is this a directory which already, at least partially, exists in the database? 
            // BUG-HERE: Contains() returned false when directory ended with back-slash.
            if (cache.Directories.Contains(directory))
            {
              // Directory already exists in the database - Only add/update new/changed files - Ignore the rest.
              var fileOps = new List<FileOp>();
              foreach (var file in directory.EnumerateImages(SearchOption.TopDirectoryOnly))
              {
                // DEBUG
                //Console.WriteLine(file.FullName);
                //>

                if (!cache.FilePathToEntityMapping.ContainsKey(file.Value))
                  fileOps.Add(new FileOp(file, DbOperation.Insert));

                else if (cache.HasChanged(file.ToFileInfo()))
                  fileOps.Add(new FileOp(file, DbOperation.Update));
              }
              return fileOps;
            }
            else
            {
              // Directory does not exist in the database - So all files are new files - Add them all

              var t = directory.Value;

              var t2 = directory.EnumerateImages(SearchOption.TopDirectoryOnly).ToArray();
              return t2.Select(file => new FileOp(file, DbOperation.Insert));
            }
          }
          catch (Exception)//ex)
          {

            throw;
          }

        });

  }

}
