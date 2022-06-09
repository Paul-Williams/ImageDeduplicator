# nullable enable

using PW.Data.EntityFramework;
using PW.Extensions;
using PW.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XnaFan.ImageComparison;

namespace Data
{
  public static partial class DataContextHelper
  {
    //public static TimeSpan OneMinute = TimeSpan.FromMinutes(1);
    //public static TimeSpan ThirtySeconds = TimeSpan.FromSeconds(30);

    //private static void AddToList(this ChangeInfo obj, List<ChangeInfo> changed, List<ChangeInfo> missing)
    //{
    //  if (obj.IsMissing) missing.Add(obj);
    //  else if (obj.HasChanged) changed.Add(obj);
    //}

    //private static (List<ChangeInfo> missing, List<ChangeInfo> changed) MissingAndChanged(this IQueryable<ChangeInfo> query)
    //{
    //  var missing = new List<ChangeInfo>();
    //  var changed = new List<ChangeInfo>();

    //  foreach (var item in query)
    //  {
    //    if (item.IsMissing) missing.Add(item);
    //    else if (item.RequiresUpdate) changed.Add(item);
    //  }

    //  return (missing, changed);
    //}

    //public static async Task<(int Deleted, int Updated)> RefreshDataAsync()
    //{
    //  return await Task.Run(async () =>
    //  {
    //    using (var context = new DataContext())
    //    {
    //      int deleted = 0;

    //      var (missing, changed) = context.ImageInfos
    //        .Select(x => new ChangeInfo()
    //        {
    //          Id = x.Id,
    //          LastWriteTime = x.LastWriteTime,
    //          Path = x.Path
    //        })
    //        .MissingAndChanged();

    //      if (missing.Count != 0)
    //        deleted = await context.ExecuteDeleteByIdAsync<Models.ImageInfoEntity, int>(missing.Select(x => x.Id).ToArray());

    //      if (changed.Count != 0)
    //      {
    //        foreach (var item in changed)
    //        {
    //          var entity = (await context.ImageInfos.FindAsync(item.Id)).UpdateFrom(new ImageInfo(item.Path));
    //        }
    //        await context.SaveChangesAsync();

    //      }
    //      return (deleted, changed.Count);

    //    }
    //  });
    //}

    ///// <summary>
    ///// Update all database entities where the file still exists, but the 'LastWriteTime' is different.
    ///// Returns the number of updated entities.
    ///// </summary>
    //public static async Task<int> UpdateEntitiesForChangedFiles()
    //{
    //  return await Task.Run(() =>
    //  {
    //    using (var context = new DataContext())
    //    {

    //      bool Mismatch(string filePath, DateTime entityDate) => File.GetLastWriteTime(filePath).Difference(entityDate) > ThirtySeconds;

    //      var needUpdating = context.ImageInfos.Select(x => new { x.Id, x.LastWriteTime, x.Path })
    //        .ToList()
    //        .Where(x => File.Exists(x.Path) && Mismatch(x.Path, x.LastWriteTime))
    //        .ToList();

    //      if (needUpdating.Count != 0)
    //      {
    //        foreach (var item in needUpdating)
    //        {
    //          var entity = context.ImageInfos.Find(item.Id);
    //          entity.UpdateFrom(new ImageInfo(item.Path));
    //        }
    //        //context.Database.Log = (s) => Trace.WriteLine(s);
    //        return context.SaveChanges();
    //      }
    //      else return 0;
    //    }
    //  });

    //}



    //public static async Task<int> InsertNewAsync(DirectoryInfo source)
    //{
    //  return await Task.Run(() =>
    //  {

    //    // ToList() || !ToList() ? This is the question...
    //    IEnumerable<FileInfo> FilePathsFromDisk() =>
    //      source.EnumerateAuthorizedDirectories(true)
    //      .Where(x => x.Name != "$RECYCLE.BIN")
    //      .SelectMany((x => x.EnumerateGdiSupportedImages(SearchOption.TopDirectoryOnly)));


    //    List<FileInfo> newFiles;

    //    using (var context = new DataContext())
    //    {
    //      newFiles = FilePathsFromDisk().Where(fi => !context.FilePathExists(fi)).ToList();
    //    }

    //    if (newFiles.Count != 0)
    //    {
    //      using (var bi = new BatchedOperations<DataContext, Models.ImageInfoEntity>(() => new DataContext()))
    //      {
    //        ImageInfo.UnsortedList(newFiles)
    //          .Select(x => ImageInfoMapper.ToEntity(x))
    //          .ForEach(x => bi.Insert(x));

    //        return bi.SaveChanges();
    //      }
    //    }
    //    else return 0;
    //  });
    //}

    ///// <summary>
    ///// Creates an entry in the database for the <paramref name="start"/> directory and all sub-directories.
    ///// Only use for directory trees not already in the database, otherwise an exception will be thrown.
    ///// Returns the number of directories imported.
    ///// </summary>
    ///// <param name="start">The directory to import, with sub-directories.</param>
    ///// <returns>The number of directories imported.</returns>
    //public static int ImportDirectories(DirectoryInfo start)
    //{
    //  using (var bulk = new BatchedOperations<DataContext, Models.Directory>(() => new DataContext()))
    //  {
    //    return bulk.Insert(start
    //      .EnumerateAuthorizedDirectories(true)
    //      .Where(x => x.Name != "$RECYCLE.BIN")
    //      .Select(d => new Models.Directory() { Path = d.FullName })
    //    );

    //  }
    //}



    //public static IEnumerable<FileInfo> EnumerateImageFiles(DirectoryInfo start) =>
    //  EnumerateDirectories(start)
    //  .SelectMany(d => d.EnumerateGdiSupportedImages(SearchOption.TopDirectoryOnly));

  }
}
