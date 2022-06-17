#nullable enable

using Data.Models;
using Prism.Events;
using PubSubEvents.DatabaseEvents;
using PW.IO.FileSystemObjects;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using XnaFan.ImageComparison;

namespace ImageDeduper
{
  internal class DatabaseUpdater : IDisposable
  {

    public DatabaseUpdater(IEventAggregator ea)
    {
      EA = ea;
      Db = new Data.DataContext();
    }

    private Data.DataContext Db { get; }

    private IEventAggregator EA { get; }


    /// <summary>
    /// Creates or updates an image record. If the image file cannot be read, then the returned <see cref="ImageEntity"/>
    /// will be null. If the image is new the returned value 'IsNew' will be true. 
    /// If an existing image is updated then 'IsNew' will be false.
    /// </summary>
    public async Task<(ImageEntity? ImageEntity, bool IsNew)> UpsertImage(FilePath newPath, FilePath? oldPath)
    {
      // This method handles both created and changed files.
      // If a file is modified AND renamed in the same batch (unlikely, but allowed for)
      // then the file will have both the new and old paths available.
      // When this is the case, then old path will be the one stored in the database and
      // should be used when attempting to retrieve the existing entity.

      // First attempt to load the image from disk. 
      // The may fail if the image is still being downloaded, or is otherwise read-locked.
      // Because of this we will wait a period. Note that this will block the current thread.
      // Aside: WaitForAccess currently only works with FileInfo. Uses Sleep() internally.

      var fileEntity = await (oldPath ?? newPath).TryCreateImageEntityAsync();
      if (fileEntity is null)
      {
        return (null, false);
      }


      var dbPath = (string)(oldPath ?? newPath);

      // Attempt to retrieve an existing entity
      var dbEntity = Db.Images.FirstOrDefault(x => x.Path == dbPath);

      // If there is an existing entity for the path, then we are to perform an update.
      // If there is not, then perform an insert.
      var isNew = dbEntity is null;
      if (dbEntity is null)
      {
        dbEntity = Db.Images.Add(fileEntity);
        isNew = true;
      }
      else dbEntity.MergeChangesFrom(fileEntity);
      Db.SaveChanges();
      return (dbEntity, isNew);
    }


    public int DeleteAllImages(DirectoryPath directoryPath) => Db.DeleteAllImages(directoryPath);

    private static SqlParameter PathParam(string name, DirectoryPath path) =>
      new(name, path.Value) { SqlDbType = System.Data.SqlDbType.NVarChar, Size = 2000 };

    /// <summary>
    /// Changes the path for images where the path starts with <paramref name="oldPath"/>.  Directly updates database using SProc.
    /// </summary>
    public int RenameDirectory(DirectoryPath oldPath, DirectoryPath newPath)
    {
      var rows = Db.Database.ExecuteSqlCommand("EXEC ChangeImagePath @oldPath,@newPath", 
        PathParam("@oldPath", oldPath), PathParam("@newPath", newPath));

      EA.GetEvent<DirectoryRenamedEvent>()
        .Publish(new DirectoryRenamedEventArgs(oldPath, newPath, rows));

      return rows;
    }


    public void DeleteImage(FilePath image)
    {
      try
      {
        Db.DeleteImage(image);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.ToString());
      }
    }

    public void ChangePath(FileRenamePair paths) => ChangePath(paths.NewPath, paths.OldPath!);

    public void ChangePath(FilePath newPath, FilePath oldPath)
    {
      try
      {
        Db.ChangeImagePath(newPath, oldPath);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.ToString());
      }
    }

    public void Dispose() => Db?.Dispose();
  }
}
