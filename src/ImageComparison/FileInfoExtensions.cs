#nullable enable

using Data.Models;
using PW.FailFast;
using PW.IO;
using PW.IO.FileSystemObjects;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace XnaFan.ImageComparison
{
  public static class FileInfoExtensions
  {
    public static async Task<ImageEntity?> TryCreateImageEntityAsync(this FilePath file)
    {
      Guard.NotNull(file, nameof(file));

      try
      {
        var fi = new FileInfo((string)file);

        using var stream = await fi.WaitForAccessAsync(TimeSpan.FromSeconds(3));

        if (stream is null) return null;

        using var image = Image.FromStream(stream);
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
        using var image = Image.FromFile(file.FullName);
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
