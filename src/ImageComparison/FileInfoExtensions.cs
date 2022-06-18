#nullable enable

using Data.Models;
using PW.FailFast;
using PW.IO;
using PW.IO.FileSystemObjects;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using static PW.ImageDeduplicator.Common.ImageHelper;

namespace XnaFan.ImageComparison
{
  public static class FileInfoExtensions
  {
    public static async Task<ImageEntity?> TryCreateImageEntityAsync(this FilePath file)
    {
      ArgumentNullException.ThrowIfNull(file);

      try
      {
        var fi = file.ToFileInfo();

        using var stream = await fi.WaitForAccessAsync(TimeSpan.FromSeconds(3));

        if (stream is null) return null;

        // Do not have a method to load WebP from stream.
        using var image = fi.IsWebP() ? LoadImage(fi.FullName) : Image.FromStream(stream);
        return new ImageEntity
        {
          Bytes = Thumbnail.GetBytes(image),
          CreationTime = fi.CreationTime,
          FileSize = fi.Length, // 1024, // Another bad decision :(
          Height = image.Height,
          HorizontalResolution = image.HorizontalResolution,
          LastWriteTime = fi.LastWriteTime,
          Path = fi.FullName,
          VerticalResolution = image.VerticalResolution,
          Width = image.Width
        };
      }
      catch (FileNotFoundException) { return null; }
      catch (OutOfMemoryException) { return null; }
    }


    public static ImageEntity ToImageEntity(this FileInfo file)
    {
      Guard.NotNull(file, nameof(file));

      // BUG  -- FileInfo.Exists value is cached and does not refresh.
      if (!file.Exists) throw new FileNotFoundException("File not found: " + file.FullName);


      try
      {
        using var image = LoadImage(file.FullName); //Image.FromFile(file.FullName);
        return new ImageEntity
        {
          Bytes = Thumbnail.GetBytes(image),
          CreationTime = file.CreationTime,
          FileSize = file.Length, // 1024, Another bad decision :(
          Height = image.Height,
          HorizontalResolution = image.HorizontalResolution,
          LastWriteTime = file.LastWriteTime,
          Path = file.FullName,
          VerticalResolution = image.VerticalResolution,
          Width = image.Width
        };
      }
      catch (OutOfMemoryException)
      {
        throw new Exception($"Image file is corrupt: {file.FullName}");
      }


    }
  }
}
