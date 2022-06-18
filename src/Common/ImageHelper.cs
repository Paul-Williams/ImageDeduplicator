using PW.Drawing.Imaging;
using PW.Helpers;
using PW.IO.FileSystemObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace PW.ImageDeduplicator.Common;

/// <summary>
/// Used to aid working with webp images in addition to GDI supported images.
/// </summary>
public static class ImageHelper
{

  private static FileExtension WebPExtension { get; } = (FileExtension)".webp";

  public static IEnumerable<FilePath> EnumerateImages(this DirectoryPath directory, SearchOption searchOption)
  {
    return directory is null ? throw new ArgumentNullException(nameof(directory))
      : !directory.Exists ? throw new DirectoryNotFoundException("Directory not found: " + directory.Value)
      : directory
         .EnumerateFiles("*", searchOption)
         .Where(file => GdiImageDecoderFormats.IsSupported(file.Extension.Value) || file.Extension == WebPExtension);
  }

  /// <summary>
  /// Determines if a file type is for a supported image.
  /// </summary>
  public static bool IsSupportedFileType(string filePath)
  {
    var ext = Path.GetExtension(filePath);
    return ext.Length != 0 && (GdiImageDecoderFormats.IsSupported(ext) || IsWebPFileExtension(ext));
  }

  public static System.Drawing.Image LoadImage(string filePath)
  {
    return !File.Exists(filePath) ? throw new FileNotFoundException("File not found: " + filePath)
      : IsWebPFileExtension(Path.GetExtension(filePath))
        ? WebP.WebPDecoder.Load(filePath)
        : System.Drawing.Image.FromFile(filePath);

  }

  public static bool IsWebP(this FileInfo fileInfo) => IsWebPFileExtension(fileInfo.Extension);


  public static bool IsWebPFile(string filePath) => IsWebPFileExtension(Path.GetExtension(filePath));


  public static bool IsWebPFileExtension(string fileExtension) =>
    string.Equals(fileExtension, ".webp", StringComparison.OrdinalIgnoreCase);

}
